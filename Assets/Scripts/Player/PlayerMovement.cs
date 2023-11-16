using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


/// <summary>
/// 플레이어의 움직임 담당.
/// </summary>
public class PlayerMovement : MonoBehaviour
{

    [Header("좌우 이동 관련 매개변수")]// 이동 관련 파라미터
    [Tooltip("플레이어 이동 속도")]
    [SerializeField] float moveSpeed = 1f;
    [Tooltip("시작 시 플레이어 스프라이트의 방향")]
    [SerializeField] LR spriteDirection = LR.RIGHT;

    [Header("점프 관련 매개변수")]
    [Tooltip("플레이어 점프 파워")]
    [SerializeField] float jumpPower = 10f;
    [Tooltip("최소 상승 시간")]
    [SerializeField] float minJumpUpDuration = 0.1f;

    [Header("벽이동 관련 매개변수")]
    [Tooltip("벽 기어오르기 활성화")]
    [SerializeField] bool wallClimbEnabled;
    [Tooltip("기어오르기 속도")]
    [SerializeField] float climbUpSpeed;
    [Tooltip("기어내려가기 속도")]
    [SerializeField] float climbDownSpeed;
    [Tooltip("벽 점프 활성화")]
    [SerializeField] bool wallJumpEnabled;
    [Tooltip("벽 점프 시 속도")]
    [SerializeField] Vector2 wallJumpPower;
    [Tooltip("벽 점프 최소시간")]
    [SerializeField] float minWallJumpDuration; // 기존 boomerangTime
    [Tooltip("반대를 보고 벽에 메달리는 시간")]
    [SerializeField] float maxClimbTime;

    // 컴포넌트 레퍼런스
    [ReadOnly, SerializeField] Rigidbody2D rb;
    [ReadOnly, SerializeField] BoxCollider2D col;
    [ReadOnly, SerializeField] PlayerController playerControl;

    [Space(20)]

    // 필드
    [Header("Debug View")]
    [ReadOnly, SerializeField] private GameObject platformBelow = null;
    //[ReadOnly, SerializeField] public GameObject aimLine;
    //[ReadOnly, SerializeField] public GameObject hookLine;
    [ReadOnly, SerializeField] public Vector2 moveVector;
    [ReadOnly, SerializeField] public LR facingDirection;                 // 플레이어 바라보는 방향

    // 타이머
    [ReadOnly, SerializeField] private Timer jumpTimer;                 // 최소 점프 시간을 위한 타이머
    [ReadOnly, SerializeField] private Timer jumpBufferTimer;           // 점프 선입력 타이머
    [ReadOnly, SerializeField] private Timer climbTimer;                // 반대방향 입력후 벽에 메달림 유지 타이머

    // 플래그
    [ReadOnly, SerializeField] public bool isFacingWall = false;
    [ReadOnly, SerializeField] public bool isWallClimbing = false;      // 벽에 붙어서 이동중
    [ReadOnly, SerializeField] public bool isWallClimbingTop = false;      // 벽에 붙어서 이동중 정상도달
    [ReadOnly, SerializeField] public bool isWallJumping = false;       // 벽 점프 중인지
    [ReadOnly, SerializeField] public bool isDoingMagic = false;
    //[ReadOnly, SerializeField] public bool isKnockbacked = false;
    [ReadOnly, SerializeField] public bool isFalling = false;
    [ReadOnly, SerializeField] public bool isDoingHooking = false;      // 후크액션을 수행하고 있는지
    [ReadOnly, SerializeField] public bool isHitHookingTarget = false;  // 후크액션중 후크목표에 도달했는지
                                                                        // 상수
    LayerMask groundLayer;      // NameToLayer가 constructor에서 호출 불가능하여 InitFields에서 초기화
    LayerMask climbableLayer;
    float gravityScale = 2.8f;
    
    // 범위 지정
    private Vector2 detectWallTop = new Vector2(0.0f, 0.5f);
    private Vector2 detectWallBot = new Vector2(0.6f, -0.7f);
    private Vector2 detectWallEndTop = new Vector2(0.0f, 0.5f);
    private Vector2 detectWallEndBot = new Vector2(0.6f, 0.0f);

    // 코루틴
    private Coroutine hookCoroutine; // 후크 중 코루틴

    // 애니메이션
    public PlayerAnimation playerAnim;

