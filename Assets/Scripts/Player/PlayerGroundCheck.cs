using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundCheck : MonoBehaviour
{
    
    [ReadOnly, SerializeField] PlayerRef playerRef;
    PlayerMovement playerControl;
    // Start is called before the first frame update
    void Start()
    {
        GetComponents();
    }

    void GetComponents()
    {
        playerRef = PlayerRef.Instance;
        playerControl = playerRef.Move;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        playerControl?.SetIsGrounded(collision.gameObject);
    }

    //private void OnTriggerStay2D(Collider2D collision)
    //{
    //    playerControl?.SetIsGrounded(collision.gameObject);
    //}

    private void OnTriggerExit2D(Collider2D collision)
    {
        playerControl?.SetIsNotGrounded();
    }
}
