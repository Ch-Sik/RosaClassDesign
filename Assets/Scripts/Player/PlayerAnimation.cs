using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AnyPortrait;
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
    apPortrait portrait;

    enum AnimState {Idle, Walk, Attack, Jump, Climb }

    AnimState state = AnimState.Idle;
    bool isFirstFrame = true;
    bool isAttack = false;

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
        anim.SetBool("isJumping", (playerControl.MoveState == PlayerMoveState.MIDAIR && !playerMove.isDoingHooking) ? true : false);
        anim.SetBool("isClimbing", (playerControl.MoveState == PlayerMoveState.CLIMBING) ? true : false);
        anim.SetBool("isHooking", (playerMove.isDoingHooking) ? true : false);
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

    void UpdateIdle()
    {
        if(isFirstFrame)
        {
            portrait.CrossFade("Idle");
            isFirstFrame = false;
        }

        if(playerControl.MoveState == PlayerMoveState.GROUNDED && playerMove.moveVector.x != 0)
        {
            state = AnimState.Walk;
            isFirstFrame = true;
        }
        else if(playerControl.MoveState == PlayerMoveState.MIDAIR && !playerMove.isDoingHooking)
        {
            state = AnimState.Jump;
            isFirstFrame = true;
        }
        else if(playerControl.MoveState == PlayerMoveState.CLIMBING)
        {
            state = AnimState.Climb;
            isFirstFrame = true;
        }
        else if(isAttack)
        {
            state = AnimState.Attack;
            isFirstFrame = true;
        }
    }

    void UpdateAttack()
    {
        if (isFirstFrame)
        {
            portrait.CrossFade("Attack", 0.1f);
            isFirstFrame = false;
        }
    }

    void UpdateWalk()
    {
        if (isFirstFrame)
        {
            portrait.CrossFade("Walk");
            isFirstFrame = false;
        }

        if (playerControl.MoveState == PlayerMoveState.MIDAIR && !playerMove.isDoingHooking)
        {
            state = AnimState.Jump;
            isFirstFrame = true;
        }
        else if (playerControl.MoveState == PlayerMoveState.CLIMBING)
        {
            state = AnimState.Climb;
            isFirstFrame = true;
        }
        else if (isAttack)
        {
            state = AnimState.Attack;
            isFirstFrame = true;
        }
        else
        {
            state = AnimState.Idle;
            isFirstFrame = true;
        }
    }

    void UpdateJump()
    {
        if (isFirstFrame)
        {
            portrait.CrossFade("JumpBegin");
            isFirstFrame = false;
        }

        if (playerControl.MoveState == PlayerMoveState.GROUNDED && playerMove.moveVector.x != 0)
        {
            state = AnimState.Walk;
            isFirstFrame = true;
        }
        else if (playerControl.MoveState == PlayerMoveState.CLIMBING)
        {
            state = AnimState.Climb;
            isFirstFrame = true;
        }
        else if (isAttack)
        {
            state = AnimState.Attack;
            isFirstFrame = true;
        }
        else 
        { 
            state = AnimState.Idle;
            isFirstFrame = true;
        }
    }

    void UpdateClimb()
    {
        if (isFirstFrame)
        {
            portrait.CrossFade("Climb");
            isFirstFrame = false;
        }

        if (playerControl.MoveState == PlayerMoveState.GROUNDED && playerMove.moveVector.x != 0)
        {
            state = AnimState.Walk;
            isFirstFrame = true;
        }
        else if (playerControl.MoveState == PlayerMoveState.MIDAIR && !playerMove.isDoingHooking)
        {
            state = AnimState.Jump;
            isFirstFrame = true;
        }
        else if (isAttack)
        {
            state = AnimState.Attack;
            isFirstFrame = true;
        }
        else
        { 
            state = AnimState.Idle;
            isFirstFrame = true;
        }

    }


    public void SetTrigger(string name)
    {
        anim.SetTrigger(name);
    }
}
