using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimParameterType { BOOL, INT, FLOAT, TRIGGER }


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

        switch(keyParamPair.parameterType)
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
                animator.SetTrigger(key);
                break;
        }
    }
}
