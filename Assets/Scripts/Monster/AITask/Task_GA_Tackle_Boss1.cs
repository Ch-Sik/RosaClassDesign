using DG.Tweening;
using Panda;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task_GA_Tackle_Boss1 : Task_GA_Tackle
{
    [FoldoutGroup("그로기 관련")]
    [Tooltip("그로기 유지 시간")]
    [SerializeField] float groggyDuration;
    [FoldoutGroup("그로기 관련")]
    [Tooltip("그로기 풀린 이후 정신차리는 모션 시간")]
    [SerializeField] float groggyRecoveryAnimDuration;


    Timer groggyTimer = null;

    protected override void OnActiveLast()
    {
        // 블랙보드에서 벽에 박음 정보 가져오기
        bool isStuckAtWall;
        blackboard.TryGet(BBK.StuckAtWall, out isStuckAtWall);

        // 벽에 박았을 경우 그로기
        if (wallStunEnabled && isStuckAtWall)
        {
            // 오버라이드: 스턴을 그로기와 스턴으로 세분화
            // 스턴은 그로기 도중 플레이어에게 밟히면 그때 수행되는 것으로.

            bool isStunned;
            blackboard.TryGet(BBK.isStunned, out isStunned);

            // 벽에 박았는데 아직 스턴이 아니다 -> 그로기 상태
            if (!isStunned)
            {
                // 그로기 첫 프레임
                if (groggyTimer == null)
                {
                    OnGroggyStart();
                }
                // 그로기 중간 프레임
                else if (groggyTimer.duration < groggyDuration)
                {
                    OnGroggyLast();
                }
                // 그로기 마지막 프레임
                else
                {
                    OnGroggyEnd();
                }
            }
            // 그로기 상태에서 플레이어에게 밟히면 -> 스턴 상태
            else
            {
                Debug.Assert(stunTimer != null);
                // 스턴 중간 프레임
                if (stunTimer.duration < wallStunDuration)
                {
                    OnStunLast();
                }
                // 스턴 마지막 프레임
                else
                {
                    OnStunEnd();
                }
            }

            return;
        }

        // 돌진 도중 방향 전환 옵션 켜진경우, 돌진 도중에도 방향 계속 체크
        if (allowUturn)
        {
            CalculateAttackDirection(false);
        }

        // 실제 돌진 수행
        DoTackle();
    }

    private void OnGroggyStart()
    {
        Debug.Log("벽에다 대가리 꽁!!!");
        groggyTimer = Timer.StartTimer();
        blackboard.Set(BBK.isGroggy, true);    // 애니메이션을 위한 블랙보드 설정
        damageComponent.attackEnabled = false;    // 플레이어가 밟을 수 있게 데미지 비활성화
    }

    private void OnGroggyLast()
    {
        ThisTask.debugInfo = $"groggy: {groggyDuration - groggyTimer.duration}";
        activeTimer.Reset();    // Task_A_Base에 의해 그로기 도중에 패턴 종료되는 것 방지
        return;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("GroundCheck"))
        {
            Debug.Log("플레이어에게 밟힘!");
            // 플레이어에게 밟혔는데 그게 마침 그로기중이었다면
            if(groggyTimer != null)
            {
                // 그로기 대신 스턴으로 이행
                DoStun();
                // 그리고 머리 위에 있는 플레이어를 저 멀리 밀어내기
                ThrowPlayer();
            }
        }
    }

    private void OnGroggyEnd()
    {
        // Debug.Log("그로기 끝");
        // 그로기는 끝났어도 '정신차리기' 애니메이션을 위해 잠시 추가로 대기
        if(groggyTimer.duration < groggyDuration + groggyRecoveryAnimDuration)
        {
            blackboard.Set(BBK.isGroggy, false);
            damageComponent.attackEnabled = true;        // 몸통 데미지 다시 활성화
        }
        else
        {
            groggyTimer = null;
            Succeed();
        }
    }

    private void DoStun()
    {
        groggyTimer = null;
        stunTimer = Timer.StartTimer();
        blackboard.Set(BBK.isGroggy, false);
        blackboard.Set(BBK.isStunned, true);

        // 바로 데미지 활성화하면 몬스터에 처맞아서 약간의 딜레이 부여
        Sequence seq = DOTween.Sequence().AppendInterval(0.2f)  
            .AppendCallback( () => { 
                damageComponent.attackEnabled = true;  // 몸통 데미지 다시 활성화
            });
    }

    private void ThrowPlayer()
    {
        PlayerRef.Instance.movement.Knockback(
            (Vector2)transform.position 
            + GetCurrentDir().toVector2() * 5f + Vector2.down * 3f
        );
    }

    private void OnStunLast()
    {
        ThisTask.debugInfo = $"stun: {wallStunDuration - stunTimer.duration}";
        activeTimer.Reset();    // Task_A_Base에 의해 그로기 도중에 패턴 종료되는 것 방지
    }

    private void OnStunEnd()
    {
        stunTimer = null;
        blackboard.Set(BBK.isStunned, false);
        Debug.Log("스턴 끝");
        Succeed();
        return;
    }
}
