using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movement : MonoBehaviour
{
    // Start is called before the first frame update
    public Playercontroller controller;

    public float spd = 40f;


    float horizontal = 0f;
    bool jmp;
    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal") * spd;
         if (Input.GetButton("Jump"))
        {
            jmp = true;
        }
    }

    private void FixedUpdate()
    {
        controller.Move(horizontal * Time.fixedDeltaTime, jmp, false);
        jmp = false;
    }
}
