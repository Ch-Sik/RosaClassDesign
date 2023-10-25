using DG.Tweening;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

/// <summary>
/// 나비의 웨이포인트들과 데이터들을 다루기 위한 프리팹 스크립트입니다.
/// </summary>

public class ButterFly : MonoBehaviour
{
    public bool showGizmo = true;                       //기즈모 표기 유무
    public Color gizmoColor = Color.red;                //라인 구분을 위한 기즈모 컬러 세팅
    [ShowInInspector] bool onWayTracking = false;       //waytacking을 하고 있는가?

    [ShowInInspector] private Vector3[] waypoints;      //연결된 웨이포인트 점의 집합
    [ShowInInspector] private float distance;           //웨이포인트 점을 따라 갈 때의 거리

    private Sequence tracking;                          //트래킹 시퀀스
    public Transform butterFly;                         //움직일 나비를 받아올 트랜스폼 오브젝트
    public Transform waypointMoules;                    //웨이포인트점들의 부모 오브젝트

    Transform rider;                                    //현재 나비를 탄 대상의 트랜스폼

    float savedGravity = 0f;

    //시작과 동시에 waypoint를 연산하고, 거리데이터를 산출한다.
    private void Start()
    {
        SetData();
    }

    //나비가 날기위한 waypoint와 거리 데이터를 산출한다.
    public void SetData()
    {
        PathsToVector3Array();
        DistanceCalcultor();
    }

    //나비가 날게 되는 핵심 코드
    [Button]
    public void ButterFlyAct(Transform player)
    {
        if (onWayTracking)
            return;

        tracking = DOTween.Sequence()
        //DOPath 이전에 기본적 세팅 //tracking 여부를 true로 설정하고, 플레이어를 나비에 태운다.
        .AppendCallback(() =>
        {
            onWayTracking = true;
            Ride(player);
        })
        //플레이어의 위치를 나비의 위치로 0.3초 동안 이동시킨다.
        .Append(player.DOMove(butterFly.position, 0.3f))
        //나비의 자식으로 플레이어가 있기에, 나비를 이동시키며 플레이어를 동시에 이동시킨다. 이때 나비는 waypoints를 순차적으로 방문한다.
        .Append(butterFly.DOPath(waypoints, distance / 3))
        //DOPath 완료후 세팅 // tracking 여부를 false로 설정하고, 트래킹을 초기화 하며, 플레이어를 내리게한다.
        .AppendCallback(() =>
        {
            ResetTracking();
            onWayTracking = false;
        });
    }

    //나비의 위치를 멈추게 한다.
    public void StopTracking()
    {
        tracking.Pause();
    }

    //트래킹을 취소시킨다.
    public void ResetTracking()
    {
        StopTracking();
        UnRide();
        butterFly.position = waypoints[0];
        onWayTracking = false;
    }

    //나비에 탈 때, 
    public void Ride(Transform player)
    {
        rider = player;                                         //탑승자 데이터를 저장한다.
        player.SetParent(butterFly);                            //플레이어를 자식으로 설정해준다.
        butterFly.GetComponent<Collider2D>().enabled = false;   //나비와의 재충돌을 대비해 콜라이더를 끈다.
        savedGravity = PlayerRef.Instance.rb.gravityScale;
        PlayerRef.Instance.rb.gravityScale = 0;
    }

    //나비에 내릴 때, (이름은 추후 생각해보기)
    public void UnRide()
    {
        rider.SetParent(null);                                  //탑승자를 내린다.
        butterFly.GetComponent<Collider2D>().enabled = true;    //나비의 재활성화
        PlayerRef.Instance.rb.gravityScale = savedGravity;
    }

    //자식데이터를 토대로 Vector3배열을 만든다.
    private void PathsToVector3Array()
    {
        int count = waypointMoules.childCount;          //Waypoint 자식의 개수

        waypoints = new Vector3[count + 1];             //나비의 위치도 포함하기 위하여 count + 1을 넣는다.

        waypoints[0] = butterFly.position;              //0번 Vector는 당연히 나비의 위치이다.

        for (int i = 0; i < count; i++)                 //자식을 순회하며 waypoint를 설정해준다.
            waypoints[i + 1] = waypointMoules.GetChild(i).position;
    }

    //waypoints의 각점을 비교하여 최종 거리를 산출한다.
    private void DistanceCalcultor()
    {
        distance = 0;
        for (int i = 0; i < waypoints.Length - 1; i++)
            distance += Vector3.Distance(waypoints[i], waypoints[i + 1]);
    }


    private void OnDrawGizmos()
    {
        if (!showGizmo)
            return;

        if (onWayTracking)
            return;

        SetData();

        Gizmos.color = gizmoColor;
        if (waypoints != null && waypoints.Length > 1)
        {
            Gizmos.DrawLine(butterFly.transform.position, waypoints[0]);
            for (int i = 0; i < waypoints.Length - 1; i++)
            {
                Gizmos.DrawLine(waypoints[i], waypoints[i + 1]);
            }
        }
    }
}