using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using AnyPortrait;

public class AnimBonePositionHandler : MonoBehaviour
{
    [SerializeField]
    private apPortrait targetPortrait;
    [SerializeField]
    private Blackboard blackboard;

    [Title("옵션")]

    [Tooltip("애니메이션에 Aim 위치 사용")]
    public bool useAimMotion;
    [ShowIf("useAimMotion"), Tooltip("공격 도중에는 Aim 고정")]
    [SerializeField] private bool stopAimWhenAttack;
    [ShowIf("useAimMotion"), Tooltip("Aim 위치를 반영할 Portrait의 Bone 이름")]
    [SerializeField] private string enemyAimBoneName;
    [ShowIf("useAimMotion"), Tooltip("Aim 따라가기 속도")]
    [SerializeField] private float aimFollowSpeed = 0.3f;

    private Vector2 currentAimPos;


    // Start is called before the first frame update
    void Start()
    {
        if(targetPortrait == null)
        {
            targetPortrait = GetComponent<apPortrait>();
            Debug.Assert(targetPortrait != null);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(useAimMotion)
        {
            bool updateAimPosition = true;

            // 에임을 업데이트해도 되는지 우선 체크
            if (stopAimWhenAttack)
            {
                int attackState;
                blackboard.TryGet(BBK.AttackState, out attackState);
                // 공격 진행중일 때 조준 따라가기 멈춤 (후딜은 포함 안됨)
                if (attackState == 1 || attackState == 2) updateAimPosition = false;    
            }

            // 에임을 업데이트해도 된다고 판단되면 에임 위치 정보 가져오기
            if (updateAimPosition)
            {
                GameObject enemy;
                blackboard.TryGet(BBK.Enemy, out enemy);
                if (enemy != null)
                {
                    // currentAimPos = Vector2.Lerp(currentAimPos, enemy.transform.position, 0.1f);
                    currentAimPos = Vector2.MoveTowards(currentAimPos, enemy.transform.position, aimFollowSpeed);
                }
            }

            // 실제 본 위치 업데이트 수행
            targetPortrait.SetBonePosition(enemyAimBoneName, currentAimPos, Space.World);
        }
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(currentAimPos, 0.2f);
        }
    }
}
