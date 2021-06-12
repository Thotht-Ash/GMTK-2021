using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ballpull : MonoBehaviour
{
    [SerializeField] private GameObject man = null;
    [SerializeField] private Transform groundcheck;
    [SerializeField] private float balldist = 20f;
    [Range(0, 0.3f)] [SerializeField] private float smoothing = 0.05f;



    private Vector2 smoothingV = Vector2.zero;
    private Rigidbody2D body;


    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        body.velocity = Vector2.SmoothDamp(body.velocity, smoothingV, ref smoothingV, smoothing);
    }

    public void MoveBall (Vector2 inv)
    {
        body.velocity = inv;
    }
}
