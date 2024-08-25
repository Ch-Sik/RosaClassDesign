using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class G_Cube : MonoBehaviour
{
    public bool onGrab = false;
    public bool canMove = true;

    public float mushJumpMoveSpeed = 15f;
    public float mushJumpPower = 15f;
    //float defaultMass;
    //float defaultGS;

    Rigidbody2D rb;
    public Collider2D boxCol;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //defaultMass = rb.mass;
    }

    public void Grab(Transform grabPosition)
    {
        onGrab = true;

        transform.position = new Vector3(transform.position.x,
                                         grabPosition.position.y + 0.515f);

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.simulated = false;
    }

    public void UnGrab()
    {
        onGrab = false;

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.simulated = true;
    }

    public void MushJump(int dir)
    {
        rb.velocity = new Vector2(mushJumpMoveSpeed * dir, mushJumpPower);
    }
}
