using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;

public class Task_G_HoldPosition : Task_Base
{
    [SerializeField] Blackboard blackboard;
    [SerializeField, Tooltip("탐지 범위 안에 플레이어가 들어왔을 때 바라볼건지")] 
    private bool lookPlayerInSight;

    // Start is called before the first frame update
    void Start()
    {
        // 컴포넌트 누락된 거 있으면 설정
        if (blackboard == null)
        {
            blackboard = GetComponent<Blackboard>();
            if (blackboard == null)
                Debug.LogError($"{gameObject.name}: Blackboard를 찾을 수 없음!");
        }
    }

    [Task]
    private void HoldPosition()
    {
        LR nowDir = GetCurrentDir();

        // 피격당했을 때 행동 중지
        bool isHitt;
        if (blackboard.TryGet(BBK.isHitt, out isHitt) && isHitt)
        {
            ThisTask.Fail();
            return;
        }

        // 적이 탐지되었을 경우 대기 종료
        GameObject enemy;
        if (blackboard.TryGet(BBK.Enemy, out enemy) && enemy != null)
        {
            if(lookPlayerInSight)
            {
                LookAt2D(PlayerRef.Instance.transform.position);
            }
            ThisTask.Fail();
            return;
        }
    }
}
