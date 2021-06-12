using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playercontroller : MonoBehaviour
{
    [SerializeField] private float jumpforce = 400f;
    [Range(0, 1)] [SerializeField] private float carryspeed = 0.3f;
    [Range(0, 0.3f)] [SerializeField] private float smoothing = 0.05f;
    [Range(0, 100f)] [SerializeField] private float snapback = 0.1f;
    [SerializeField] private Transform groundcheck;
    [SerializeField] private float balldist = 20f;
    // optional? [SerializeField] private Transform balldstcheck;
    [SerializeField] private LayerMask groundtype;
    [SerializeField] private GameObject ball = null;
    // mb similar thing for ball?

    private bool grounded;
    private bool carrying;
    private bool[] locked = { false, false, false, false }; //up down left right
    private Vector2 velocity = Vector2.zero;
    private bool facingR = true;
    private Rigidbody2D body;

    // Update is called once per frame

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }
    void FixedUpdate()
    {
        Vector3 distV;
        //bool wasGrounded = grounded;
        grounded = false;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundcheck.position, 0.5f, groundtype);
        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject != gameObject)
            {
                grounded = true;
            }
        }

        distV = groundcheck.position - ball.transform.position;
        Debug.Log(distV.magnitude);
        if (distV.magnitude >= balldist)
        {
            if (distV.x >= 0)// Right
            {
                locked[3] = true;
            }
            else
            {
                locked[3] = false;
            }
            if (distV.x <= 0) //no elif because it spares explicit handling of x=0
            {   // left
                locked[2] = true;
            } else
            {
                locked[2] = false;
            }

            if (distV.y >= 0)
            {
                locked[0] = true;
            } else
            {
                locked[0] = false;
            }
            if (distV.y <= 0) //no elif because it spares explicit handling of x=0
            {
                locked[1] = true;
            } else
            {
                locked[1] = false;
            }
            // z *should* always be zero
            if (distV.z != 0)
            {
                Debug.Log("WTF? distV.z should never be non 0");
            }
        } else
        {
            for (int i = 0; i < locked.Length; i++)
            {
                locked[i] = false;
            }
        }
        Vector2 v = body.velocity;

        body.velocity = v;
    }

    public void Move(float move, bool jump, bool carry)
    {
        bool carriable = false;

        if (carry)
        {
            if (!carrying)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(groundcheck.position, 0.2f);
                foreach (Collider2D collider in colliders)
                {
                    if (collider.gameObject == ball)
                    {
                        carriable = true;
                    }
                }
                if (carriable)
                {
                    carrying = true; //this needs to send an Event
                }
            }
            move *= carryspeed;
        } else
        {
            if (carrying)
            {
                carrying = false;
                //throw event?
            }
        }

        Vector2 targetV = new Vector2(move * 10f, body.velocity.y);
        Vector2 smoothedV = Vector2.SmoothDamp(body.velocity, targetV, ref velocity, smoothing);
        if (locked[0] && smoothedV.y > 0)
        {
            smoothedV.y = -snapback;
        } else if (locked[1] && smoothedV.y < 0) // HERE BE HILARIOUS BUGS
        {
            smoothedV.y = snapback;
        }

        if (locked[2] && smoothedV.x < 0) //these should be less hilarious
        {
            smoothedV.x = snapback;
        } else if (locked[3] && smoothedV.x > 0)
        {
            smoothedV.x = -snapback;
        }
        body.velocity = smoothedV;

        if (move > 0 && !facingR)
        {
            Flip();
        } else if (move < 0 && facingR) {
            Flip();
        }

        if (grounded && jump)
        {
            grounded = false;
            if (!locked[0])
            {
                body.AddForce(new Vector2(0f, jumpforce));
            }
        }
    }

    private void Flip()
    {
        facingR = !facingR;

        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}

