using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private PlayerController playerControllerVar;
    public Animator animator;
    public SpriteRenderer armSprite;
    //public SpriteRenderer playerSprite;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerControllerVar = GetComponent<PlayerController>();
        armSprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
        //playerSprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (playerControllerVar.isGrounded == true && playerControllerVar.isCrouching == false && playerControllerVar.isRunning == false)
        {
            animator.Play("Idle");
        }

        if (playerControllerVar.canFire == false && playerControllerVar.isGrounded == true && playerControllerVar.isCrouching == false && playerControllerVar.isDashing == false && playerControllerVar.isRunning == false && playerControllerVar.isAiming == false)
        {
            animator.Play("Idle_Shooting");
        }

        if (playerControllerVar.isAiming == true)
        {
            animator.Play("Aim");
            armSprite.enabled = true;
        }
        else
        {
            armSprite.enabled = false;
        }

        if (playerControllerVar.isGrounded == true && playerControllerVar.isCrouching == false && playerControllerVar.isDashing == false && playerControllerVar.isRunning == true)
        {
            animator.Play("Run");
        }

        if (playerControllerVar.canFire == false && playerControllerVar.isGrounded == true && playerControllerVar.isCrouching == false && playerControllerVar.isDashing == false && playerControllerVar.isRunning == true)
        {
            animator.Play("Run_Shooting");
        }

        if (playerControllerVar.isGrounded == false && playerControllerVar.isDashing == false)
        {
            animator.Play("Jump");
        }

        if (playerControllerVar.isCrouching == true)
        {
            animator.Play("Crouch");
        }

        if (playerControllerVar.isCrouching == true && playerControllerVar.canFire == false)
        {
            animator.Play("Crouch_Shooting");
        }

        if (playerControllerVar.isDashing == true)
        {
            animator.Play("Dash");
        }
    }
}
