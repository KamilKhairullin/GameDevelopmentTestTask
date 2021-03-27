using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using UnityEngine.SceneManagement;
public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement instance;
    private Rigidbody2D rigitbody;
    private Animator animator;

    //object to check in Character touches the ground
    public Transform groundCheck;
    //object to check in Character touches the wall
    public Transform wallCheck;
    //object to set which sprites are ground and which are not.
    public LayerMask whatIsGround;


    [SerializeField] private float
        //speed of character
        speed,
        //force of jump of character
        jumpForce,
        normalJumpForce,
        //variable, defining in which radius around character's feet ground can be detected
        groundCheckRadius,
        //variable, defining in which radius around character wall can be detected
        wallCheckDistance,
        //speed of sliding from the wall
        wallSlideSpeed,
        jumpHeightMultiplier,
        wallJumpOffForce,
        wallJumpForce,
        airDragMultiplier,
        movementForceInAir;
    
    [SerializeField] private Vector2 
        //variable defining maximum number of jumps in a row(2 is doublejump)
         wallJumpCoeficient;

    [SerializeField] private int 
        amountOfJumps;
    
    //variable to compare float with zero
    private double TOLERANCE = 1e-3;
    
    private int
        //variable using to check how much jumps can be done in every moment of game.
        jumpsLeft,
        //defines direction of jumping while sliding from the wall (-1 is left, 1 is right)
        wallJumpFacingDirection;
    
    //defines direction of movement (diapason from -1 to 1)
    private float movementDirection;

    private float[]
        buffDetailsPlayer = new float[3];
    [SerializeField] private Image image;
    
    private bool
        isMoving,
        isGrouned,
        isTouchingWall,
        isWallSliding,
        lookingRight = true,
        canJump,
        canFlip = true;

    void Start()
   {
       if (instance == null)
       {
           instance = this;
       }
       rigitbody = GetComponent<Rigidbody2D>();
       animator = GetComponent<Animator>();
       jumpsLeft = amountOfJumps;
       wallJumpFacingDirection = 1;
       normalJumpForce = jumpForce;
       buffDetailsPlayer[0] = 0.0f;
       buffDetailsPlayer[1] = 0.0f;
       buffDetailsPlayer[2] = 0.0f;
       image.enabled = false;
       //wallJumpCoeficient.Normalize();
   }

    void Update()
    {
        if (Input.GetKey("escape"))
        {
            SceneManager.LoadScene("Menu");
        }

        checkInput();
        checkDirection();
        UpdateAnimation();
        IsCanJump();
        CheckIfWallSliding();
        CheckBuff();
    }

    private void FixedUpdate()
    {
        Move();
        CheckSurrounding();
    }

    /**
     * function checks input from keyboard each Update()
     * movementDirection is input of movement by X-axis
     * Input.GetButtonDown("Jump") is makes character jump at the moment when SPACE button pressed
    */
    private void checkInput()
    {
        movementDirection = Input.GetAxisRaw("Horizontal");
        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }
        /*
        if (Input.GetButtonUp("Jump"))
        {
            rigitbody.velocity = new Vector2(rigitbody.velocity.x, rigitbody.velocity.y * jumpHeightMultiplier);
        }*/
    }
    /**
     * function implements character's moving
     * Character can move if he is on ground
     * first else if implements movement in air if some X-axis input is existing
     * second else if implements movement in air if no X-axis input is existing
     *  last if implements character's wall sliding
    */
    private void Move()
    {
        if (isGrouned)
        {
            rigitbody.velocity = new Vector2(speed * movementDirection, rigitbody.velocity.y);
        }
        else if (!isGrouned && !isWallSliding && Math.Abs(movementDirection) > TOLERANCE)
        {
            Vector2 forceToAdd = new Vector2(movementForceInAir * movementDirection, 0);
            rigitbody.AddForce(forceToAdd);
            if (Math.Abs(rigitbody.velocity.x) > speed)
            {
                rigitbody.velocity = new Vector2(speed * movementDirection, rigitbody.velocity.y);
            }
        }
        else if (!isGrouned && !isWallSliding && Math.Abs(movementDirection) < TOLERANCE)
        {
            rigitbody.velocity = new Vector2(rigitbody.velocity.x * airDragMultiplier, rigitbody.velocity.y);
        }
        if (isWallSliding)
        {
            if (rigitbody.velocity.y < -wallSlideSpeed)
            {
                rigitbody.velocity = new Vector2(rigitbody.velocity.x, -wallSlideSpeed);
            }
        }
    }
    /**
     * function implements character's jumping
     * if (canJump && !isWallSliding) is jumping if character is not sliding from wall
     *  First "else if" stands for jumping off the wall while sliding
     * Second "else if" stands for jumping wall-by-wall like price of persia
    */
    private void Jump()
    {
        if (canJump)
        {
            rigitbody.velocity = new Vector2(rigitbody.velocity.x, jumpForce);
            jumpsLeft --;
        }
        //jump off
        else if (isWallSliding && Math.Abs(movementDirection) < TOLERANCE)
        {
            isWallSliding = false;
            Vector2 forceToAdd = new Vector2(wallJumpOffForce  * -wallJumpFacingDirection,
                -wallJumpOffForce);
            rigitbody.AddForce(forceToAdd, ForceMode2D.Impulse);
        }
        else if (isWallSliding && Math.Abs(movementDirection) >= 1.0f && movementDirection * wallJumpFacingDirection < 0) // wall jump
        {
            isWallSliding = false;
            Vector2 forceToAdd = new Vector2(wallJumpCoeficient.x * wallJumpForce * -wallJumpFacingDirection,
                wallJumpCoeficient.y *  wallJumpForce);
            rigitbody.AddForce(forceToAdd, ForceMode2D.Impulse);
        }
    }

    /*
     * When player pick up potion, signal comes here
     */
    public void GetBuff(float[] buffDetails)
    {
        buffDetailsPlayer = buffDetails;
    }

    private void CheckBuff()
    {
        if (Time.time < buffDetailsPlayer[2] + buffDetailsPlayer[1])
        {
            jumpForce = buffDetailsPlayer[0];
            image.enabled = true;
        }
        else
        {
            image.enabled = false;
            jumpForce = normalJumpForce;
        }
    }
    /**
     * function checks if character can jump or not
     * Character can jump only if he standing on ground
     * and he have jumps left
    */
    private void IsCanJump()
    {
        if (isGrouned && rigitbody.velocity.y <= TOLERANCE)
        {
            jumpsLeft = amountOfJumps;
        }

        if (jumpsLeft > 0 && !isWallSliding)
        {
            canJump = true;
        }
        else
        {
            canJump = false;
        }
    }
    /**
     * function checks is character wall sliding or not
     * and stores result in isWallSliding
    */
    private void CheckIfWallSliding()
    {
        if (isTouchingWall && !isGrouned && rigitbody.velocity.y < TOLERANCE)
        {
            isWallSliding = true;
        }
        else
        {
             isWallSliding = false;
        }
    }
    /**
     * function checks is character touch ground or not and stores result in isGrouned
     * and
     * is character touch wall or not and stores result in  isTouchingWall
    */
    private void CheckSurrounding()
    {
        isGrouned = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, whatIsGround);
    }
    /**
     * function checks is character's isMoving state
     * and
     * checks if character looks right in lookingRight
     * and
     * rotates all the world around character when he goes left
    */
    private void checkDirection()
    {
        if ((movementDirection > 0 && !lookingRight) || (movementDirection < 0 && lookingRight))
        {
            if (!isWallSliding && canFlip)
            {
                wallJumpFacingDirection *= -1;
                transform.Rotate(0.0f, 180.0f, 0.0f);
                lookingRight = !lookingRight;

            }
        }
        isMoving = !(Math.Abs(rigitbody.velocity.x) < TOLERANCE);
    }
    
    /*function enables and disables Flipping of world around character
     */
    private void disableFlip()
    {
        canFlip = false;
    }
    private void enableFlip()
    {
        canFlip = true;
    }
    /**
     * function updates all animator's booleans.
    */
    private void UpdateAnimation()
    {
        animator.SetBool("moving", isMoving);
        animator.SetBool("grounded", isGrouned);
        animator.SetFloat("yVelocity", rigitbody.velocity.y);
        animator.SetBool("sliding", isWallSliding);
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance,
                wallCheck.position.y, wallCheck.position.z)
        );
    }
}
