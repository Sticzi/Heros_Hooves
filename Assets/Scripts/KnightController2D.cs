using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class KnightController2D : MonoBehaviour
{
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;
    public bool m_AirControl = false;

    private Vector3 m_Velocity = Vector3.zero;
    private bool m_FacingRight = true; 

    private KnightMovement movement;
    private Rigidbody2D rb;
    private Animator anim;

    public GameObject horse;    
      
    public bool IsInRangeOfHorse;


    public Vector2 boxSize = new Vector2(1f, 0.2f);
    public float castDistance = 0.1f;    

    public ContactFilter2D ContactFilter;
    public bool IsGrounded => rb.IsTouching(ContactFilter);

    //Collider2D boundCollider;

    public GameObject virtualCameraHorse;
    public GameObject virtualCameraKnight;

    public bool isKnockedback;

    private void Awake()
    {
        virtualCameraHorse = GameObject.FindGameObjectWithTag("VirtualCameraHorse");
        virtualCameraKnight = GameObject.FindGameObjectWithTag("VirtualCameraKnight");
        movement = GetComponent<KnightMovement>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void FixedUpdate()  
    {
        // Check if the knight is grounded
        if (IsGrounded)
        {
            isKnockedback = false;
            anim.SetBool("IsGrounded", true);
        }
        else
        {
            anim.SetBool("IsGrounded", false);
            anim.SetFloat("velocity.y", rb.velocity.y);            
        }
        anim.SetFloat("speed", Mathf.Abs(rb.velocity.x));

    }    

    public void Move(float move)
    {

        //only control the player if grounded or airControl is turned on
        if ((IsGrounded || m_AirControl) && !isKnockedback)
        {
            // Move the character by finding the target velocity
            Vector3 targetVelocity = new Vector2(move * 10f, rb.velocity.y);
            // And then smoothing it out and applying it to the character
            rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

            // If the input is moving the player right and the player is facing left...
            if (move > 0 && !m_FacingRight)
            {
                // ... flip the player.
                Flip();
            }
            // Otherwise if the input is moving the player left and the player is facing right...
            else if (move < 0 && m_FacingRight)
            {
                // ... flip the player.
                Flip();
            }
        }
        
    }

    public void Attack()
    {
        anim.SetTrigger("attack");
        rb.velocity = new Vector2(0, rb.velocity.y);
    }
    
    public async void SwapCharacter(string whoIsControlled)
    {
        switch (whoIsControlled)
        {
            case "horse":
                movement.whoIsControlled = "knight";
                virtualCameraKnight.GetComponent<CinemachineVirtualCamera>().Follow = this.transform;
                anim.SetBool("isControlled", true);
                movement.isKnightControlled = true;
                horse.GetComponent<HorseMovement>().isHorseControlled = false;
                horse.GetComponent<Rigidbody2D>().velocity = new Vector2(0, horse.GetComponent<Rigidbody2D>().velocity.y);
                //boundCollider = Physics2D.OverlapCircle(transform.position, 0.2f, boundLayerMask); // nie wiem co to tbh

                virtualCameraKnight.GetComponent<CinemachineVirtualCamera>().Priority = 20;

                if(virtualCameraHorse.GetComponent<CinemachineConfiner>().m_BoundingShape2D != virtualCameraKnight.GetComponent<CinemachineConfiner>().m_BoundingShape2D)
                {
                    horse.GetComponent<HorseController2D>().PlayerFreeze();
                    await Task.Delay(750); // HARD CODE TIME next room duration lerp
                    horse.GetComponent<HorseController2D>().KnightAndHorseFreeze();
                }
                

                //freeze
                
                break;

            case "knight":
                movement.whoIsControlled = "horse";
                anim.SetBool("isControlled", false);
                movement.isKnightControlled = false;
                horse.GetComponent<HorseMovement>().isHorseControlled = true;
                gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, gameObject.GetComponent<Rigidbody2D>().velocity.y);

                virtualCameraKnight.GetComponent<CinemachineVirtualCamera>().Priority = 0;

                if (virtualCameraHorse.GetComponent<CinemachineConfiner>().m_BoundingShape2D != virtualCameraKnight.GetComponent<CinemachineConfiner>().m_BoundingShape2D)
                {
                    horse.GetComponent<HorseController2D>().PlayerFreeze();
                    await Task.Delay(750); // HARD CODE TIME next room duration lerp
                    horse.GetComponent<HorseController2D>().KnightAndHorseFreeze();
                }

                break;

        }              
        await Task.Yield();
    }

    public void InRangeOfHorse()
    {
        IsInRangeOfHorse = true;
    }

    public void OutOfRangeOfHourse()
    {
        IsInRangeOfHorse = false;
    }

    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        m_FacingRight = !m_FacingRight;

        transform.Rotate(0f, 180f, 0f);
    }

}
