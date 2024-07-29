using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


/// <summary>
/// 플레이어의 움직임 담당.
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    // 이동 관련 파라미터
    [FoldoutGroup("좌우 이동 관련")]
    [Tooltip("플레이어 이동 속도")]
    [SerializeField] float moveSpeed = 1f;

    [FoldoutGroup("좌우 이동 관련")]
    [Tooltip("시작 시 플레이어 스프라이트의 방향")]
    [SerializeField] LR spriteDirection = LR.RIGHT;

    [FoldoutGroup("좌우 이동 관련")]
    [Tooltip("공격 중에 좌우 이동 불가능하게 설정")]
    public bool noMoveOnAttack;

    // 점프 관련 파라미터
    [FoldoutGroup("점프 관련")]
    [Tooltip("플레이어 점프 파워")]
    [SerializeField] float jumpPower = 10f;

    [FoldoutGroup("점프 관련")]
    [Tooltip("플레이어 하향 점프 파워")]
    [SerializeField] float jumpDownPower = 1f;

    [FoldoutGroup("점프 관련")]
    [Tooltip("최소 상승 시간")]
    [SerializeField] float minJumpUpDuration = 0.1f;

    [FoldoutGroup("점프 관련")]
    [Tooltip("하향 점프 플렛폼 적용 시간")]
    [SerializeField] float downJumpPlatformDuration = 0.1f;

    // 벽이동 관련 파라미터
    [FoldoutGroup("벽이동 관련")]
    [Tooltip("벽 기어오르기 활성화")]
    [SerializeField] bool wallClimbEnabled;

    [FoldoutGroup("벽이동 관련")]
    [Tooltip("기어오르기 속도")]
    [SerializeField] float climbUpSpeed;

    [FoldoutGroup("벽이동 관련")]
    [Tooltip("기어내려가기 속도")]
    [SerializeField] float climbDownSpeed;

    [FoldoutGroup("벽이동 관련")]
    [Tooltip("벽 점프 활성화")]
    [SerializeField] bool wallJumpEnabled;

    [FoldoutGroup("벽이동 관련")]
    [Tooltip("벽 점프 시 속도")]
    [SerializeField] Vector2 wallJumpPower;

    [FoldoutGroup("벽이동 관련")]
    [Tooltip("벽 점프 최소시간")]
    [SerializeField] float minWallJumpDuration;

    [FoldoutGroup("벽이동 관련")]
    [Tooltip("반대를 보고 벽에 메달리는 시간")]
    [SerializeField] float maxClimbTime;

    [FoldoutGroup("벽이동 관련")]
    [Tooltip("벽오르기 root motion 대체")]
    public Vector3 ClimbEndOffset = new Vector3(0.85f, 0.744f);

    // 슈퍼대시 관련
    [FoldoutGroup("슈퍼대쉬(오이) 관련")]
    [Tooltip("오이대쉬 속도")]
    [SerializeField] float superDashSpeed = 3f;

    // 넉백 관련
    [FoldoutGroup("넉백 관련")]
    [Tooltip("넉백 계수")]
    [SerializeField] float knockbackStrength = 1f;

    [FoldoutGroup("큐브 관련")]
    [SerializeField] float grabSpeedCoef = 0.5f;

    // 필드값 중 일부는 디버그용으로 Inspector에 노출
    [FoldoutGroup("Debug")]
    [VerticalGroup("Debug/Vertical")]
    [BoxGroup("Debug/Vertical/Component")]
    [ReadOnly, SerializeField] Rigidbody2D rb;

    [BoxGroup("Debug/Vertical/Component")]
    [ReadOnly, SerializeField] BoxCollider2D col;

    [BoxGroup("Debug/Vertical/Component")]
    [ReadOnly, SerializeField] PlayerRef playerRef;

    [BoxGroup("Debug/Vertical/Component")]
    [ReadOnly, SerializeField] PlayerController playerControl;

    [BoxGroup("Debug/Vertical/General")]
    [ReadOnly, SerializeField] public GameObject platformBelow = null;

    [BoxGroup("Debug/Vertical/General")]
    [ReadOnly, SerializeField] public Vector2 moveVector;

    [BoxGroup("Debug/Vertical/General")]
    [ReadOnly, SerializeField] public LR facingDirection;                 // 플레이어 바라보는 방향

    // 플래그
    [FoldoutGroup("플래그")]
    [ReadOnly] public bool isGrounded = false;
    [FoldoutGroup("플래그")]
    [ReadOnly] public bool isJumpingUp = false;
    [FoldoutGroup("플래그")]
    [ReadOnly] public bool isFacingWall = false;
    [FoldoutGroup("플래그")]
    [ReadOnly] public bool isWallClimbing = false;      // 벽에 붙어서 이동중
    [FoldoutGroup("플래그")]
    [ReadOnly] public bool isWallJumpReady = false;      // 벽에 붙어서 이동중
    [FoldoutGroup("플래그")]
    [ReadOnly] public bool isWallClimbingTop = false;      // 벽에 붙어서 이동중 정상도달
    [FoldoutGroup("플래그")]
    [ReadOnly] public bool isWallJumping = false;       // 벽 점프 중인지
    [FoldoutGroup("플래그")]
    [ReadOnly] public bool isDoingMagic = false;
    [FoldoutGroup("플래그")]
    [ReadOnly] public bool isKnockbacked = false;
    [FoldoutGroup("플래그")]
    [ReadOnly] public bool isSlidingOnWall = false;
    [FoldoutGroup("플래그")]
    [ReadOnly] public bool isDoingHooking = false;      // 후크액션을 수행하고 있는지
    [FoldoutGroup("플래그")]
    [ReadOnly] public bool isHitHookingTarget = false;  // 후크액션중 후크목표에 도달했는지
    [FoldoutGroup("플래그")]
    [ReadOnly] public bool isDoingAttack = false;
    [FoldoutGroup("플래그")]
    [ReadOnly] public bool isDoingSuperDash = false;
    [FoldoutGroup("플래그")]
    [ReadOnly] public bool isNotMoveable = false;   // 종합적으로 고려하여 플레이어가 움직일 수 있는지 없는지
    [FoldoutGroup("플래그")]
    [ReadOnly] public bool isGrabCube = false;          //큐브를 잡고 있는지

    // 범위 지정
    [FoldoutGroup("벽 감지 범위")]
    public Vector2 detectWallTop = new Vector2(0.0f, 0.5f);
    [FoldoutGroup("벽 감지 범위")]
    public Vector2 detectWallBot = new Vector2(0.6f, -0.7f);
    [FoldoutGroup("벽 감지 범위")]
    public Vector2 detectWallEndTop = new Vector2(0.0f, 0.5f);
    [FoldoutGroup("벽 감지 범위")]
    public Vector2 detectWallEndBot = new Vector2(0.6f, 0.0f);

    // 타이머 (non-serializable)
    private Timer jumpTimer;                 // 최소 점프 시간을 위한 타이머
    private Timer jumpBufferTimer;           // 점프 선입력 타이머
    private Timer climbTimer;                // 반대방향 입력후 벽에 메달림 유지 타이머

    // 상수
    LayerMask groundLayer;      // NameToLayer가 constructor에서 호출 불가능하여 InitFields에서 초기화
    LayerMask climbableLayer;
    float gravityScale = 2.8f;

    #region 초기화
    private void Start()
    {
        GetComponents();
        InitFields();
    }

    
    void GetComponents()
    {
        playerRef = PlayerRef.Instance;
        rb = playerRef.rb;
        col = playerRef.col;
        playerControl = playerRef.Controller;
    }

    void InitFields()
    {
        //aimLine = GameObject.Find("AimLine");
        facingDirection = spriteDirection;
        groundLayer = LayerMask.NameToLayer("Ground");
        climbableLayer = LayerMask.NameToLayer("Climbable");
    }
    #endregion

    #region 업데이트

    // =====
    // Update and set flags
    // =====

    void FixedUpdate()
    {
        UpdateFlags();
        UpdateMovement();
    }

    void UpdateFlags()
    {
        isFacingWall = DetectWall();
        if (isFacingWall) 
        { 
            if(playerControl.currentMoveState == PlayerMoveState.CLIMBING)
            {
                isWallClimbingTop = CheckWallEnd();
            }
            else
            {
                isWallClimbingTop = false;
            }
            
            isSlidingOnWall = false;
            if(isWallClimbingTop)
            {
                rb.velocity = Vector2.zero;
            }
        }
        else
        {
            if (playerControl.currentMoveState == PlayerMoveState.CLIMBING && !isWallJumpReady)
            {
                UnstickFromWall();
            }
            isWallClimbingTop = false;
        }
        isDoingAttack = playerRef.combat.isDoingAttack;
        isNotMoveable = isWallClimbingTop || isKnockbacked || isWallJumping;
    }

    /// <summary>
    /// Walk와 Climb 등에서 설정한 moveVector를 실제로 플레이어 움직임에 반영
    /// </summary>
    void UpdateMovement()
    {
        if (isNotMoveable) return;

        // CLIMBING 상태면 벽오르기
        if (playerControl.currentMoveState == PlayerMoveState.CLIMBING)
        {
            if (playerControl.currentMoveState == PlayerMoveState.CLIMBING)
            {
                // 기어 올라가는 속도와 내려오는 속도를 구분
                rb.velocity = new Vector2(0,
                    moveVector.y * (moveVector.y > 0 ? climbUpSpeed : climbDownSpeed));
                if (isFacingWall == false)
                {
                    // TODO: 이 부분을 벽 위 지면에 올라가는 것으로 대체
                    //UnstickFromWall();
                }
            }
        }
        // DEFAULT 상태라면 좌우 이동
        else
        {
            // 공격 중에는 이동 불가
            if(noMoveOnAttack && isDoingAttack)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
            else
            {
                //큐브를 옮기는 동안은 이동속도를 조정
                float xVelocity = moveVector.x * moveSpeed;
                if (isGrabCube)
                {
                    xVelocity *= grabSpeedCoef;
                    //속도에 따라 값의 조정
                    if (xVelocity == 0)
                        playerControl.currentCubeActionState = PlayerCubeActionState.GRAB;
                    else
                    {
                        if (IsFacingDirection(xVelocity))
                            playerControl.currentCubeActionState = PlayerCubeActionState.PUSH;
                        else
                            playerControl.currentCubeActionState = PlayerCubeActionState.PULL;
                    }
                }
                else
                    playerControl.currentCubeActionState = PlayerCubeActionState.DEFAULT;
                rb.velocity = new Vector2(xVelocity, rb.velocity.y);
                LookAt2DLocal(moveVector);
            }
        }
    }
    #endregion

    #region 걷기 관련

    /// <summary>
    /// 지상과 공중에서 x축 방향으로의 이동. 
    /// 여기서는 조작 입력값만 받고 실제 속도 업데이트는 UpdateMovement에서 이루어짐.
    /// </summary>
    /// <param name="inputVector">x,y 모두 -1~+1 사이의 값인 입력 벡터</param>
    internal void Walk(Vector2 inputVector)
    {
        moveVector = new Vector2(inputVector.x, 0);
    }

    #endregion

    #region 점프 관련
    internal void OnJump(bool pressedDown)
    {
        //큐브를 옮기는 중엔 점프 불가능
        if (isGrabCube)
            return;

        if (!isGrounded)
        {
            ReserveJump();       // 공중에서는 점프 선입력
        }
        else
        {
            if (pressedDown && (platformBelow?.CompareTag("Platform") == true))
            {
                Debug.Log(platformBelow?.tag);
                JumpDown();      // 하향 점프
            }
            else
            {
                isJumpingUp = true;
                JumpUp();        // 상향 점프
            }
        }
    }

    internal void JumpUp()
    {
        if(isNotMoveable) return;

        //playerAnim.SetTrigger("JumpTrigger");
        rb.velocity = new Vector2(rb.velocity.x, jumpPower);
        jumpTimer = Timer.StartTimer();
    }

    /// <summary>
    /// 숏점프와 롱점프 구분을 위해 조기 점프종료하는 함수
    /// </summary>
    internal void FinishJumpUp()
    {
        // early return
        if (isNotMoveable) return;
        if (!isJumpingUp) return;

        isJumpingUp = false;
        // 최소 점프 시간에 도달했는지 체크
        if (jumpTimer.duration > minJumpUpDuration) 
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y < 0 ? rb.velocity.y : 0);
        }
        else
            StartCoroutine(ReserveFinishJumpUp());

        IEnumerator ReserveFinishJumpUp()
        {
            // Debug.Log("Too short jump duration");
            yield return new WaitForSeconds(minJumpUpDuration - jumpTimer.duration);
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y < 0 ? rb.velocity.y : 0);
        }
    }

    /// <summary>
    /// 하향 점프. 콜라이더를 트리거로 변환하여 아래의 플랫폼을 통과할 수 있게 함.
    /// </summary>
    internal void JumpDown()
    {
        StartCoroutine(ApplyDownJumpCollider());
        rb.velocity = new Vector2(rb.velocity.x, jumpDownPower);

        IEnumerator ApplyDownJumpCollider()
        {
            PlatformEffector2D platEffector = platformBelow.GetComponent<PlatformEffector2D>();
            platEffector.rotationalOffset = 180f;
            yield return new WaitForSeconds(downJumpPlatformDuration);
            platEffector.rotationalOffset = 0;
        }
    }

    /// <summary>
    /// OnTriggerExit2D에 의해 호출됨. 플랫폼을 완전히 통과하면 트리거 해제
    /// </summary>
    void FinishJumpDown()
    {
        col.isTrigger = false;      // 지형 통과 불가능
    }

    /// <summary>
    /// 점프 선입력 받기. 선입력 받은 점프의 처리는 OnLanded에서 수행함.
    /// </summary>
    internal void ReserveJump()
    {
        jumpBufferTimer = Timer.StartTimer();
    }

    /// <summary>
    /// 벽에서의 점프. 벽점프는 소점프/대점프를 구분하지 않음.
    /// </summary>
    internal void WallJump()
    {
        if (isNotMoveable) return;

        // 플래그 설정
        isWallJumping = true;
        isWallJumpReady = false;

        // 벽으로부터 떨어지고 상태 바꾸기
        UnstickFromWall();
        playerControl.ChangeMoveState(PlayerMoveState.DEFAULT);

        if (!isSlidingOnWall)
        {
            float xDirection = facingDirection.isRIGHT() ? -1 : 1;  // 보고 있는 방향의 반대방향으로 점프
            rb.velocity = new Vector2(wallJumpPower.x * xDirection, wallJumpPower.y);
            Flip();
        }
        else
        {
            float xDirection = facingDirection.isRIGHT() ? 1 : -1;  // 보고 있는 방향으로 점프
            rb.velocity = new Vector2(wallJumpPower.x * xDirection, wallJumpPower.y);
        }


        StartCoroutine(ReserveFinishWallJump());

        IEnumerator ReserveFinishWallJump()
        {
            // minWallJumpDuration 동안 wallJumping 플래그를 통해 x축 방향 입력을 무시함. (Walk 함수 참고)
            yield return new WaitForSeconds(minWallJumpDuration);
            isWallJumping = false;
        }
    }
    #endregion

    #region 벽 오르기 관련

    /// <summary>
    /// 벽면에서의 y축 방향으로의 이동.
    /// 여기서는 조작 입력값만 받고 실제 속도 업데이트는 UpdateMovement에서 이루어짐.
    /// </summary>
    /// <param name="inputVector"></param>
    internal void Climb(Vector2 inputVector)
    {
        //큐브를 옮기는 중엔 벽타기 불가능
        if (isGrabCube)
            return;

        Debug.Log("Climbing");

        moveVector = inputVector;       // 벽 점프 등을 위해 x축 방향 필터링하지 않음.
        if (moveVector.x != 0)
        {
            if (!DetectWall())
            {
                Debug.Log("start Timer");
                climbTimer = Timer.StartTimer();
                isWallJumpReady = true;
            }
        }
        else
        {
            if (!DetectWall())
            {
                // TODO: 지면으로 올라가기 구현
                //Debug.Log("AAA");
                //UnstickFromWall();
            }
        }


    }

    internal void StopClimb(Vector2 inputVector)
    {
        if (isNotMoveable) return;
        if (moveVector.x != 0 && !isWallClimbingTop)
        {
            if (!DetectWall() && climbTimer != null)
            {
                Debug.Log("TimerCheck: " + climbTimer.duration);
                if (climbTimer.duration > maxClimbTime)
                {
                    UnstickFromWall();
                    Flip();
                    isSlidingOnWall = true;
                    isWallJumpReady = false;
                }
            }
        }
        moveVector = inputVector;
        isWallJumpReady = false;
    }

    /// <summary>
    /// 벽 오르기 상태로 전환
    /// </summary>
    internal void StickToWall()
    {
        //큐브를 옮기는 중엔 벽에 붙기 불가능
        if (isGrabCube)
            return;

        isWallClimbing = true;
        playerControl.ChangeMoveState(PlayerMoveState.CLIMBING);
        rb.gravityScale = 0;
    }

    /// <summary>
    /// 벽 오르기 해제.
    /// </summary>
    internal void UnstickFromWall()
    {
        isWallClimbing = false;
        playerControl.ChangeMoveState(PlayerMoveState.DEFAULT);
        rb.gravityScale = this.gravityScale;
        
    }

    public void ClimbEnd()
    {
        Debug.Log("ClimbEnd");
        
        if(facingDirection == LR.LEFT)
        {
            gameObject.transform.position += new Vector3(-ClimbEndOffset.x, ClimbEndOffset.y);
        }
        else
        {
            gameObject.transform.position += new Vector3(ClimbEndOffset.x, ClimbEndOffset.y);
        }

        isWallClimbingTop = false;
        playerControl.ChangeMoveState(PlayerMoveState.DEFAULT);
        rb.gravityScale = this.gravityScale;
    }

    /// <summary>
    /// 하향 점프 시 플랫폼을 온전히 빠져나오면 FinishJumpDown을 호출
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer.Equals(groundLayer))
        {
            FinishJumpDown();
        }
    }

    /// <summary>
    /// 덩굴과 충돌하였을 때 벽오르기 상태로 전환
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDoingSuperDash)
        {
            isDoingSuperDash = false;
            CancelSuperDashAfterLaunch();
        }
        else
        {
            // 벽에 설치된 덩굴에 충돌하였을 경우
            if (collision.gameObject.layer.Equals(climbableLayer))
            {
                if (isFacingWall && !isSlidingOnWall)
                {

                    StickToWall();
                    //playerAnim.SetTrigger("ClimbTrigger");
                }
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // 벽에 설치된 덩굴에 충돌하였을 경우
        if (collision.gameObject.layer.Equals(climbableLayer))
        {
            if (isFacingWall && !isSlidingOnWall)
                StickToWall();
        }
    }

    bool DetectWall()
    {
        if ((moveVector.x == -1 && facingDirection == LR.RIGHT) || (moveVector.x == 1 && facingDirection == LR.LEFT)) return false;
        Vector2 pointTop = (Vector2)transform.position
                            + Vector2.Scale(detectWallTop, new Vector2(facingDirection.isRIGHT() ? 1 : -1, 1));
        Vector2 pointBot = (Vector2)transform.position
                            + Vector2.Scale(detectWallBot, new Vector2(facingDirection.isRIGHT() ? 1 : -1, 1));
        Collider2D[] hitWall = Physics2D.OverlapAreaAll(pointTop, pointBot);
        // OverlapArea가 clibableLayer로 지정하였을 때 항상 null을 리턴하는 문제가 있어 아래와 같이 구현함.
        foreach (var i in hitWall)
        {
            if (i.gameObject.layer == climbableLayer)
                return true;
        }
        return false;
    }

    bool CheckWallEnd()
    {
        Vector2 pointTop = (Vector2)transform.position
                            + Vector2.Scale(detectWallEndTop, new Vector2(facingDirection.isRIGHT() ? 1 : -1, 1));
        Vector2 pointBot = (Vector2)transform.position
                            + Vector2.Scale(detectWallEndBot, new Vector2(facingDirection.isRIGHT() ? 1 : -1, 1));
        Collider2D[] hitWall = Physics2D.OverlapAreaAll(pointTop, pointBot);
        foreach (var i in hitWall)
        {
            if (i.gameObject.layer == climbableLayer)
                return false;
        }
        return true;
    }
    public void Falling()
    {
        isSlidingOnWall = true;
        UnstickFromWall();
    }

    #endregion

    #region 슈퍼대시 관련
    public void PrepareSuperDash()
    {
        Debug.Log("Prepare Super Dash");
        rb.gravityScale = 0;
        rb.velocity = Vector2.zero;
        moveVector = Vector2.zero;
        playerControl.ChangeMoveState(PlayerMoveState.SUPERDASH_READY);
    }

    public void LaunchSuperDash(LR direction)
    {
        Debug.Log("Launch Super Dash");
        isDoingSuperDash = true;
        LookAt2DLocal(direction.toVector2());
        playerControl.ChangeMoveState(PlayerMoveState.SUPERDASH);
        moveVector = direction.toVector2() * superDashSpeed;
    }

    public void CancelSuperDashBeforeLaunch()
    {
        Debug.Log("Cancel Super Dash Before Launch");
        rb.gravityScale = gravityScale;
        playerControl.ChangeMoveState(PlayerMoveState.DEFAULT);
    }

    public void CancelSuperDashAfterLaunch()
    {
        Debug.Log("Cancel Super Dash After Launch");
        isDoingSuperDash = false;
        rb.gravityScale = gravityScale;
        moveVector = Vector2.zero;
        playerControl.ChangeMoveState(PlayerMoveState.DEFAULT);
    }

    public void OnMoveDuringSuperDash(LR direction)
    {
        if (facingDirection == direction) return;
        CancelSuperDashAfterLaunch();
    }
    #endregion

    #region 그라운드 체크. PlayerGroundCheck.cs에서 참조
    public void SetIsGrounded(GameObject belowObject)
    {
        //해당 값이 0일 경우, 플랫폼에 평행하게 진입할 경우 바로 리턴되는 아래와 같은 문제가 있었음.
        //1. 이동하면서 더블점프하면 점프가 안 되는 버그
        //2. A방향으로 더블점프를 착지 전에 -A 방향으로 전환 시 점프 리셋이 안 됌.
        if (rb.velocity.y > 0.1f) return; // 1-way platform의 groundcheck 방지
        if (playerControl.currentMoveState == PlayerMoveState.CLIMBING) return; // climb 중 groundcheck 방지

        if (!isGrounded)
        {
            // 막 착지했을 때
            OnLanded();
        }
        isGrounded = true;
        platformBelow = belowObject;
        // playerControl.ChangeMoveState(PlayerMoveState.GROUNDED);
    }

    public void SetIsNotGrounded()
    {
        isGrounded = false;
        platformBelow = null;
        // state가 기본 상태일 경우에만 state를 수정하지 않음.
        if (playerControl.currentMoveState == PlayerMoveState.DEFAULT)
        {
            // playerControl.ChangeMoveState(PlayerMoveState.MIDAIR);
            playerRef.Animation.ResetTrigger("Grounded");
        }
    }

    /// <summary>
    /// 그라운드 체크 관리는 PlayerController.cs에서 수행하고 OnLanded를 호출해줌.
    /// </summary>
    internal void OnLanded()
    {
        playerRef.Animation.SetTrigger("Grounded");
        if (isSlidingOnWall)
        {
            isSlidingOnWall = false;
        }
        if (jumpBufferTimer?.duration < 0.3) // 최근 O.3초 이내에 점프 선입력이 있었을 경우
        {
            JumpUp();
            Debug.Log("선입력으로 인한 점프");
        }
    }
    #endregion

    public void LookAt2D(Vector2 worldPoint)
    {
        float delta = worldPoint.x - transform.position.x;
        if((facingDirection.isLEFT() && delta > 0) 
            || (facingDirection.isRIGHT() && delta < 0))
        {
            Flip();
        }
    }

    public void LookAt2DLocal(Vector2 localPoint)
    {
        if ((facingDirection.isLEFT() && localPoint.x > 0)
            || (facingDirection.isRIGHT() && localPoint.x < 0))
        {
            Flip();
        }
    }

    public bool IsFacingDirection(float x)
    { 
        if ((facingDirection.isLEFT() && x > 0) ||
            (facingDirection.isRIGHT() && x < 0))
            return false;
        return true;
    }

    public void Flip()
    {
        if (isGrabCube)
            return;

        // 플레이어가 바라보는 방향을 전환
        if (facingDirection.isLEFT()) facingDirection = LR.RIGHT;
        else facingDirection = LR.LEFT;

        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
        //aimLine.transform.localScale = theScale;
    }

    public void OnKnockback(Vector3 knockbackPos)
    {
        Vector2 knockbackDirection = transform.position - knockbackPos;
        knockbackDirection.Normalize();

        Debug.Log(knockbackDirection);
        
        if(playerControl.currentMoveState == PlayerMoveState.CLIMBING)
        {
            UnstickFromWall();
        }
        rb.velocity = Vector2.zero;
        rb.AddForce(knockbackDirection * knockbackStrength, ForceMode2D.Impulse);

        StartCoroutine(Knockback());

        IEnumerator Knockback()
        {
            playerControl.ChangeMoveState(PlayerMoveState.NO_MOVE);
            LookAt2DLocal(-knockbackDirection);     // 넉백되는 방향의 반대편 바라보기
            playerRef.Animation.SetTrigger("Hit");
            playerRef.Animation.ResetTrigger("Grounded");
            isKnockbacked = true;
            float waitTime = 0.3f;
            bool isGrounded = false;
            while (waitTime > 0)
            {
                if(playerControl.currentMoveState == PlayerMoveState.DEFAULT)
                {
                    isGrounded = true;
                    break;
                }
                waitTime -= Time.deltaTime;
                yield return null;
            }
            if(!isGrounded) playerControl.ChangeMoveState(PlayerMoveState.DEFAULT);
            isKnockbacked = false;
        }
    }

    private void OnDrawGizmos()
    {
        Vector2 detectPointTop = (Vector2)transform.position
                            + Vector2.Scale(detectWallTop, new Vector2(facingDirection.isRIGHT() ? 1 : -1, 1));
        Vector2 detectPointBot = (Vector2)transform.position
                            + Vector2.Scale(detectWallBot, new Vector2(facingDirection.isRIGHT() ? 1 : -1, 1));
        Vector2 detectPointTopWall = (Vector2)transform.position
                            + Vector2.Scale(detectWallEndTop, new Vector2(facingDirection.isRIGHT() ? 1 : -1, 1));
        Vector2 detectPointBotWall = (Vector2)transform.position
                            + Vector2.Scale(detectWallEndBot, new Vector2(facingDirection.isRIGHT() ? 1 : -1, 1));
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube((detectPointTop + detectPointBot) / 2, detectPointTop - detectPointBot);
        Gizmos.DrawWireCube((detectPointTopWall + detectPointBotWall) / 2, detectPointTopWall - detectPointBotWall);
    }
}
