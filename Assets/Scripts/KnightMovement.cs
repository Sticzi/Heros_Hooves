using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightMovement : MonoBehaviour
{
    private KnightController2D controller;
    
    public bool isKnightControlled;
    public bool isAttacking;

    public string whoIsControlled = "horse";

    float horizontalMove = 10f;
    public float runSpeed;

    [SerializeField] private float currentSpeed;
    [SerializeField] private LayerMask whatIsHorse;
    [SerializeField] private Vector2 offset;
    [SerializeField] private float Radius;  


    public void Awake()
    {
        currentSpeed = runSpeed;
        controller = GetComponent<KnightController2D>();
    }

    public void Update()
    {
        
        Collider2D collider = Physics2D.OverlapCircle(new Vector2(transform.position.x - offset.x, transform.position.y - offset.y), Radius, whatIsHorse);
        if (collider != null)
        {
            controller.InRangeOfHorse();
        }
        else
        {
            controller.OutOfRangeOfHourse();
        }        

        if (isKnightControlled&&!isAttacking)
        {
            //usuń Raw żeby uzyskać płynny ruch
            horizontalMove = Input.GetAxisRaw("Horizontal") * currentSpeed;
            controller.Move(horizontalMove * Time.fixedDeltaTime);

            if (Input.GetButtonDown("Jump"))
            {
                controller.Attack();
            }
        }

        

        if (Input.GetButtonDown("swap"))
        {
            controller.SwapCharacter(whoIsControlled);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(new Vector2(transform.position.x - offset.x, transform.position.y - offset.y), Radius);
    }
}


