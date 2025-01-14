using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Panda;

public class Task_A_Boss4SpecialAttack : Task_A_Base
{
    [InfoBox("이 패턴의 Active time은 아래의 2개 변수로 덮어씌워짐에 주의")]
    [Tooltip("좌우 와리가리 횟수")]
    [SerializeField] int moveCount;

    [Tooltip("좌우 와리가리 1번에 걸리는 시간")]
    [SerializeField] float travelTime;

    [Tooltip("좌우 와리가리할 때 '위'의 높이")]
    [SerializeField] float upperLaneOffset;

    [Tooltip("보스방 왼쪽 끝 앵커")]
    [SerializeField] Transform bossroomLeftend;

    [Tooltip("보스방 오른쪽 끝 앵커")]
    [SerializeField] Transform bossroomRightend;

    [Tooltip("상승기류")]
    [SerializeField] G_Draft wind;

    [Tooltip("사라질 때 연기 이펙트")]
    [SerializeField] GameObject blinkVFX;

    [Tooltip("본체의 비주얼")]
    [SerializeField] GameObject bodyVisual;

    [Tooltip("본체의 충돌판정")]
    [SerializeField] new Collider2D collider;

    new Rigidbody2D rigidbody;
    float _lowerLaneHeight;          // 아래쪽 라인 높이
    float _upperLaneHeight;          // 위쪽 라인 높이
    float _gravityScaleBackup;       // 중력가속도 값 보관관

    Vector3 startPosition;          // 패턴 끝나고 돌아오기 위해 처음 위치 저장

    [Task]
    void SpecialAttack()
    {
        ExecuteAttack();
    }

    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();

        this.activeDuration = travelTime * moveCount;
        _gravityScaleBackup = rigidbody.gravityScale;
    }

    protected override void OnStartupBegin()
    {
        _lowerLaneHeight = transform.position.y;
        _upperLaneHeight = _lowerLaneHeight + upperLaneOffset;
        startPosition = transform.position;

        // 1. 표효하면서 상승기류 활성화
        wind.OnAct();
        DOTween.Sequence()
            .AppendInterval(1f)
            .AppendCallback(()=>
            {
                // 2. 펑하고 사라지기
                if(blinkVFX != null)
                    blinkVFX.SetActive(true);
                bodyVisual.SetActive(false);
                collider.enabled = false;
            });
    }

    protected override void OnActiveBegin()
    {
        if(blinkVFX != null)
            blinkVFX.SetActive(false);          // 펑 이펙트 회수

        // 3. 화면의 한쪽 끝에서 나타나기. 어느쪽일지는 랜덤
        LR startPosition = Random.Range(0, 2) < 1 ? LR.LEFT : LR.RIGHT;
        if(startPosition == LR.LEFT)
        {
            transform.position = bossroomLeftend.position;
        }
        else
        {
            transform.position = bossroomRightend.position;
        }
        LookAt2D((bossroomLeftend.position + bossroomRightend.position) / 2);       // 보스방 가운데쪽 바라보기
        bodyVisual.SetActive(true);                                                 // 스프라이트 활성화
        collider.enabled = true;                                                    // 공격 판정 활성화
        rigidbody.gravityScale = 0;                                                 // 패턴 도중엔 중력 0으로 설정.

        // 4. 와리가리 n회 반복
        var seq = DOTween.Sequence();
        bool headLeft = GetCurrentDir().isLEFT();
        for(int i = 0; i<moveCount; i++)
        {
            // 위쪽 라인과 아래쪽 라인 어느쪽에서 나타날 지 랜덤 선택
            bool isUpper = Random.Range(0, 2) < 1 ? false : true;
            seq.AppendCallback(() => {
                Vector3 pos = transform.position;
                pos.y = isUpper ? _upperLaneHeight : _lowerLaneHeight;
                transform.position = pos;
                LookAt2D((bossroomLeftend.position + bossroomRightend.position) / 2);       // 보스방 가운데쪽 바라보기
            });
            // 바라보고 있는 방향으로, 보스방 끝까지 돌진
            seq.Append(rigidbody.DOMoveX(
                headLeft ? bossroomLeftend.position.x  : bossroomRightend.position.x
                , travelTime
            ));
            headLeft = !headLeft;
        }
    }

    protected override void OnRecoveryBegin()
    {
        bodyVisual.SetActive(false);            // 일단 모습 감추기
        transform.position = startPosition;     // 시작 위치로 되돌아오기
        rigidbody.gravityScale = _gravityScaleBackup;
        if(blinkVFX != null)
            blinkVFX.SetActive(true);               // 펑 이펙트 활성화
        bodyVisual.SetActive(true);             // 다시 모습 보이기
        wind.OffAct();                          // 상승기류 끔
    }
}
