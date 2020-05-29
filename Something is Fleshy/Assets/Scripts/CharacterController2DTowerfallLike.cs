﻿using System.Collections;
using UnityEngine;

public class CharacterController2DTowerfallLike : MonoBehaviour
{
	#region CONFIGURATION
#pragma warning disable 0649
	[Header("Movement")]
	[SerializeField] float movementSpeed = 10f;
	[Tooltip("Used for acceleration. Bigger is the value, longer the character make to stop or start moving.")]
	[Range(0, .3f)] [SerializeField] private float movementSmoothing = .05f;
	[Header("Jump")]
	[Tooltip("The horizontal impulsion given to player when he hits jump button holding a direction.")]
	[SerializeField] float initialXJumpForce = 10f;
	[Tooltip("The vertical impulsion given to the player when he hits jump button.")]
	[SerializeField] float initialYJumpForce = 20f;
	[Tooltip("The force add to velocity.y at each frame if the player keeps jumping.")]
	[SerializeField] float incrementYJumpForce = 1f;
	[Tooltip("The value modifying velocity.x each frame when the player is in air. Greater it is greater the air control is.")]
	[SerializeField] float airControl = .25f;
	[Tooltip("Maximum horizontal speed in air, if this value is greater than 'movementSpeed' the player will move faster in air.")]
	[SerializeField] float maxXSpeedInAir = 10f;
	[Tooltip("Total time where player can keep jumping.")]
	[SerializeField] float jumpTimeMax = .35f;
	[Header("Jump polish")]
	[Tooltip("How many frames player can still jump after leaving the ground (Coyote time).")]
	[SerializeField] int nbFramesCoyoteTime = 5;
	[Tooltip("Hom many frames jump input is stocked when the player is not grounded.")]
	[SerializeField] int nbFramesJumpBuffering = 5;
	[Header("Ground Detection")]
	[SerializeField] LayerMask whatIsGround;
	[SerializeField] Transform groundCheck;
	[SerializeField] float groundedRadius = .2f;
	[Header("Debug")]
	[SerializeField] bool showCheckerDebug = true;
	[SerializeField] bool showMovementDebug = true;
	[SerializeField] Color groundedColor = Color.cyan;
	[SerializeField] Color jumpColor = Color.yellow;
	[SerializeField] Color fallColor = Color.red;
#pragma warning restore 0649
	#endregion

	[Header("Components")]
	[Header("DON'T TOUCH BELOW")]
	Rigidbody2D rb;
	ActionsMap actionsMap;

	[Header("Variables")]
	//Movement variable
	bool isGrounded;
	bool wasGrounded;
	bool facingRight = true;
	public float movementInput;
	Vector3 velocity = Vector3.zero;
	//Jump variable
	bool isJumping;
	float jumpTimeCounter;
	//Polish jump variable
	bool jumpBuffering;
	int framesCounterCoyoteTime;
	int framesCounterJumpBuffering;
	//Debug variable
	Color debugColor;

	private void OnEnable() => actionsMap.Gameplay.Enable();
	private void OnDisable() => actionsMap.Gameplay.Disable();

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();

		actionsMap = new ActionsMap();

		actionsMap.Gameplay.Movement.performed += ctx => movementInput = ctx.ReadValue<float>();
		actionsMap.Gameplay.Movement.canceled += ctx => movementInput = 0f;
		actionsMap.Gameplay.Jump.started += ctx => StartJumping();
		actionsMap.Gameplay.Jump.canceled += ctx => StopJumping();
	}

	private void FixedUpdate()
	{
		GroundDetection();
		CoyoteTimeSystem();
		JumpBufferingSystem();
		Move(movementInput);
		KeepJumping();
		if (showMovementDebug)
			Debug.DrawLine(transform.position, transform.position - new Vector3(0, -.1f, 0), debugColor, 10);
	}

	void GroundDetection()
	{
		wasGrounded = isGrounded;
		isGrounded = false;

		Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, groundedRadius, whatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				isGrounded = true;
				if (!wasGrounded)
				{
					//Landing
				}
			}
		}
		if (isGrounded)
			debugColor = groundedColor;
		else
			debugColor = fallColor;
	}

	void CoyoteTimeSystem()
	{
		if (!isGrounded && wasGrounded)
		{
			framesCounterCoyoteTime++;
			if (framesCounterCoyoteTime <= nbFramesCoyoteTime)
				isGrounded = true;
			else
				framesCounterCoyoteTime = 0;
		}
	}

	void JumpBufferingSystem()
	{
		if (isGrounded && jumpBuffering)
		{
			StartJumping();
		}
		if (jumpBuffering)
		{
			framesCounterJumpBuffering++;
			if (framesCounterJumpBuffering > nbFramesJumpBuffering)
				jumpBuffering = false;
		}
	}

	void StartJumping()
	{
		if (isGrounded)
		{
			jumpBuffering = false;
			isGrounded = false;
			isJumping = true;
			framesCounterCoyoteTime = 0;
			jumpTimeCounter = 0f;
			if(movementInput != 0)
				rb.velocity = new Vector2((facingRight ? 1 : -1) * initialXJumpForce, initialYJumpForce);
			else
				rb.velocity = new Vector2(0f, initialYJumpForce);
			debugColor = jumpColor;
		}
		else
		{
			jumpBuffering = true;
			framesCounterJumpBuffering = 0;
		}
	}

	void KeepJumping()
	{
		if (isJumping)
		{
			if (jumpTimeCounter < jumpTimeMax)
			{
				jumpTimeCounter += Time.deltaTime;
				rb.velocity += new Vector2(0f, incrementYJumpForce);
				debugColor = jumpColor;
			}
			else
				StopJumping();
		}
	}

	void StopJumping()
	{
		jumpBuffering = false;
		isJumping = false;
		debugColor = fallColor;
	}

	public void Move(float horizontalMove)
	{
		if (isGrounded)
		{
			Vector3 targetVelocity = new Vector2(horizontalMove * movementSpeed, rb.velocity.y);
			rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, movementSmoothing);

			if (horizontalMove > 0 && !facingRight)
				Flip(false);
			else if (horizontalMove < 0 && facingRight)
				Flip(false);
		}
		else
		{
			if (horizontalMove > 0 && !facingRight)
				Flip(true);
			else if (horizontalMove < 0 && facingRight)
				Flip(true);
			if(horizontalMove != 0)
			{
				rb.velocity += new Vector2((facingRight ? 1 : -1) * airControl, 0f);
				if (Mathf.Abs(rb.velocity.x) > maxXSpeedInAir)
					rb.velocity = new Vector2((facingRight ? 1 : -1) * maxXSpeedInAir, rb.velocity.y);
			}
			else
			{
				if(Mathf.Abs(rb.velocity.x) > airControl)
				{
					rb.velocity -= new Vector2((facingRight ? 1 : -1) * airControl, 0f);
				}
			}
		}
	}

	private void Flip(bool callInAir)
	{
		facingRight = !facingRight;

		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
		if (callInAir)
			rb.velocity = new Vector2(0f, rb.velocity.y);
	}

	private void OnDrawGizmos()
	{
		if (showCheckerDebug)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(groundCheck.position, groundedRadius);
		}
	}
}