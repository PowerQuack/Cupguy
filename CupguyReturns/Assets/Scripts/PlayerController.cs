using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float walkSpeed;
    public float jumpSpeed;
    private bool isJumping = false;

    private Rigidbody2D rb2d;

    // Use this for initialization
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    //FixedUpdate is called at a fixed interval and is independent of frame rate. Put physics code here.
    void FixedUpdate()
    {
        var horizontalMovement = Input.GetAxis("Horizontal");
        transform.position += new Vector3(horizontalMovement, 0, 0) * walkSpeed * Time.deltaTime;

        if (isJumping == false)
        {
            var verticalMovement = Input.GetButtonDown("A");
            transform.position += new Vector3(0, verticalMovement, 0) * jumpSpeed * Time.deltaTime;
            isJumping = true;
        }
    }
}
