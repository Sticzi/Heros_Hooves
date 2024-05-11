using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorseMovement : MonoBehaviour
{

	private HorseController2D controller;
	private Animator animator;

    public float currentSpeed;	
	public float runSpeed = 36;
	public float walkSpeed = 24;

	public float jumpBufferTime;
	[HideInInspector] public float jumpBufferCounter;	

	private Rigidbody2D rb;

	float horizontalMove = 0f;
	bool jump = false;
	public bool isHorseControlled = true;

	public float Radius;
	[SerializeField] private LayerMask whatIsKnight;
	[SerializeField] private Vector2 offset;
	
	[SerializeField] private float holdTime = 1.0f; // Czas przytrzymania w sekundach
	private float timer = 0.0f;

	public void Awake()
    {
		controller = GetComponent<HorseController2D>();
		animator = GetComponent<Animator>();
		rb = GetComponent<Rigidbody2D>();
		currentSpeed = runSpeed;
	}	

	void Update() 
	{
		Collider2D collider = Physics2D.OverlapCircle(new Vector2(transform.position.x - offset.x, transform.position.y - offset.y), Radius, whatIsKnight);
		if (collider != null)
		{
			controller.InRangeOfKnight();
		}
		else
		{
			controller.OutOfRangeOfKnight();
		}

		if (Input.GetButtonDown("Pick/Drop Knight"))
        {
			if(controller.KnightPickedUp)
            {
				controller.KnightDropOfF();
            }
			else
            {			
				if(controller.IsInRangeOfKnight)
                {
					controller.KnightPickUp();
                }
            }
        }
				

		if (Input.GetButton("Down"))
		{
			Debug.Log("anything");
			timer += Time.deltaTime;

			// Sprawdzamy, czy czas przytrzymania przekroczy� 1 sekund�
			if (timer >= holdTime && !controller.isLookingDown)
			{
				controller.isLookingDown = true;
				Debug.Log("Klawisz strza�ki w d� jest przytrzymany przez co najmniej 1 sekund�.");
				// Tutaj mo�esz wykona� dodatkowe akcje, je�li chcesz
			}
		}
		else
		{
			// Je�li klawisz zosta� zwolniony, resetujemy timer i ustawiamy lookingDown na false
			timer = 0.0f;
			if (controller.isLookingDown)
			{
				controller.isLookingDown = false;
				controller.StopLookingDown();
				Debug.Log("Przesta�e� przytrzymywa� klawisz strza�ki w d�.");
				// Tutaj mo�esz wykona� dodatkowe akcje, je�li chcesz
			}
		}


		animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
		animator.SetFloat("Velocity.y", rb.velocity.y);

		if (isHorseControlled)
        {
			//usu� Raw �eby uzyska� p�ynny ruch
			horizontalMove = Input.GetAxisRaw("Horizontal") * currentSpeed;			

			controller.Move(horizontalMove * Time.fixedDeltaTime);

			if (Input.GetButtonDown("Jump"))
			{
				jump = true;
				animator.SetBool("IsJumping", true);
				jumpBufferCounter = jumpBufferTime;

				controller.Jump(jump);
				jump = false;
			}
			else
			{
				jumpBufferCounter -= Time.deltaTime;
			}
		}		
	}		

	public void OnLanding()
	{
		if(controller.IsGrounded&& rb.velocity.y <= 0f)
        {
			controller.doubleJumpEnabled = true;
			controller.isKnockedback = false;
			animator.SetBool("IsJumping", false);			
			//rb.velocity = Vector2.zero;
			controller.m_AirControl = true;
		}
	}

	private void OnDrawGizmosSelected()
	{
		//Gizmos.DrawWireSphere(new Vector2(transform.position.x - offset.x, transform.position.y - offset.y), Radius);
	}
}
