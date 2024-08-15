using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimParameterType { BOOL, INT, FLOAT, TRIGGER }

/// <summary>
/// Animator를 사용하는 몬스터 애니메이션을 컨트롤하는 스크립트
/// </summary>
public class MonsterAnimationHandler : MonoBehaviour
{
    [Serializable]
    public class MonsterAnimParameter
    {
        public string blackboardKey;
        public AnimParameterType parameterType;
        public string animParameter;
    }


    [SerializeField]
    private Blackboard blackboard;
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private List<MonsterAnimParameter> parameters;

    // Start is called before the first frame update
    void Start()
    {
        blackboard.OnBlackboardUpdated += BlackboardUpdateHandler;
    }
    
    void BlackboardUpdateHandler(string key, object value)
    {
        MonsterAnimParameter keyParamPair = parameters.Find(x => x.blackboardKey == key);
        if(keyParamPair == null)
        {
            return;
        }

        try
        {
            switch (keyParamPair.parameterType)
            {
                case AnimParameterType.BOOL:
                    animator.SetBool(key, (bool)value);
                    break;
                case AnimParameterType.INT:
                    animator.SetInteger(key, (int)value);
                    break;
                case AnimParameterType.FLOAT:
                    animator.SetFloat(key, (float)value);
                    break;
                case AnimParameterType.TRIGGER:
                    if ((bool)value == true)
                        animator.SetTrigger(key);
                    break;
            }
        }
        catch 
        {
            Debug.LogError("설정한 애니메이터 파라미터가 존재하지 않습니다! 이름 또는 타입을 확인하세요!", this);
        }
    }
}
