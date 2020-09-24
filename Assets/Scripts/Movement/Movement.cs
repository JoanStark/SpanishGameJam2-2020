﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    Rigidbody2D rb;
    Collisions coll;

    [SerializeField] private float speed = 10;
    [SerializeField] private float jumpforce = 10;
    [SerializeField] private float timeUntilWallJump = 1.5f;
    

    private bool canMove = true;
    private bool canWallJump = true;
    private bool dashing = false;

    [HideInInspector] public bool extraJump = false;
    public float dashSpeed = 20;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collisions>();
    }

    private void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        Vector2 dir = new Vector2(x, y);

        Walk(dir);

        if (Input.GetKeyDown(KeyCode.Space))
        {            
            if (coll.onGround || extraJump)
            {
                if (extraJump)
                {
                    extraJump = false;
                }

                
                Jump(Vector2.up);
            }
               
            else if (coll.onWall && canWallJump)
                WallJump();
        }

        if (coll.onGround)
        {
            StopCoroutine(DisableWallJump(0));
            canWallJump = true;
        }

    }

    private void Walk(Vector2 dir)
    { 
        if (!canMove || dashing /*|| coll.onWall*/)
            return;

        rb.velocity = (new Vector2(dir.x * speed, rb.velocity.y));
    }

    private void Jump(Vector2 dir)
    {
        rb.velocity = dir * jumpforce;
    }
    private void WallJump()
    {
        //StartCoroutine(DisableWallJump(timeUntilWallJump));


        Vector2 wallDir = coll.onRightWall ? Vector2.left : Vector2.right;

        Jump((Vector2.up / 1.2f + wallDir / 1.5f));
    }

    IEnumerator DisableWallJump(float time)
    {
        canWallJump = false;
        yield return new WaitForSeconds(time);
        canWallJump = true;
    }

    public void Dash(float x, float y)
    {
        rb.velocity = Vector2.zero;
        Vector2 dir = new Vector2(x,y);

        print(dir + " " + dir * dashSpeed);

        rb.velocity = dir * dashSpeed;

        StartCoroutine(DashWait());

    }

    IEnumerator DashWait()
    {
        GetComponent<BetterJump>().enabled = false;
        rb.gravityScale = 0;
        dashing = true;

        yield return new WaitForSeconds(.3f);

        dashing = false;
        GetComponent<BetterJump>().enabled = true;
        rb.gravityScale = 1;
    }

}