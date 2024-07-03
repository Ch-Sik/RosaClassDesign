using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Utilities;

public class G_Laser : Gimmick
{
    public bool showGizmos = true;
    public SpriteRenderer sp;
    public LayerMask playerLayerMask;
    public LayerMask obstaclesLayerMask;
    public float laserMaxLength;
    public bool isActivate = true;      //작동 여부 Activate일 때만 사이클이 작동
    public float onTime = 1.0f;
    public float offTime = 1.0f;        //OFF Time이 0일 시 무한

    public bool lasing = false;
    public float length = 0.00f;

    RaycastHit hit;
    Coroutine cor;

    private void Start()
    {
        ActivateLaser();
    }

    private void Update()
    {
        if (!lasing)
        {
            sp.enabled = false;
            return;
        }
        sp.enabled = true;

        sp.size = new Vector2(sp.size.x, length);

        Detect();
    }

    [Button]
    public void ActivateLaser()
    {
        isActivate = true;
        if (cor != null)
            StopCoroutine(cor);
        cor = StartCoroutine(Laser());
    }

    [Button]
    public void InactivateLaser()
    {
        isActivate = false;
        lasing = false;
        StopCoroutine(cor);
    }

    IEnumerator Laser()
    {
        lasing = true;
        yield return new WaitForSeconds(onTime);
        lasing = false;
        yield return new WaitForSeconds(offTime);

        if (cor != null)
            StopCoroutine(cor);
        cor = StartCoroutine(Laser());
    }

    public void Detect()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, laserMaxLength, obstaclesLayerMask);

        if (showGizmos)
            Debug.DrawRay(transform.position, transform.up * laserMaxLength, Color.red);

        bool isDetected = false;

        if (hit.collider != null && (obstaclesLayerMask & (1 << hit.collider.gameObject.layer)) != 0)
        {
            // 레이캐스트가 어떤 콜라이더와 충돌했을 때
            if (hit.collider.CompareTag("Player"))
            {
                // 충돌한 콜라이더가 원하는 태그를 가졌는지 확인
                //Debug.Log("태그를 발견했습니다!");
            }
            if (hit.collider.CompareTag("Ground"))
            {
                //Debug.Log("벽과 충돌");
            }
        }

        Debug.Log(hit.collider != null ? hit.collider.gameObject.name : "");

        if (hit.collider == null)
            length = laserMaxLength;
        else
            length = Vector2.Distance(transform.position, hit.point);
    }

    public override void OffAct()
    {
        ActivateLaser();
    }

    public override void OnAct()
    {
        InactivateLaser();
    }
}
