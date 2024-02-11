using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundCheck : MonoBehaviour
{
    
    [ReadOnly, SerializeField] PlayerRef playerRef;
    PlayerMovement playerMove;
    
    // Start is called before the first frame update
    void Start()
    {
        GetComponents();
    }

    void GetComponents()
    {
        playerRef = PlayerRef.Instance;
        playerMove = playerRef.Move;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        playerMove?.SetIsGrounded(collision.gameObject);
        
    }

    //private void OnTriggerStay2D(Collider2D collision)
    //{
    //    playerControl?.SetIsGrounded(collision.gameObject);
    //}

    private void OnTriggerExit2D(Collider2D collision)
    {
        Transform parentTransform = playerMove?.platformBelow?.transform.parent;
        if (parentTransform != null)
        {
            BreakablePlatform component = parentTransform.GetComponent<BreakablePlatform>();

            if (component != null)
            {
                component.ColBreak();
            }
        }
        playerMove?.SetIsNotGrounded();
    }
}
