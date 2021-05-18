using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Variables

    // States
    private bool canRun = true;
    private bool canCrouch = true;
    private bool canJump = true;
    public bool isCrouching;
    public bool isParrying;
    public bool isRunning;
    public bool isDashing;
    public bool isJumping;
    public bool isGrounded;
    public bool isAiming;
    public bool canFire = true;

    public float runSpeed;
    public float jumpForce;
    private float moveInput;
    public bool facingRight;

    private bool canDash = true;
    public float dashSpeed;
    public float dashCooldown;
    public float dashDuration;

    public Transform wheelsPosition;
    public float groundCheckRadius;
    public LayerMask whatIsGround;

    private float jumpTimer;
    public float jumpTimeLeft;

    private Rigidbody2D rb2d;
    private CircleCollider2D cc2d;
    private float baseGravity;

    public Transform snapFirePoint;
    public GameObject SnapFirePrefab;
    public float snapFireSpeed;

    public SpriteRenderer playerSprite;
    public SpriteRenderer armSprite;

    public GameObject firingArm;

    #endregion

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        cc2d = GetComponent<CircleCollider2D>();
        baseGravity = rb2d.gravityScale;
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(wheelsPosition.position, groundCheckRadius, whatIsGround); //check if player is on the ground or not

        Run();
        Jump();
        Parry();
        Dash();
        Crouch();
        PlayerSpriteFlip();
        Aim();
        SnapFire();
    }

    private void Run()
    {
        if (canRun == true && isDashing == false && isCrouching == false)
        {
            moveInput = Input.GetAxis("Horizontal"); //set moveInput for further use
            var move = Input.GetAxis("Horizontal"); //horizontal move
            rb2d.velocity = new Vector2(moveInput * runSpeed, rb2d.velocity.y);
        }

        if (rb2d.velocity != Vector2.zero)
        {
            isRunning = true;
        }
        else
        {
            isRunning = false;
        }
    }

    private void Jump()
    {
        if (Input.GetButtonDown("A") && isGrounded == true && isDashing == false && canJump == true) //player jump
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

    private void Parry()
    {
        if (Input.GetButtonDown("A") && isJumping == true && isDashing == false && isParrying == false)
        {
            isParrying = true;
        }
    }

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
        }
    }

    private IEnumerator DashCooldown()
    {
        yield return new WaitForSecondsRealtime(dashCooldown);
        canDash = true;
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
        if (Input.GetAxisRaw("Vertical") < -0.75 && isGrounded == true && canCrouch == true) //player crouch
        {
            cc2d.enabled = false;
            canRun = false;
            isCrouching = true;

            if (facingRight == true)
            {
                snapFirePoint.localPosition = new Vector3(1.26f, -0.5f, 0f);
            }
            else
            {
                snapFirePoint.localPosition = new Vector3(1.26f, 0.5f, 0f);
            }
        }
        else
        {
            cc2d.enabled = true;
            canRun = true;
            isCrouching = false;
            snapFirePoint.localPosition = new Vector3(1.26f, 0f, 0f);
        }
    }

    private void SnapFire()
    {
        if (Input.GetButton("X") && isDashing == false && canFire == true)
        {
            Instantiate(SnapFirePrefab, snapFirePoint.position, snapFirePoint.rotation); //projectile spawn point
            StartCoroutine(SnapFireCooldown());
            canFire = false;
        }
    }

    private void Aim()
    {
        if (Input.GetButton("Right_Bumper") && isDashing == false && isGrounded == true)
        {
            canRun = false;
            canCrouch = false;
            canJump = false;
            isAiming = true;

            Vector2 direction = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            firingArm.transform.localRotation = Quaternion.Euler(0, 0, Mathf.Round(angle/45f) * 45f);

            if (facingRight == false)
            {
                armSprite.flipX = false;
            }
        }
        else
        {
            canRun = true;
            canCrouch = true;
            canJump = true;
            isAiming = false;
        }
    }

    private IEnumerator SnapFireCooldown()
    {
        yield return new WaitForSecondsRealtime(snapFireSpeed); //player can't spam fire
        canFire = true;
    }

    private void PlayerSpriteFlip()
    {
        if (moveInput > 0 && !facingRight) //player is always fracing the direction he's going
        {
            facingRight = !facingRight;
            playerSprite.flipX = false;
            armSprite.flipX = false;
            firingArm.transform.localRotation = Quaternion.identity;
        }
        else if (moveInput < 0 && facingRight)
        {
            facingRight = !facingRight;
            playerSprite.flipX = true;
            armSprite.flipX = true;
            firingArm.transform.localRotation = Quaternion.Euler(0, 0, 180);
        }
    }
}
