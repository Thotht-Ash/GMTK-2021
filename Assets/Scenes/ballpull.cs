using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Math;


public class ballpull : MonoBehaviour
{
    [SerializeField] private GameObject man = null;
    [SerializeField] private Rigidbody2D manbody = null;
    [SerializeField] private Transform groundcheck;
    [SerializeField] private float balldist = 20f;
    [Range(0, 0.3f)] [SerializeField] private float smoothing = 0.05f;

    private Vector2 smoothingV = Vector2.zero;
    private Vector2 wasteV = Vector2.zero;
    private Rigidbody2D body;


    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 distV = man.transform.position - body.transform.position;
        if (distV.magnitude >= balldist && System.Math.Sign(manbody.velocity.x) == System.Math.Sign(distV.x)) // DO NOT QUESTION ME AGAIN MACHINE
        {
            body.velocity = manbody.velocity;
        }
        else
        {
            body.velocity = Vector2.SmoothDamp(body.velocity, smoothingV, ref wasteV, smoothing);
        }
    }

}