    //싱글톤
    [ReadOnly, SerializeField] PlayerRef playerRef;

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
        playerAnim = playerRef.Animation;
    }

    void InitFields()
    {
        //aimLine = GameObject.Find("AimLine");
        //hookLine = GameObject.Find("HookLine");
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
            isWallClimbingTop = CheckWallEnd();
            isFalling = false;
        }
    }

    /// <summary>
    /// Walk와 Climb 등에서 설정한 moveVector를 실제로 플레이어 움직임에 반영
    /// </summary>
    void UpdateMovement()
    {
        if (!isWallJumping)    // 벽 점프 시작 후 minWallJumpDuration 동안은 좌우 이동 불가
        {
            if (playerControl.MoveState == PlayerMoveState.CLIMBING)
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
            else
            {
                if (!isDoingHooking) // 후크 중일경우 좌 우 입력 값 무시
                {
                    rb.velocity = new Vector2(moveVector.x * moveSpeed, rb.velocity.y);
                }
                if ((moveVector.x > 0 && facingDirection.isLEFT())
                || (moveVector.x < 0 && facingDirection.isRIGHT()))
                {
                    Flip();
                }
            }
        }
    }

    

    #endregion

    #region 움직임 관련 함수들

    /// <summary>
    /// 지상과 공중에서 x축 방향으로의 이동. 
    /// 여기서는 조작 입력값만 받고 실제 속도 업데이트는 UpdateMovement에서 이루어짐.
    /// </summary>
    /// <param name="inputVector">x,y 모두 -1~+1 사이의 값인 입력 벡터</param>
    internal void Walk(Vector2 inputVector)
    {
        moveVector = new Vector2(inputVector.x, 0);
    }

    /// <summary>
    /// 벽면에서의 y축 방향으로의 이동.
    /// 여기서는 조작 입력값만 받고 실제 속도 업데이트는 UpdateMovement에서 이루어짐.
    /// </summary>
    /// <param name="inputVector"></param>
    internal void Climb(Vector2 inputVector)
    {        
        Debug.Log("Climbing");
        if (!DetectWall())
        {
            // TODO: 지면으로 올라가기 구현
            
        }
        moveVector = inputVector;       // 벽 점프 등을 위해 x축 방향 필터링하지 않음.
        if(moveVector.x != 0)
        {
            if (!DetectWall())
            {
                Debug.Log("start Timer");
                climbTimer = Timer.StartTimer();
            }
        }
        

    }

    internal void StopClimb(Vector2 inputVector)
    {
        if (moveVector.x != 0)
        {
            if(!DetectWall())
            {
                Debug.Log("TimerCheck: " + climbTimer.duration);
                if (climbTimer.duration > maxClimbTime)
                {
                    UnstickFromWall();
                    Flip();
                    isFalling = true;
                }
            }
        }
        moveVector = inputVector;
    }


    internal void JumpUp()
    {
        if (isDoingHooking && isHitHookingTarget) // 후크 목표에 도달했고 아직 후크 액션 사용중일 경우 점프로 캔슬
        {
            isDoingHooking = false;
        }
        //playerAnim.SetTrigger("JumpTrigger");
        rb.velocity = new Vector2(rb.velocity.x, jumpPower);
        jumpTimer = Timer.StartTimer();
    }

    /// <summary>
    /// 숏점프와 롱점프 구분을 위해 조기 점프종료하는 함수
    /// </summary>
    internal void FinishJumpUp()
    {
        if (jumpTimer.duration > minJumpUpDuration) // 최소 점프 시간에 도달
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
        col.isTrigger = true;       // 지형 통과 가능
    }

    /// <summary>
    /// OnTriggerExit2D에 의해 호출됨. 플랫폼을 완전히 통과하면 트리거 해제
    /// </summary>
    void FinishJumpDown()
    {
        // Debug.Log("FinishJumpDown");
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
        isWallJumping = true;
        UnstickFromWall();
        //playerAnim.SetTrigger("JumpTrigger");
        playerControl.ChangeMoveState(PlayerMoveState.MIDAIR);

        if(!isFalling)
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

    /// <summary>
    /// 그라운드 체크 관리는 PlayerController.cs에서 수행하고 OnLanded를 호출해줌.
    /// </summary>
    internal void OnLanded()
    {
        Debug.Log("land");
        if (isDoingHooking) // 땅에 닿았을 시 후크액션 종료 판정
        {
            isDoingHooking = false;

        }
        if (isFalling)
        {
            isFalling = false;
        }
        if (jumpBufferTimer?.duration < 0.3) // 최근 O.3초 이내에 점프 선입력이 있었을 경우
        {
            JumpUp();
            Debug.Log("선입력으로 인한 점프");
        }
    }

    /// <summary>
    /// 벽 오르기 상태로 전환
    /// </summary>
    internal void StickToWall()
    {
        if (playerControl.MoveState == PlayerMoveState.GROUNDED)
        {

        }
        if (isDoingHooking && !isHitHookingTarget)
        {

        }
        else
        {
            playerControl.ChangeMoveState(PlayerMoveState.CLIMBING);
            rb.gravityScale = 0;
        }

    }

    /// <summary>
    /// 벽 오르기 해제.
    /// </summary>
    internal void UnstickFromWall()
    {
        playerControl.ChangeMoveState(PlayerMoveState.MIDAIR);
        rb.gravityScale = this.gravityScale;
        
    }

    /// <summary>
    /// 목표를 향해 후크액션 사용
    /// 실제 힘을 가하는 부분은 SeedHook에서 처리
    /// </summary>
    internal void HookToTarget(Vector3 target)
    {


        isDoingHooking = true;
        isHitHookingTarget = false;

        if (playerControl.MoveState == PlayerMoveState.CLIMBING)
        {
            UnstickFromWall();
        }

        playerAnim.SetTrigger("HookTrigger");

        if (hookCoroutine == null)
        {
            hookCoroutine = StartCoroutine(DoingHookAction());
        }
        else
        {
            StopCoroutine(hookCoroutine);
            hookCoroutine = StartCoroutine(DoingHookAction());
        }

        IEnumerator DoingHookAction() // 후크 중 넝쿨 역할을 하는 라인 생성
        {
            //hookLine.SetActive(true);
            while (!isHitHookingTarget)
            {
                Vector2 endPoint = target - gameObject.transform.position;
                //hookLine.GetComponent<Line>().End = endPoint;
                yield return null;
            }
            //hookLine.SetActive(false);
            hookCoroutine = null;
        }
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
        if (collision.gameObject.layer.Equals(groundLayer))
        {
            // 땅에 닿았을 경우 후크 라인 표시 종료
            if (hookCoroutine != null)
            {
                isHitHookingTarget = true;
                StopCoroutine(hookCoroutine);
                //hookLine.SetActive(false);
                hookCoroutine = null;
            }
        }
        // 벽에 설치된 덩굴에 충돌하였을 경우
        if (collision.gameObject.layer.Equals(climbableLayer))
        {
            //Debug.Log("덩굴과 충돌");
            if (isFacingWall && !isFalling)
            {

                StickToWall();
                //playerAnim.SetTrigger("ClimbTrigger");
            }

        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // 벽에 설치된 덩굴에 충돌하였을 경우
        if (collision.gameObject.layer.Equals(climbableLayer))
        {
            //Debug.Log("덩굴과 충돌");
            if (isFacingWall && !isFalling)
                StickToWall();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "HookObj")
        {
            isHitHookingTarget = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "HookObj")
        {
            isHitHookingTarget = true;
        }
    }

    #endregion

    // 플레이어 점프 파워 조정
    public float SetJumpPower(float power)
    {
        float temp = jumpPower;
        jumpPower = power;
        return temp;
    }

    bool DetectWall()
    {
        if (isDoingHooking && !isHitHookingTarget) return false;
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

    void Flip()
    {
        // 플레이어가 바라보는 방향을 전환
        if (facingDirection.isLEFT()) facingDirection = LR.RIGHT;
        else facingDirection = LR.LEFT;

        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
        //aimLine.transform.localScale = theScale;
        //hookLine.transform.localScale = theScale;
    }

    public void OnKnockback()
    {
        StartCoroutine(Knockback());

        IEnumerator Knockback()
        {
            playerControl.ChangeMoveState(PlayerMoveState.CANNOTMOVE);
            yield return new WaitForSeconds(0.5f);
            playerControl.ChangeMoveState(PlayerMoveState.MIDAIR);
        }
    }

    public void Falling()
    {
        Debug.Log("Fall");
        isFalling = true;
        UnstickFromWall();
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
