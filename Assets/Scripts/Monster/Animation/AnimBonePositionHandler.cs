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

    [SerializeField]
    private bool useEnemyAim;
    [SerializeField, ShowIf("useEnemyAim"), Tooltip("공격 도중에는 조준 고정")]
    private bool stopAimWhenAttack;
    [SerializeField, ShowIf("useEnemyAim")]
    private string enemyAimBoneName;
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
        if(useEnemyAim)
        {
            bool updateAimPosition = true;

            if (stopAimWhenAttack)
            {
                int attackState;
                blackboard.TryGet(BBK.AttackState, out attackState);
                // 공격 진행중일 때 조준 따라가기 멈춤 (후딜은 포함 안됨)
                if (attackState == 1 || attackState == 2) updateAimPosition = false;    
            }

            if (updateAimPosition)
            {
                GameObject enemy;
                blackboard.TryGet(BBK.Enemy, out enemy);
                if (enemy != null)
                {
                    // currentAimPos = Vector2.Lerp(currentAimPos, enemy.transform.position, 0.1f);
                    currentAimPos = Vector2.MoveTowards(currentAimPos, enemy.transform.position, 0.3f);
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
