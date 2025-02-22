﻿using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 4.0f;
    public float jumpForce = 7.5f;
    [SerializeField] private float sprintMultiplier = 2f;
    [SerializeField] private float dashSpeed = 150f;
    [SerializeField] private float dashTime = 0.5f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Jump Mechanics")]
    [SerializeField] private float coyoteTime = 0.2f;
    [SerializeField] private float jumpBufferTime = 0.2f;

    [Header("AudioClips")]
    [SerializeField] private AudioClip[] footsteps;
    [SerializeField] private AudioClip dashClip;

    private Rigidbody2D rb;
    private Animator animator;
    private float xInput, coyoteCounter, jumpBufferCounter, currentDashTime, footstepTimer;
    private readonly float footstepDelay = .33f;
    private bool isGrounded, isDashing, canDash, isSprinting, facingRight = true;
    private Player player;

    [HideInInspector] public bool dashActive = false;

    private readonly List<Enemy> damagedEnemies = new();

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GetComponent<Player>();
    }

    private void Update()
    {
        if (!LevelManager.instance.IsGameOver())
        {
            xInput = Input.GetAxis("Horizontal");
            isGrounded = Physics2D.CircleCast(transform.position, .3f, Vector2.down, 0.1f, groundLayer);

            //coyote Time
            coyoteCounter = isGrounded ? coyoteTime : coyoteCounter - Time.deltaTime;

            //jump Buffer
            if (Input.GetButtonDown("Jump"))
                jumpBufferCounter = jumpBufferTime;

            if (jumpBufferCounter > 0 && coyoteCounter > 0)
                Jump();

            //sprint
            if (Input.GetKey(KeyCode.LeftShift) && xInput != 0 && player.stamina > 0)
            {
                isSprinting = true;
                player.ReduceStamina(50f); //stamina drain rate
            }
            else isSprinting = false;

            //dash
            if (Input.GetKeyDown(KeyCode.LeftControl) && !isGrounded && !isDashing && dashActive && canDash)
                StartDash();

            if (isDashing)
                Dash();

            //player flip
            if ((xInput > 0 && !facingRight) || (xInput < 0 && facingRight)) Flip();

            //animator parameters
            animator.SetFloat("AirSpeed", rb.velocity.y);
            animator.SetBool("Grounded", isGrounded);
            animator.SetInteger("AnimState", Mathf.Abs(xInput) > Mathf.Epsilon ? 2 : 0);

            PlayFootsteps();
        }
        else
            rb.velocity = Vector3.zero; 
    }

    private void FixedUpdate()
    {
        if (!isDashing && !LevelManager.instance.IsGameOver())
        {
            float moveSpeed = player.chestMultipliers.speedMultiplier * speed * (isSprinting ? sprintMultiplier : 1f);
            rb.velocity = new Vector2(xInput * moveSpeed, rb.velocity.y);
        }
    }

    private void PlayFootsteps()
    {
        if (isGrounded && Mathf.Abs(xInput) > 0 && footstepTimer <= 0f && rb.velocity.x != 0)
        {
            AudioManager.instance.PlaySFX(footsteps[Random.Range(0, footsteps.Length)]);
            footstepTimer = isSprinting ? footstepDelay / sprintMultiplier : footstepDelay;
        }
        else if (!isGrounded)
            footstepTimer = -1f;
        else
            footstepTimer -= Time.deltaTime;
    }

    private void Jump()
    {
        jumpBufferCounter = 0;
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        coyoteCounter = 0;
        animator.SetTrigger("Jump");
        canDash = true;
    }

    private void StartDash()
    {
        isDashing = true;
        canDash = false;
        AudioManager.instance.PlaySFX(dashClip);
        currentDashTime = dashTime;
    }

    private void Dash()
    {
        rb.velocity = new Vector2((facingRight ? 1 : -1) * dashSpeed, rb.velocity.y);
        currentDashTime -= Time.deltaTime;

        Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(transform.position + new Vector3(0, 0.5f, 0), 1.5f, LayerMask.GetMask("Enemy"));

        //loop through the enemies and apply damage only if they haven't been hit during this dash
        foreach (var enemyCollider in enemiesHit)
        {
            if (enemyCollider.CompareTag("Enemy"))
            {
                Enemy enemy = enemyCollider.GetComponent<Enemy>();
                if (!damagedEnemies.Contains(enemy)) //check if this enemy has already been damaged
                {
                    if(enemy.isDead) return;
                    damagedEnemies.Add(enemy);

                    //apply damage and knockback
                    enemy.TakeDamage(15);
                    Vector2 knockbackDirection = (enemy.transform.position - transform.position).normalized;
                    knockbackDirection.y = Mathf.Abs(knockbackDirection.y);
                    enemy.ApplyKnockback(knockbackDirection, 10f);
                }
            }
        }

        if (currentDashTime <= 0)
            StopDash();
    }

    private void StopDash()
    {
        isDashing = false;
        rb.velocity = new Vector2(0, rb.velocity.y);
        damagedEnemies.Clear();
    }

    private void Flip()
    {
        facingRight = !facingRight;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }
}