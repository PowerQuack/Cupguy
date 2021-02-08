using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float walkSpeed;
    public float jumpForce;
    private float moveInput;

    private bool isJumping;
    private bool isGrounded;
    public Transform wheelsPosition;
    public float groundCheckRadius;
    public LayerMask whatIsGround;

    private float jumpTimer;
    public float jumpTime;

    private Rigidbody2D rb2d;
    private CircleCollider2D cc2d;

    // Use this for initialization
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        cc2d = GetComponent<CircleCollider2D>();
    }

    //FixedUpdate is called at a fixed interval and is independent of frame rate. Put physics code here.
    void FixedUpdate()
    {
        moveInput = Input.GetAxisRaw("Horizontal"); //set moveInput for further use
        
        var horizontalMovement = Input.GetAxisRaw("Horizontal"); //horizontal move
        rb2d.velocity = new Vector2(moveInput * walkSpeed, rb2d.velocity.y);
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(wheelsPosition.position, groundCheckRadius, whatIsGround); //check if player is on the ground or not

        if (Input.GetButtonDown("A") && isGrounded == true) //player jump
        {
            rb2d.velocity = Vector2.up * jumpForce;
            jumpTimer = jumpTime;
            isJumping = true;
        }

        if (Input.GetButton("A") && isJumping == true) //the player can hold A to jump higher
        {
            if (jumpTimer > 0)
            {
                rb2d.velocity = Vector2.up * jumpForce;
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

        if (isGrounded == false && Input.GetButtonDown("A"))
        {
            //parry
        }

        if (Input.GetAxisRaw("Vertical") == -1 && isGrounded == true)
        {
            Debug.Log("Crouch!");
            //cc2d.SetActive(false);
        }

        if (moveInput > 0) //player is always fracing the direction he's going
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        } else if (moveInput < 0)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
    }
}
