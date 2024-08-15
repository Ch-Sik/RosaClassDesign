using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using AnyPortrait;

/// <summary>
/// Animator를 사용하지 않고 ApPortrait를 직접 컨트롤하는 스크립트
/// </summary>
public class MonsterPortraitHandler : MonoBehaviour
{
    [SerializeField]
    private Blackboard blackboard;
    [SerializeField]
    private apPortrait portrait;

    [Title("애니메이션 정보")]
    AnimData idle;
    AnimData walk;
    AnimData hitt;
    AnimData die;

    Dictionary<string, PatternAnimSet> aditionalAnimNames;

    // Start is called before the first frame update
    void Start()
    {
        blackboard.OnBlackboardUpdated += BlackboardUpdateHandler;
    }

    void BlackboardUpdateHandler(string key, object value)
    {
        switch(key)
        {
            case BBK.isMoving:
                if ((bool)value == true)
                    portrait.CrossFade(walk.name);
                else
                {
                    // 상황에 따라 idle이 출력되어야 할수도 있고 아닐수도 있음.
                }
                break;


        }
    }
}

public struct PatternAnimSet
{
    AnimData startup;
    AnimData active;
    AnimData recovery;
}

public struct AnimData
{
    public string name;
    public float crossfadeTime;
    public bool needQueued;
    // float offset;
}
