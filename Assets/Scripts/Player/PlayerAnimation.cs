using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AnyPortrait;
/// <summary>
/// 플레이어 애니메이션을 담당
/// </summary>
public class PlayerAnimation : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator anim;
    public PlayerController playerControl;
    public PlayerMovement playerMove;
    public PlayerRef playerRef;
    public apPortrait portrait;
    public float minAngle;
    public float maxAngle;

    private Vector3 _aimPosition;
    public enum AnimState {Idle, Walk, Attack, Jump, Climb, Fall }

    
    public AnimState state = AnimState.Idle;
    bool isFirstFrame = true;
    bool isAttack = false;

    Coroutine blinkCoroutine;
    float reactionBrightness = 0.75f;
    private void Start()
    {
        GetComponents();
    }

    void GetComponents()
    {
        playerRef = PlayerRef.Instance;
        rb = playerRef.rb;
        //anim = playerRef.anim;
        playerControl = playerRef.Controller;
        playerMove = playerRef.Move;
        portrait = GetComponentInChildren<apPortrait>();
    }

    private void FixedUpdate()
    {
        UpdateAnim();
        UpdateHeadDir2();
    }

    void UpdateAnim()
    {
        
        anim.SetBool("isWalking", (playerControl.currentMoveState == PlayerMoveState.GROUNDED && playerMove.moveVector.x != 0) ? true : false);
        anim.SetBool("isJumping", (playerControl.isMIDAIR && !playerMove.isDoingHooking) ? true : false);
        anim.SetBool("isClimbing", (playerControl.currentMoveState == PlayerMoveState.CLIMBING) ? true : false);
        anim.SetBool("isClimbEnd", (playerMove.isWallClimbingTop) ? true : false);
        anim.SetBool("isInputClimbX", (playerControl.currentMoveState == PlayerMoveState.CLIMBING && 
            (playerMove.facingDirection.toVector2().x == -playerMove.moveVector.x)) ? true : false);
        anim.SetBool("isMagicReady", (playerControl.currentActionState == PlayerActionState.MAGIC_READY) ? true : false);
        //anim.SetBool("isHooking", (playerMove.isDoingHooking) ? true : false);
        //anim.SetBool("isClimbOver", ((playerControl.currentMoveState == PlayerMoveState.CLIMBING && playerMove.isWallClimbingTop) ? true : false));
        anim.SetFloat("yVel", rb.velocity.y);
        
        //if (!playerMove.isWallJumping)q
        //{
        //    if (playerControl.currentMoveState == PlayerMoveState.CLIMBING)
        //    {
        //        if (playerMove.isWallClimbingTop)
        //        {
        //            if (playerMove.moveVector.y > 0)
        //            {
        //                Debug.Log("애니메이션 정재생");
        //                /*
        //                anim["ClimbAnimation"].speed = 1;
        //                if (!anim.IsPlaying("ClimbAnimation"))
        //                {
        //                    anim.Play("ClimbAnimation");
        //                }
        //                */
        //            }
        //            else if (playerMove.moveVector.y < 0)
        //            {
        //                Debug.Log("애니메이션 역재생");
        //                /*
        //                anim["ClimbAnimation"].time = anim["ClimbAnimation"].length;
        //                anim["ClimbAnimation"].speed = -1;
        //                if (!anim.IsPlaying("ClimbAnimation"))
        //                {
        //                    anim.Play("ClimbAnimation");
        //                }
        //                */
        //            }
        //            else
        //            {
        //                Debug.Log("애니메이션 정지");
        //                //anim.Stop("ClimbAnimation");
        //            }
        //        }
        //    }
        //}
        
    }

    void UpdateIdle()
    {
        if(isFirstFrame)
        {
            portrait.CrossFade("Idle");
            isFirstFrame = false;
        }

        if(playerControl.currentMoveState == PlayerMoveState.GROUNDED && playerMove.moveVector.x != 0)
        {
            state = AnimState.Walk;
            isFirstFrame = true;
        }
        else if(playerControl.currentMoveState == PlayerMoveState.MIDAIR && !playerMove.isDoingHooking)
        {
            state = AnimState.Jump;
            isFirstFrame = true;
        }
        else if(playerControl.currentMoveState == PlayerMoveState.CLIMBING)
        {
            state = AnimState.Climb;
            isFirstFrame = true;
        }
        else if(isAttack)
        {
            state = AnimState.Attack;
            isFirstFrame = true;
        }
        else if(playerControl.currentMoveState == PlayerMoveState.MIDAIR && rb.velocity.y < 0)
        {
            state = AnimState.Fall;
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
            portrait.CrossFade("Run");
            portrait.SetAnimationSpeed(2);
            isFirstFrame = false;
        }

        if (playerControl.currentMoveState == PlayerMoveState.MIDAIR && !playerMove.isDoingHooking)
        {
            state = AnimState.Jump;
            isFirstFrame = true;
        }
        else if (playerControl.currentMoveState == PlayerMoveState.CLIMBING)
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
            portrait.CrossFade("JumpBegin", 0.1f);
            isFirstFrame = false;
        }

        if(playerControl.currentMoveState == PlayerMoveState.MIDAIR && rb.velocity.y < 0)
        {
            state = AnimState.Fall;
            isFirstFrame = true;
        }
        else if (playerControl.currentMoveState == PlayerMoveState.GROUNDED && playerMove.moveVector.x != 0)
        {
            portrait.CrossFade("JumpEnd",0.1f);
            state = AnimState.Walk;
            isFirstFrame = true;
        }
        else if (playerControl.currentMoveState == PlayerMoveState.CLIMBING)
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
            portrait.CrossFade("JumpEnd", 0.1f);
            state = AnimState.Idle;
            isFirstFrame = true;
        }
    }

    void UpdateFall()
    {
        if(isFirstFrame)
        {
            portrait.CrossFade("Fall");
            isFirstFrame = false;
        }

        if (playerControl.currentMoveState == PlayerMoveState.GROUNDED && playerMove.moveVector.x != 0)
        {
            portrait.CrossFade("JumpEnd", 0.1f);
            state = AnimState.Walk;
            isFirstFrame = true;
        }
        else if (playerControl.currentMoveState == PlayerMoveState.CLIMBING)
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
            portrait.CrossFade("JumpEnd", 0.1f);
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

        if (playerControl.currentMoveState == PlayerMoveState.GROUNDED && playerMove.moveVector.x != 0)
        {
            state = AnimState.Walk;
            isFirstFrame = true;
        }
        else if (playerControl.currentMoveState == PlayerMoveState.MIDAIR && !playerMove.isDoingHooking)
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

    void UpdateHeadDir()
    {
        Vector2 mousePos_Screen = Input.mousePosition;
        if (Camera.main != null)
        {
            _aimPosition = Camera.main.ScreenToWorldPoint(mousePos_Screen);
            _aimPosition.z = 0;
            if (portrait != null)
            {
                Vector2 girlToMouse = (Vector2)_aimPosition - (Vector2)portrait.transform.position;
                float angle = Vector2.SignedAngle(Vector2.right, girlToMouse) ;
                if(gameObject.transform.localScale.x == -1) // 바라보는 방향에 따른 각도 보정
                {
                    angle *= -1;
                    if (angle < 0) angle += 180;
                    else angle -= 180;
                }
                Mathf.Clamp(angle,minAngle,maxAngle);
                portrait.SetBonePosition("IKHelper", _aimPosition, Space.World);
                Vector2 headVector2 = new Vector2(0, angle / 50);
                portrait.SetControlParamVector2("Head Direction", headVector2);
            }
            else
            {
                Debug.LogError("girl object is not initialized!");
            }
        }
        else
        {
            Debug.LogError("Camera.main is null!");
        }
    }

    Rect leftHalfScreen = new Rect(0, 0, 0.5f, 1f);
    Rect rightHalfScreen = new Rect(0.5f, 0, 0.5f, 1f);
    Rect wholeScreen = new Rect(0, 0, 1f, 1f);

    private Vector2 headDirection = new Vector2(0, 0);
    private Vector2 eyeDirection = new Vector2(0, 0);
    void UpdateHeadDir2()
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 viewportPosition = Camera.main.ScreenToViewportPoint(mousePosition);

        if(gameObject.transform.localScale.x == -1)
        {
            // Viewport 좌표를 (-1, -1)에서 (1, 1) 범위로 매핑
            headDirection = MapToRange(viewportPosition, rightHalfScreen.min, rightHalfScreen.max, new Vector2(-1, -1), new Vector2(1, 1));
            eyeDirection = MapToRange(viewportPosition, wholeScreen.min, wholeScreen.max, new Vector2(-1, -1), new Vector2(1, 1));
        }
        else
        {
            // Viewport 좌표를 (-1, -1)에서 (1, 1) 범위로 매핑
            headDirection = MapToRange(viewportPosition, leftHalfScreen.min, leftHalfScreen.max, new Vector2(1, -1), new Vector2(-1, 1));
            eyeDirection = MapToRange(viewportPosition, wholeScreen.min, wholeScreen.max, new Vector2(1, -1), new Vector2(-1, 1));
        }

        portrait.SetControlParamVector2("Head Direction", headDirection);
        portrait.SetControlParamVector2("Eye Direction", eyeDirection);
    }

    Vector2 MapToRange(Vector3 value, Vector3 fromMin, Vector3 fromMax, Vector2 toMin, Vector2 toMax)
    {
        Vector2 result;
        result.x = Mathf.Lerp(toMin.x, toMax.x, Mathf.InverseLerp(fromMin.x, fromMax.x, value.x));
        result.y = Mathf.Lerp(toMin.y, toMax.y, Mathf.InverseLerp(fromMin.y, fromMax.y, value.y));
        return result;
    }

    public void BlinkEffect()
    {
        if (portrait == null)
            return;         // 아직 애니메이션이 적용되지 않은 녀석들 예외 처리

        // 딱히 추가로 수정할 필요 없어보여서 수치들을 하드코딩했는데 필요하다면 필드값으로 빼낼 것
        const float timePerBlink = 0.2f;
        const int blinkCount = 8;
        Color blinkColor = Color.gray * reactionBrightness;
        blinkColor.a = 1;

        blinkCoroutine = StartCoroutine(DoBlink());

        IEnumerator DoBlink()
        {
            for (int i = 0; i < blinkCount; i++)
            {
                portrait.SetMeshColorAll(blinkColor);
                yield return new WaitForSeconds(timePerBlink / 2);
                portrait.ResetMeshMaterialToBatchAll();
                yield return new WaitForSeconds(timePerBlink / 2);
            }
            blinkCoroutine = null;
        }
    }

    public void SetTrigger(string name)
    {
        anim.SetTrigger(name);
        
    }

    public void ResetTrigger(string name)
    {
        anim.ResetTrigger(name);
    }
}
