using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlatform : MonoBehaviour
{
    public Transform pos1, pos2;
    public float speed;
    public Transform startPos;

    public Vector2 nextPos;

    public bool moving = true;
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        nextPos = startPos.position;
        anim = GetComponent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Horse") || collision.gameObject.CompareTag("Knight") || collision.gameObject.CompareTag("Box"))
        {
            collision.collider.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Horse") || collision.gameObject.CompareTag("Knight") || collision.gameObject.CompareTag("Box"))
        {
            collision.collider.transform.SetParent(null);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position == pos1.position)
        {
            nextPos = pos2.position;
        }

        if(transform.position == pos2.position)
        {
            nextPos = pos1.position;
        }

        if(moving)
        {
            anim.enabled = true;
            transform.position = Vector2.MoveTowards(transform.position, nextPos, speed * Time.deltaTime);
        }
        else
        {
            anim.enabled = false;
        }
        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(pos1.position, pos2.position);
    }
}
