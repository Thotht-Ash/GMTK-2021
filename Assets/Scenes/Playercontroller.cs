using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ObjectEvent : UnityEvent <object> { }

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
	[SerializeField] private float angleSpeed = 1f;
	[SerializeField] private float ballSpeed = 100f;

    private bool grounded;
    private bool carrying;
    private Vector2 velocity = Vector2.zero;
    private bool facingR = true;
    private Rigidbody2D body;
    private float distV;
	private float angle;
	private float polarity = 1;
	private bool rotationStart = true;


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

        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundcheck.position, 0.5f, groundtype);
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

    public void Move(float move, bool jump, bool carry, bool toss)
    {
        bool carriable = false;
        
		if (carry)
        {
            if (!carrying)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(groundcheck.position, 0.1f);
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
			} else
			{
				if (carrying)
				{
					carrying = false;
					//throw event?
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
			else body.AddForce(new Vector2(0f, jumpforce*0.2f));
        }
    }

    private void Flip()
    {
        facingR = !facingR;

        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
	
	public void tosser(bool hold, bool release)
	{
		if (carrying && hold)
		{
			if(rotationStart && facingR)
			{
				ball.transform.rotation.z = 225;
				polarity = 1;
			}
			else if(rotationStart && !facingR)
			{
				ball.transform.rotation.z = 135;
				polarity = -1;
			}
			//Spawn aiming reticule, if it doesnt exist
			ball.transform.rotation.z += angleSpeed * polarity;
		}
		else if (carrying && release)
		{
			throw the ball, delete reticule, carrying to false
		}
	}
}