using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;

public class Task_A_LastBossIvy : Task_A_Base
{
    [SerializeField]
    GameObject prefab;
    [SerializeField]
    int spawnCount;
    [SerializeField]
    float spawnableAreaWidth;
    [SerializeField]
    float spawnPositionY;
    [SerializeField]
    float randomOffsetX;
#if UNITY_EDITOR
    [SerializeField]
    bool drawGizmos;
#endif

    List<GameObject> instances;

    [Task]
    void IvyAttack()
    {
        ExecuteAttack();
    }

    // 씨앗 활성화
    protected override void OnStartupBegin()
    {
        instances = new List<GameObject>();
        for (int i=0; i<spawnCount; i++)
        {
            float xPos = transform.position.x + spawnableAreaWidth * (-0.5f + (float)i / (spawnCount - 1));
            float randomedXpos = xPos + Random.Range(-randomOffsetX, randomOffsetX);        // 너무 균일해보이지 않게 x위치 랜덤하게 조정
            Debug.Log($"xPos: {xPos}, after random added: {randomedXpos}");
            instances.Add(Instantiate(prefab, new Vector3(randomedXpos, spawnPositionY, 0), Quaternion.identity));
        }
    }

    // 덩굴 자라기 시작
    protected override void OnActiveBegin()
    {
        foreach (GameObject go in instances)
        {
            go.GetComponent<LastBossIvy>().StartGrow();
        }
    }

    // 덩굴 삭제는 덩굴 자체에 부여된 lifetime으로 관리
    protected override void OnRecoveryBegin()
    {
        
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if(!drawGizmos) return;
        Gizmos.color = Color.red;
        Vector3 left = new Vector3(transform.position.x - 0.5f * spawnableAreaWidth, spawnPositionY, 0);
        Vector3 right = new Vector3(transform.position.x + 0.5f * spawnableAreaWidth, spawnPositionY, 0);
        Gizmos.DrawLine(left, right);
    }
#endif
}
