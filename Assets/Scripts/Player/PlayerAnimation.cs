using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어 애니메이션을 담당
/// </summary>
public class PlayerAnimation : MonoBehaviour
{
    Rigidbody2D rb;
    Animator anim;
    PlayerController playerControl;
    PlayerMovement playerMove;
    PlayerRef playerRef;



    private void Awake()
    {
        GetComponents();
    }

    void GetComponents()
    {
        playerRef = PlayerRef.Instance;
        rb = playerRef.rb;
        anim = playerRef.anim;
        playerControl = playerRef.Controller;
        playerMove = playerRef.Move;
    }

    private void FixedUpdate()
    {
        UpdateAnim();
    }

    void UpdateAnim()
    {

        anim.SetBool("isWalking", (playerControl.MoveState == PlayerMoveState.GROUNDED && playerMove.moveVector.x != 0) ? true : false);
        anim.SetBool("isJumping", (playerControl.MoveState == PlayerMoveState.MIDAIR) ? true : false);
        anim.SetBool("isClimbing", (playerControl.MoveState == PlayerMoveState.CLIMBING) ? true : false);
        anim.SetBool("isClimbOver", ((playerControl.MoveState == PlayerMoveState.CLIMBING && playerMove.isWallClimbingTop) ? true : false));
        anim.SetFloat("yVel", rb.velocity.y);
        if (!playerMove.isWallJumping)
        {
            if (playerControl.MoveState == PlayerMoveState.CLIMBING)
            {
                if (playerMove.isWallClimbingTop)
                {
                    if (playerMove.moveVector.y > 0)
                    {
                        Debug.Log("애니메이션 정재생");
                        /*
                        anim["ClimbAnimation"].speed = 1;
                        if (!anim.IsPlaying("ClimbAnimation"))
                        {
                            anim.Play("ClimbAnimation");
                        }
                        */
                    }
                    else if (playerMove.moveVector.y < 0)
                    {
                        Debug.Log("애니메이션 역재생");
                        /*
                        anim["ClimbAnimation"].time = anim["ClimbAnimation"].length;
                        anim["ClimbAnimation"].speed = -1;
                        if (!anim.IsPlaying("ClimbAnimation"))
                        {
                            anim.Play("ClimbAnimation");
                        }
                        */
                    }
                    else
                    {
                        Debug.Log("애니메이션 정지");
                        //anim.Stop("ClimbAnimation");
                    }
                }
            }
        }
    }

    public void SetTrigger(string name)
    {
        anim.SetTrigger(name);
    }
}
