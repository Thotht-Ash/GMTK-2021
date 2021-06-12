using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movement : MonoBehaviour
{
    // Start is called before the first frame update
    public Playercontroller controller;

    [SerializeField] public float spd = 20f;


    float horizontal = 0f;
    bool jmp;
	bool carry;
    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal") * spd;
         if (Input.GetButton("Jump"))
        {
            jmp = true;
        }
		if (Input.GetButtonDown("Fire1"))
		{
			carry = true;
		}
    }

    private void FixedUpdate()
    {
        controller.Move(horizontal * Time.fixedDeltaTime, jmp, carry);
        jmp = false;
		carry = false;
    }
}
