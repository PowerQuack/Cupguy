using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Variables
    private bool canRun = true;
    public float runSpeed;
    public float jumpForce;
    private float moveInput;

    private bool canDash = true;
    private bool isDashing;
    public float dashSpeed;
    public float dashCooldown;
    public float dashDuration;

    private bool isJumping;
    private bool isGrounded;
    public Transform wheelsPosition;
    public float groundCheckRadius;
    public LayerMask whatIsGround;

    private float jumpTimer;
    public float jumpTimeLeft;

    private Rigidbody2D rb2d;
    private CircleCollider2D cc2d;
    private SpriteRenderer bowlFace;
    private float baseGravity;
    #endregion

    // Use this for initialization
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        cc2d = GetComponent<CircleCollider2D>();
        bowlFace = transform.GetChild(0).GetComponent<SpriteRenderer>();
        baseGravity = rb2d.gravityScale;
    }

    void Update()
    {
        Run();
        Jump();
        Dash();
        Crouch();
        PlayerSpriteFlip();

        isGrounded = Physics2D.OverlapCircle(wheelsPosition.position, groundCheckRadius, whatIsGround); //check if player is on the ground or not

        if (isGrounded == false && Input.GetButtonDown("A")) //aerial parry
        {
            //parry
        }
    }

    private void Run()
    {
        if (canRun == true && isDashing == false)
        {
            moveInput = Input.GetAxisRaw("Horizontal"); //set moveInput for further use

            var horizontalMovement = Input.GetAxisRaw("Horizontal"); //horizontal move
            rb2d.velocity = new Vector2(moveInput * runSpeed, rb2d.velocity.y);
        }
    }

    private void Jump()
    {
        if (Input.GetButtonDown("A") && isGrounded == true && isDashing == false) //player jump
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, jumpForce);
            jumpTimer = jumpTimeLeft;
            isJumping = true;
        }

        if (Input.GetButton("A") && isJumping == true && isDashing == false) //the player can hold A to jump higher
        {
            if (jumpTimer > 0)
            {
                rb2d.velocity = new Vector2(rb2d.velocity.x, jumpForce);
                jumpTimer -= Time.deltaTime;
            }
            else //make the player fall
            {
                isJumping = false;
            }
        }

        if (Input.GetButtonUp("A")) //player released A so he can't double jump
        {
            isJumping = false;
        }
    }

    private void Dash()
    {
        if(Input.GetButtonDown("Y") && canDash == true) //check if player is dashing and in which direction
        {
            isDashing = true;
            rb2d.gravityScale = 0; //make sure the dash is straight
            rb2d.velocity = Vector2.zero; //make sure ancient velocity doesn't add with the added force
            rb2d.AddForce(new Vector2(moveInput, 0f) * dashSpeed, ForceMode2D.Impulse);
            StartCoroutine(DashDuration());
            StartCoroutine(DashCooldown());
            Debug.Log("Dash");
        }
    }

    private IEnumerator DashCooldown()
    {
        yield return new WaitForSecondsRealtime(dashCooldown);
        canDash = true;
        Debug.Log("cooldownEnd");
    }

    private IEnumerator DashDuration()
    {
        yield return new WaitForSecondsRealtime(dashDuration);
        rb2d.velocity = Vector2.zero;
        isDashing = false;
        rb2d.gravityScale = baseGravity;
    }

    private void Crouch()
    {
        if (Input.GetAxisRaw("Vertical") < 0 && isGrounded == true) //player crouch
        {
            cc2d.enabled = false;
            canRun = false;
        }
        else
        {
            cc2d.enabled = true;
            canRun = true;
        }
    }

    private void PlayerSpriteFlip()
    {
        if (moveInput > 0) //player is always fracing the direction he's going
        {
            bowlFace.flipX = false;
        }
        else if (moveInput < 0)
        {
            bowlFace.flipX = true;
        }
    }
}
