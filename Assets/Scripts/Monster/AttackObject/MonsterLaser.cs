using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterLaser : MonoBehaviour
{
    [Title("게임오브젝트/컴포넌트 레퍼런스")]
    [SerializeField, Tooltip("레이저 공격 이펙트의 시작부분")]
    private GameObject beamStart;
    [SerializeField, Tooltip("레이저 공격 이펙트의 길쭉한 부분")]
    private GameObject beamMid;
    [SerializeField, Tooltip("레이저 공격 이펙트의 끝부분")]
    private GameObject beamEnd;
    [SerializeField, Tooltip("레이저 공격 판정")]
    private Collider2D laserCollider;
    [SerializeField, Tooltip("레이저 공격 처리 컴포넌트")]
    private MonsterDamageInflictor damageInflictor;

    [Title("레이저 형태 관련 파라미터")]
    [SerializeField, Range(0.01f, 20.0f)]
    private float laserWidth = 0.5f;
    [SerializeField, Tooltip("레이저가 지형에 막히지 않았을 경우의 최대 길이")]
    private float laserMaxLength = 20.0f;
    [SerializeField, Tooltip("빔이 막힐 지형을 나타내는 레이어들 선택")]
    LayerMask terrainLayers;

    private bool collideWithTerrain;
    private float laserLength;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(beamMid != null);
        if(laserCollider == null)
        {
            laserCollider = GetComponentInChildren<Collider2D>();
            Debug.Assert(laserCollider != null);
        }
    }

    public void Initalize(Vector2 dir)
    {
        // 레이저 방향에 맞게 회전
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.eulerAngles = new Vector3(0, 0, angle);

        // 필요한 오브젝트/컴포넌트만 활성화하고 나머지는 비활성화
        gameObject.SetActive(true);
        beamStart?.SetActive(true);
        beamMid.SetActive(false);
        beamEnd?.SetActive(false);
        laserCollider.enabled = false;

        // raycast 수행 후 레이저 길이 조절
        RaycastHit2D hit;
        hit = Physics2D.Raycast(transform.position, dir, laserMaxLength, terrainLayers);
        if(hit.collider == null)
        {
            collideWithTerrain = false;
            laserLength = laserMaxLength;
        }
        else
        {
            collideWithTerrain = true;
            laserLength = hit.distance;
        }

        // 만약에 레이저 발사 방향 미리보기 필요하다면 여기에다가 구현하기
    }

    public void Activate(int damage)
    {
        // 오브젝트/컴포넌트 모두 활성화
        beamMid.SetActive(true);
        beamEnd?.SetActive(true);

        // 레이저 크기/위치 조절
        beamMid.transform.localPosition = new Vector3(laserLength / 2, 0, 0);
        beamMid.transform.localScale = new Vector3(laserLength, laserWidth, 1.0f);
        if(beamEnd!=null)
        {
            beamEnd.transform.localPosition = new Vector3(laserLength, 0, 0);
            beamEnd.transform.localScale = new Vector3(1.0f, laserWidth, 1.0f);
        }
        // TODO: 레이저가 활성화될때 자연스러운 느낌 나도록 트위닝 적용하기

        // 데미지 값 설정하고 콜라이더 활성화하기
        damageInflictor.damage = damage;
        laserCollider.enabled = true;
    }

    public void Terminate()
    {
        // TODO: 레이저 비활성화되기 전에 자연스러운 느낌 나도록 트윈 적용

        // 불필요한 오브젝트 비활성화
        beamStart?.SetActive(false);
        beamMid.SetActive(false);
        beamEnd?.SetActive(false);
        gameObject.SetActive(false);

        // 레이저 공격 판정 비활성화
        laserCollider.enabled = false;
    }
}
