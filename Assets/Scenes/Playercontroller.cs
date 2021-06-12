using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable] public class ObjectEvent : UnityEvent <object> { }


public class Playercontroller : MonoBehaviour
{
    [SerializeField] private float jumpforce = 400f;
    [Range(0, 1)] [SerializeField] private float carryspeed = 0.3f;
    [Range(0, 0.3f)] [SerializeField] private float smoothing = 0.05f;
    [Range(0, 100f)] [SerializeField] private float snapback = 0.1f;
    [SerializeField] private Transform groundcheck;
    [SerializeField] private float balldist = 20f;
    [SerializeField] private LayerMask groundtype;
    [SerializeField] private GameObject ball = null;
	//[SerializeField] private Transform ballAngle;
	[SerializeField] private float ballSpeed = 100f;


    private bool grounded;
    private bool carrying;
    private Vector2 velocity = Vector2.zero;
    private bool facingR = true;
    private Rigidbody2D body;
    private float distV;
	private Vector2 ballDirection;
	private float ballAngle;


    public ObjectEvent BallPullEvent;
    // Update is called once per frame


    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        if (BallPullEvent == null)
        {
            BallPullEvent = new ObjectEvent();
        }
    }
    void FixedUpdate()
    {
        //bool wasGrounded = grounded;
        grounded = false;
		if(carrying) ball.transform.position = body.transform.position;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundcheck.position, 1.5f, groundtype);
        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject != gameObject)
            {
                grounded = true;
            }
        }

        distV = (groundcheck.position - ball.transform.position).magnitude;
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
                Collider2D[] colliders = Physics2D.OverlapCircleAll(groundcheck.position, 0.5f);
                foreach (Collider2D collider in colliders)
                {
                    if (collider.gameObject == ball)
                    {
                        carriable = true;
						ball.transform.position = body.transform.position;
                    }
                }
                if (carriable)
                {
                    carrying = true; //this needs to send an Event
					ball.transform.position = body.transform.position;
                }
				Debug.Log(carriable);
            
            move *= carryspeed;
			} 
			else
			{
				if (carrying)
				{
					carrying = false;
				}
			}
		}
		

        Vector2 targetV;
        if ((distV >= balldist) || carrying)
        {
            targetV = new Vector2(move * carryspeed * 10f, body.velocity.y);
        }
        else
        {
            targetV = new Vector2(move * 10f, body.velocity.y);
        }
        body.velocity = Vector2.SmoothDamp(body.velocity, targetV, ref velocity, smoothing);


        if (move > 0 && !facingR)
        {
            Flip();
        } else if (move < 0 && facingR) {
            Flip();
        }

        if (grounded && jump)
        {
            grounded = false;
			//Debug.Log((1-(distV-balldist)));
            if (!carrying) body.AddForce(new Vector2(0f, (1-(distV/balldist))*jumpforce));
			else 
			{	
				body.AddForce(new Vector2(0f, jumpforce*0.2f));
		
				ballDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - ball.transform.position;
				ballAngle = Mathf.Atan2(ballDirection.y, ballDirection.x) * Mathf.Rad2Deg;
				ball.transform.rotation = Quaternion.Euler(0f, 0f, ballAngle - 90f);
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