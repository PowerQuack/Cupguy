using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float runSpeed;
    public float jumpForce;
    private float moveInput;

    public float dashSpeed;
    private bool isDashing;
    private float dashTimer;
    public float dashTime;

    private bool isJumping;
    private bool isGrounded;
    public Transform wheelsPosition;
    public float groundCheckRadius;
    public LayerMask whatIsGround;

    private float jumpTimer;
    public float jumpTimeLeft;

    private Rigidbody2D rb2d;
    private CircleCollider2D cc2d;

    // Use this for initialization
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        cc2d = GetComponent<CircleCollider2D>();
    }

    void Update()
    {
        Run();
        Jump();
        Dash();
        PlayerSpriteFlip();

        isGrounded = Physics2D.OverlapCircle(wheelsPosition.position, groundCheckRadius, whatIsGround); //check if player is on the ground or not

        if (isGrounded == false && Input.GetButtonDown("A")) //aerial parry
        {
            //parry
        }

        if (Input.GetAxisRaw("Vertical") == -1 && isGrounded == true) //player crouch
        {
            cc2d.enabled = false;
            moveInput = 0; //make sure the player can't move when he crouches
        }
        else
        {
            cc2d.enabled = true;
        }

    }
    private void Run()
    {
        moveInput = Input.GetAxisRaw("Horizontal"); //set moveInput for further use

        var horizontalMovement = Input.GetAxisRaw("Horizontal"); //horizontal move
        rb2d.velocity = new Vector2(moveInput * runSpeed, rb2d.velocity.y);
    }

    private void Jump()
    {
        if (Input.GetButtonDown("A") && isGrounded == true) //player jump
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, jumpForce);
            jumpTimer = jumpTimeLeft;
            isJumping = true;
        }

        if (Input.GetButton("A") && isJumping == true) //the player can hold A to jump higher
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
        if(Input.GetButton("Y")) //check if player is dashing and in which direction
        {
            Debug.Log("Start of dash");

            if (dashTimer <= 0)
            {
                rb2d.velocity = Vector2.zero; //make sure the velocity gets back to zero
                dashTimer = dashTime;
                Debug.Log("End of dash");
            }
            else
            {
                dashTimer -= Time.deltaTime; //can't spam dash

                if (Input.GetAxisRaw("Horizontal") != 0)
                {
                    rb2d.velocity = new Vector2(moveInput * dashSpeed, rb2d.velocity.y);
                }
            }
        }
    }

    private void PlayerSpriteFlip()
    {
        if (moveInput > 0) //player is always fracing the direction he's going
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else if (moveInput < 0)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
    }
}
