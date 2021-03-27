using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;
using Vector3 = System.Numerics.Vector3;

public class RangedEnemyController : MonoBehaviour
{
    //variable stores current state of enemy
    private state currentState;
    
    //Finite state machine concept used. Enemy have 3 states and goes over them.
    private enum state
    {
        Fire,
        Knockback,
        Dead
    }
    
    [SerializeField] private Transform 
        touchDamageCheck,
        firePoint;
    
    [SerializeField] private LayerMask 
        whatIsPlayer;

    [SerializeField] private GameObject
        projectile,
        //When enemy is alive he is 'alive' object and hive specific sprites and etc.
        alive,
        //When enemy os dead he becomes another object.
        dead;
    
    private Rigidbody2D 
        aliveRigitbody;
    
    private Animator
        aliveAnimator;

    [SerializeField] private float
        lastTouchDamageTime,
        touchDamageCooldown,
        touchDamage,
        touchDamageWidth,
        touchDamageHeight,
        shootDurationFrom,
        shootDurationTo,
        maxHealth,
        //duration of knockback after enemy was hit by character
        knockbackDuration;
    
    [SerializeField] private Vector2
        //speed of knockback after enemy was hit by character
        knockbackSpeed;
    
    private float 
        lastFlip,
        shootTime,
        //variable storing current Health of enemy 
        currentHealth;


    private float[]
        attackDetails = new float[2]; 
    //variable stores time in which knockback started
    private float 
        knockbackStartTime;
    private int
        //direction of enemy: 1 is facing right, -1 is facing left
        facingDirection,
        //stores direction of damage done by character (-1 is left, 1 is right)
        damageDirection;

    private Vector2
        movement,
        touchDamageBottomLeft,
        touchDamageTopRight;
    
    private void Start()
    {
        alive = transform.Find("Alive").gameObject;
        aliveRigitbody = alive.GetComponent<Rigidbody2D>();
        facingDirection = 1;
        aliveAnimator = alive.GetComponent<Animator>();
        currentHealth = maxHealth;
        shootTime = Random.Range(shootDurationFrom, shootDurationTo);
    }
    
    /*
     * each update is variable currentState changed, state of enemy is changing
     */
    private void Update()
    {
        CheckTouchDamage();
        switch (currentState)
        {
            case state.Fire:
                UpdateFirestate();
                break;
            case state.Knockback:
                UpdateKnockbackstate();
                break;
            case state.Dead:
                UpdateDeadstate();
                break;
        }
    }
    
     
    //Fire state
    private void EnterFireState()
    {
        
    }
    /*
     * if enemy detected wall or cliff in front of  him, flips direction of movement
     */
    private void UpdateFirestate()
    {
        if (Time.time > lastFlip + shootTime)
        {
            lastFlip = Time.time;
            Flip();
            Shoot();
            shootTime = Random.Range(shootDurationFrom, shootDurationTo);
            //  wallDetected = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, whatIsGround);
        }
        //fire projectile
    }

    private void ExitFireState()
    {
        
    }
    
    //knockback state
    /*
     * enemy knockbacks in side oposite to damage direction
     */
    private void EnterKnockbackState()
    {
        knockbackStartTime = Time.time;
        movement.Set(knockbackSpeed.x * damageDirection, knockbackSpeed.y);
        aliveRigitbody.velocity = movement;
        aliveAnimator.SetBool("knockback", true);
    }
    
    /*
     * after knockback duration ended, return to Fire state
     */
    private void UpdateKnockbackstate()
    {
        if (Time.time >= knockbackStartTime + knockbackDuration)
        {
            SwitchState(state.Fire);
        }
    }

    /*
     * updates animator variable when exiting knockback state.
     */
    private void ExitKnockbackState()
    {
        aliveAnimator.SetBool("knockback", false);
    }
    //dead state
    /*
     * replaces enemy object by enemy corpse object
     */
    private void EnterDeadState()
    {
        Instantiate(dead, new UnityEngine.Vector3(alive.transform.position.x, (float) (alive.transform.position.y + 0.45), 0)
            , alive.transform.rotation);
        Destroy(gameObject);
        //dead = transform.Find("Dead").gameObject;
    }
    private void UpdateDeadstate()
    {
        
    }

    private void ExitDeadState()
    {
        
    }
    
    // other functions
    /*
     * function to which between the states
     */
    private void SwitchState(state state)
    {
        switch (currentState)
        {
            case state.Fire:
                ExitFireState();
                break;
            case state.Knockback:
                ExitKnockbackState();
                break;
            case state.Dead:
                ExitDeadState();
                break;
        }
        switch (state)
        {
            case state.Fire:
                EnterFireState();
                break;
            case state.Knockback:
                EnterKnockbackState();
                break;
            case state.Dead:
                EnterDeadState();
                break;
        }

        currentState = state;
    }

    /*
     * flips enemy to oposite direction
     */
    private void Flip()
    {
        facingDirection *= -1;
        alive.transform.Rotate(0.0f, 180.0f, 0.0f);
    }

    /*
     * From playerCombat script receives details about attack
     * and
     * apply changes to enemy.
     * attackDetails[0] is damage done
     * attackDetails[1] is direction of damage
     */
    private void Damage(float[] attackDetails)
    {
        currentHealth -= attackDetails[0];
        if (attackDetails[1] > alive.transform.position.x)
        {
            damageDirection = -1;
        }
        else
        {
            damageDirection = 1;
        }

        if (currentHealth > 0.0f)
        {
            SwitchState(state.Knockback);
        }
        else if (currentHealth <= 0.0f)
        {
            SwitchState(state.Dead);
        }
    }

    /*
     * function checks if character is inside enemy's hitbox
     * if character is, sends message to PlayerCombat script.
     */
    private void CheckTouchDamage()
    {
        if (Time.time >= lastTouchDamageTime + touchDamageCooldown)
        {
            touchDamageBottomLeft.Set(touchDamageCheck.position.x - (touchDamageWidth/2), touchDamageCheck.position.y - (touchDamageHeight/2));
            touchDamageTopRight.Set(touchDamageCheck.position.x + (touchDamageWidth/2), touchDamageCheck.position.y + (touchDamageHeight/2));

            Collider2D hit = Physics2D.OverlapArea(touchDamageBottomLeft, touchDamageTopRight, whatIsPlayer);

            if (hit != null)
            {
                lastTouchDamageTime = Time.time;
                attackDetails[0] = touchDamage;
                attackDetails[1] = alive.transform.position.x;
                hit.SendMessage("Damage", attackDetails);
            }
        }
    }

    private void Shoot()
    {
        Instantiate(projectile, firePoint.position, firePoint.rotation);
    }
    
    private void OnDrawGizmos()
    {
        
        Vector2 botLeft = new Vector2(touchDamageCheck.position.x - (touchDamageWidth/2), touchDamageCheck.position.y - (touchDamageHeight/2));
        Vector2 botRight = new Vector2(touchDamageCheck.position.x + (touchDamageWidth/2), touchDamageCheck.position.y - (touchDamageHeight/2));
        Vector2 topRight = new Vector2(touchDamageCheck.position.x + (touchDamageWidth/2), touchDamageCheck.position.y + (touchDamageHeight/2));
        Vector2 topLeft = new Vector2(touchDamageCheck.position.x - (touchDamageWidth/2), touchDamageCheck.position.y + (touchDamageHeight/2));
        
        Gizmos.DrawLine(botLeft, botRight);
        Gizmos.DrawLine(botRight, topRight);
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topLeft, botLeft);
    }
}
