using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastBossIvy : MonoBehaviour
{
    [SerializeField] GameObject segmentPrefab;
    [SerializeField] float growHeight;
    [SerializeField] float growPerSec;
    [SerializeField] float unitPerSegment;
    [SerializeField] float lifetime;
    [Title("사인파 형태 조정")]
    [SerializeField] float frequency;
    [SerializeField] float amplitude;

    List<GameObject> segments = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        int segmentCount = Mathf.CeilToInt(growHeight / unitPerSegment);
        for (int i = 0; i < segmentCount; i++)
        {
            GameObject instance = Instantiate(segmentPrefab, transform);
            instance.SetActive(false);
            segments.Add(instance);
        }
    }

    [Button]
    public void Reset()
    {
        StopAllCoroutines();
        foreach(var o in segments)
        {
            o.SetActive(false);
        }
    }

    [Button]
    public void StartGrow()
    {
        StartCoroutine(co_Grow());
        Invoke("DoDestroy", lifetime);

        IEnumerator co_Grow()
        {
            float secPerSegment = (1 / growPerSec) * (unitPerSegment);
            float vias = Random.Range(0, 2 * Mathf.PI);

            for (int i = 0; i < segments.Count; i++)
            {
                // 각 알맹이의 위치 계산
                float growth = i * unitPerSegment;
                float x = transform.position.x + Mathf.Sin(growth * frequency + vias) * amplitude;
                float y = transform.position.y + growth;
                // 알맹이 위치 지정 & 활성
                segments[i].SetActive(true);
                segments[i].transform.position = new Vector3(x, y, transform.position.z);
                // 딜레이 부여
                yield return new WaitForSeconds(secPerSegment);
            }
        }
    }

    void DoDestroy()
    {
        // TODO: 덩굴 삭제되는 연출 구현
        Destroy(gameObject, 0.5f);
    }
}
