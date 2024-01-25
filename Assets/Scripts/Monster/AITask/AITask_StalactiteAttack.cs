using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;
using Unity.VisualScripting;
using System.Linq;

public enum DropAttackSpawnMode 
{
    /// <summary> 대상 머리 바로 위에 배치, 낙하물이 2개가 이상인 경우는 추천하지 않음 </summary>
    Target,
    /// <summary> 가능한 범위 내에서 랜덤 위치에 배치 </summary>
    Random, 
    /// <summary> 가능한 범위 내에서 균등하게 배치 </summary>
    Evenly
}

public enum DropAttackLaunchMode
{
    /// <summary> 모든 오브젝트를 동시에 낙하 </summary>
    Parallel,
    /// <summary> 왼쪽부터 순서대로 ActiveTime에 걸쳐 낙하 </summary>
    SerialFromLeft,
    /// <summary> 오른쪽부터 순서대로 ActiveTime에 걸쳐 낙하 </summary>
    SerialFromRight,
    /// <summary> 무작위 순서대로 ActiveTime에 걸쳐 낙하 </summary>
    Random
}

// 패턴을 보스만 쓴다고 가정, 보스방에서만 실행되며 지형은 아무런 장애물이 없다고 가정함.
public class AITask_StalactiteAttack : AITask_AttackBase
{
    [SerializeField]
    private GameObject attackPrefab;
    [SerializeField, Tooltip("동시에 소환할 낙하 공격 갯수")]
    private int numOfInstance = 1;
    [SerializeField, Tooltip("낙하물 소환 높이")]
    private float spawnHeight;
    [SerializeField, Tooltip("낙하 공격 소환 시의 위치 설정 옵션")]
    private DropAttackSpawnMode spawnMode;
    [SerializeField, Tooltip("낙하 공격 발동 시의 순서 설정 옵션")]
    private DropAttackLaunchMode launchMode;

    private float leftEnd, rightEnd;        // 보스방의 왼쪽/오른쪽 끝의 x좌표
    private float secondsPerDrop;           // 낙하물 하나 떨어뜨릴 때마다 시간 간격
    private int curStalactiteCount;
    private List<MonsterStalactite> attackInstance = new List<MonsterStalactite>();
    

    [Task]
    protected void Attack()
    {
        _Attack();
    }

    [Task]
    protected void StalactiteAttack()
    {
        _Attack();
    }

    protected override void OnAttackStartupBeginFrame()
    {
        SetFields();
        SpawnObjects();
    }

    private void SetFields()
    {
        curStalactiteCount = 0;
        secondsPerDrop = activeDuration / numOfInstance;

        // 맵 사이즈 측정
        RaycastHit2D leftHit = Physics2D.Raycast(transform.position, Vector2.left, 100f, LayerMask.GetMask("Ground"));
        RaycastHit2D rightHit = Physics2D.Raycast(transform.position, Vector2.right, 100f, LayerMask.GetMask("Ground"));
        leftEnd = leftHit.point.x;
        rightEnd = rightHit.point.x;
    }

    private void SpawnObjects()
    {
        // 소환되어야할 좌표를 우선 산정. xPos 배열은 launch 순서를 반영함.
        float[] xPos = new float[numOfInstance];
        switch (spawnMode)
        {
            case DropAttackSpawnMode.Target:
                GameObject enemy;
                blackboard.TryGet(BBK.Enemy, out enemy);
                for (int i = 0; i < numOfInstance; i++)
                {
                    xPos[i] = enemy.transform.position.x;
                }
                break;
            case DropAttackSpawnMode.Random:
                for (int i = 0; i < numOfInstance; i++)
                {
                    xPos[i] = Random.Range(leftEnd, rightEnd);
                }
                break;
            case DropAttackSpawnMode.Evenly:
                float bossRoomXSize = rightEnd - leftEnd;
                float attackIntervalDist = numOfInstance == 1 ? 0 : bossRoomXSize / (numOfInstance);
                switch (launchMode)
                {
                    case DropAttackLaunchMode.Parallel:
                    case DropAttackLaunchMode.SerialFromLeft:
                    case DropAttackLaunchMode.Random:
                        for (int i = 0; i < numOfInstance; i++)
                        {
                            xPos[i] = leftEnd + (0.5f * attackIntervalDist) + i * attackIntervalDist;
                        }
                        if (launchMode == DropAttackLaunchMode.Random)
                        {
                            System.Random rand = new System.Random();
                            xPos = xPos.OrderBy(e => rand.Next()).ToArray();
                        }
                        break;
                    case DropAttackLaunchMode.SerialFromRight:
                        for (int i = 0; i < numOfInstance; i++)
                        {
                            xPos[i] = rightEnd - (0.5f * attackIntervalDist) - i * attackIntervalDist;
                        }
                        break;
                }
                break;
        }

        // 순서대로 스폰하고 attackInstance에 넣고 Init 수행시킴
        for (int i = 0; i < numOfInstance; i++)
        {
            GameObject instance = Instantiate(attackPrefab, 
                        new Vector3(xPos[i], transform.position.y + spawnHeight, 0), Quaternion.identity);
            attackInstance.Add(instance.GetComponent<MonsterStalactite>());
            instance.GetComponent<MonsterStalactite>().Init();
        }
    }

    protected override void OnAttackActiveFrames()
    {
        // 랜덤이던 왼쪽에서부터던 오른쪽에서부터건 순차적으로 내려와야 하는 경우
        if (launchMode != DropAttackLaunchMode.Parallel)
        {
            float nextShockwaveEmit = secondsPerDrop * (curStalactiteCount);
            if (activeTimer.duration >= nextShockwaveEmit)
            {
                attackInstance[curStalactiteCount].Launch();
                curStalactiteCount++;
            }
        }
        // 한꺼번에 내려와야 하는 경우
        else
        {
            if (curStalactiteCount < numOfInstance)
            {
                foreach (var instance in attackInstance)
                {
                    instance.Launch();
                }
                curStalactiteCount += numOfInstance + 1;
            }
        }
    }

    protected override void Fail()
    {
        base.Fail();
        attackInstance.Clear();
    }

    protected override void Succeed()
    {
        base.Succeed();
        attackInstance.Clear();
    }
}
