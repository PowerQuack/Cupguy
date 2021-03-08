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
    private bool facingRight = false;

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
    private float baseGravity;

    private bool canFire = true;
    public Transform SnapFirePoint;
    public GameObject SnapFirePrefab;
    public float snapFireSpeed;
    public GameObject ArmPivot;

    public Animator animator;
    #endregion

    // Use this for initialization
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        cc2d = GetComponent<CircleCollider2D>();
        baseGravity = rb2d.gravityScale;
    }

    void Update()
    {
        Run();
        Jump();
        Dash();
        Crouch();
        PlayerSpriteFlip();
        SnapFire();

        isGrounded = Physics2D.OverlapCircle(wheelsPosition.position, groundCheckRadius, whatIsGround); //check if player is on the ground or not
    }

    private void Run()
    {
        if (canRun == true && isDashing == false)
        {
            moveInput = Input.GetAxisRaw("Horizontal"); //set moveInput for further use

            var move = Input.GetAxisRaw("Horizontal"); //horizontal move
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

    /*private void Parry()
    {
        if (Input.GetButtonDown("A") && isJumping == true && isDashing == false && isParrying == false)
        {
            isParrying = true;
        }
    }*/

    private void Dash()
    {
        if (Input.GetButtonDown("Y") && canDash == true) //check if player is dashing and in which direction
        {
            isDashing = true;
            canDash = false;
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
        if (Input.GetAxisRaw("Vertical") < -0.75 && isGrounded == true) //player crouch
        {
            cc2d.enabled = false;
            canRun = false;
            //animator.Play("Player_Crouch");
        }
        else
        {
            cc2d.enabled = true;
            canRun = true;
            //animator.Play("Player_Idle");
        }
    }

    private void SnapFire()
    {
        if (Input.GetButton("X") && isDashing == false && canFire == true)
        {
            Instantiate(SnapFirePrefab, SnapFirePoint.position, SnapFirePoint.rotation); //projectile spawn point
            StartCoroutine(SnapFireCooldown());
            canFire = false;
        }
    }

    private void Aim()
    {
        if (Input.GetAxis("Left_Trigger") < 0.5f && isDashing == false && canFire == true)
        {
            canRun = false;
            Debug.Log(canRun);
            //Input.GetAxis("Left_Joystick") == aim direction
            ArmPivot.transform.eulerAngles = new Vector3(0, 0, 90);
        }
        else
        {
            canRun = true;
        }
    }

    private IEnumerator SnapFireCooldown()
    {
        yield return new WaitForSecondsRealtime(snapFireSpeed); //player can't spam fire
        canFire = true;
    }

    private void PlayerSpriteFlip()
    {
        if (moveInput > 0 && facingRight == true) //player is always fracing the direction he's going
        {
            facingRight = !facingRight;
            transform.Rotate(0, 180, 0);
        }
        else if (moveInput < 0 && !facingRight)
        {
            facingRight = !facingRight;
            transform.Rotate(0, 180, 0);
        }
    }
}
