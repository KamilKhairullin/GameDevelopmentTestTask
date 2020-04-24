using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    private Animator animator;
    private PlayerStats stats;

    //bool determining if character can attack or not
    [SerializeField] private bool combatEnabled;
    [SerializeField] private float
        //variable sets radius of attack
        attack1Radius,
        //variable sets damage of attack
        attack1Damage,
        inputTimer,
        //float for checking last time then attack pressed to swap attack#1 and attack#2
        lastInputTime = Mathf.NegativeInfinity;

    
    [SerializeField] private Transform
        //Transform, which sets hitbox of character
        attackHitBoxPos;
    
    [SerializeField] private LayerMask
        //Layermask for defining if object in map can be damaged or not. 
        whatIsDamagable;
    
    
    private bool 
        //bool checking if attack button was pressed or not
        gotInput,
        //bool with isAttacking state for character
        isAttacking,
        //bool for swapping attack#1 and attack#2 animation
        isFirstAttack;
    
    
    private float[]
        //attack Details: [0] - damage of character's attack
        attackDetails = new float[2];
    
    private void Start()
    {
        stats = GetComponent<PlayerStats>();
;       animator = GetComponent<Animator>();
        animator.SetBool("canAttack", combatEnabled);
    }

    private void Update()
    {
        CheckCombat();
        Attack();
    }
    
    /*
     * Each update checks if attack button pressed
     * Updates time of last attack
     */
    private void CheckCombat()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (combatEnabled)
            {
                gotInput = true;
                lastInputTime = Time.time;
            }
        }
    }
    
    /*
     * function implements attack of character.
     * first if check if enough time after last attack passed 
     * second if updates variables state and updates animator's variables
     */
    private void Attack()
    {
        if (Time.time > lastInputTime + inputTimer)
        {
            gotInput = false;
        }
        if (gotInput)
        {
            if (!isAttacking)
            {
                gotInput = false;
                isAttacking = true;
                isFirstAttack = !isFirstAttack;
                animator.SetBool("attack#1", true);
                animator.SetBool("firstAttack", isFirstAttack);
                animator.SetBool("isAttacking", isAttacking);
               // CheckAttackHitbox();
            }
        }
    }

    /*
     * Function checks Hitbox in front of character and send "Damage" message to all damageble detected objects in this area
     * !!this function is called inside animation using addEvent!!
     */
    private void CheckAttackHitbox()
    {
        Collider2D[] detectedobject = Physics2D.OverlapCircleAll(attackHitBoxPos.position, attack1Radius, whatIsDamagable);
        
        attackDetails[0] = attack1Damage;
        attackDetails[1] = transform.position.x;
        
        foreach (Collider2D collider in detectedobject)
        {
         collider.transform.parent.SendMessage("Damage", attackDetails);
        }
    }
    /*
     * Function updates animator variables.
     * !!this function is called inside animation using addEvent!!
     */
    private void finishAttack1()
    {
        isAttacking = false;
        animator.SetBool("isAttacking", isAttacking);
        animator.SetBool("attack#1", false);
    }

    /*
     * When player takes Damage from Enemy, message from EnemyController comes here.
     */
    public void Damage(float[] attackDetails)
    {
        stats.DecreseHealth(attackDetails[0]);
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attackHitBoxPos.position, attack1Radius);
    }
}
