using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G_Lazer : MonoBehaviour
{
    public bool showGizmos = true;
    public LayerMask playerLayerMask;
    public LayerMask obstaclesLayerMask;
    public float lazerMaxLength;
    public bool isActivate = true;      //작동 여부 Activate일 때만 사이클이 작동
    public float onTime = 1.0f;
    public float offTime = 1.0f;        //OFF Time이 0일 시 무한

    public bool lazing = false;

    RaycastHit hit;
    Coroutine cor;

    private void Start()
    {
        ActivateLazer();

        Debug.Log("start");
    }

    private void Update()
    {
        if (!lazing)
            return;

        Detect();
        Debug.Log("On");
    }

    [Button]
    public void ActivateLazer()
    {
        isActivate = true;
        if (cor != null)
            StopCoroutine(cor);
        cor = StartCoroutine(Lazer());
    }

    [Button]
    public void InactivateLazer()
    {
        isActivate = false;
        lazing = false;
        StopCoroutine(cor);
    }

    IEnumerator Lazer()
    {
        lazing = true;
        yield return new WaitForSeconds(onTime);
        lazing = false;
        yield return new WaitForSeconds(offTime);

        if (cor != null)
            StopCoroutine(cor);
        cor = StartCoroutine(Lazer());
    }

    public void Detect()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, lazerMaxLength, obstaclesLayerMask);

        if (showGizmos)
            Debug.DrawRay(transform.position, transform.up * lazerMaxLength, Color.red);

        if (hit.collider != null && (playerLayerMask & (1 << hit.collider.gameObject.layer)) != 0)
        {
            // 레이캐스트가 어떤 콜라이더와 충돌했을 때
            if (hit.collider.CompareTag("Player"))
            {
                // 충돌한 콜라이더가 원하는 태그를 가졌는지 확인
                Debug.Log("태그를 발견했습니다!");
            }
        }
    }
}
