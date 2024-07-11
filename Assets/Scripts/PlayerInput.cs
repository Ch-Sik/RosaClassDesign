using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public float speed;
    public float currentVelocity;

    public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        currentVelocity = Input.GetAxis("Horizontal") * speed;
        if (Input.GetKey(KeyCode.LeftShift))
            currentVelocity *= 3;
        anim.SetFloat("velocityAbs", Mathf.Abs(currentVelocity));

        if (Input.GetKeyDown(KeyCode.F))
        {
            anim.SetTrigger("Attack");
        }
    }
}
