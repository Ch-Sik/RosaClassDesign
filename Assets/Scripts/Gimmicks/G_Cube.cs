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

    public Vector2 startPos;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //defaultMass = rb.mass;

        startPos = transform.position;
    }

    public void ResetPosition()
    {
        transform.position = startPos;
    }

    public void Grab(Transform grabPosition)
    {
        onGrab = true;

        transform.position = new Vector3(transform.position.x,
                                         grabPosition.position.y + 0.515f);

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.simulated = false;
    }

    public void UnGrab(bool isReset = false)
    {
        onGrab = false;

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.simulated = true;

        if (isReset)
            ResetPosition();
    }

    public void MushJump(int dir)
    {
        rb.velocity = new Vector2(mushJumpMoveSpeed * dir, mushJumpPower);
    }
}
