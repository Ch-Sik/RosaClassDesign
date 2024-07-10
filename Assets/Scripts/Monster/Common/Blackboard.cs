using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 몬스터 AI에서 사용되는 변수들을 담아두기 위한 Blackboard.
/// 몬스터 GameObject의 최상단에 컴포넌트로 부착
/// </summary>
public class Blackboard : MonoBehaviour
{
    [System.Serializable]
    public class BlackboardPreviewElement
    {
        public string key;
        public string value;
        public BlackboardPreviewElement(string key, string value)
        {
            this.key = key;
            this.value = value;
        }
    }

    public List<BlackboardPreviewElement> dictPreview;

    private Dictionary<string, object> dict;

    public delegate void BlackboardUpdateEvent(string key, object value);
    // 이벤트 핸들러들이 모두 같은 오브젝트 내에 있기 때문에 같이 소멸됨 = 사라진 리스너 문제 걱정할 필요 없음.
    public BlackboardUpdateEvent OnBlackboardUpdated;       

    private void Awake()
    {
        dict = new Dictionary<string, object>();
    }

    public void Set<T>(string key, T value)
    {
        // 값 설정
        if (dict.ContainsKey(key))
        {
            dict[key] = value;
        }
        else
            dict.Add(key, value);

        // 미리보기 업데이트
        bool alreadyInList = false;
        for(int i = 0; i< dictPreview.Count; i++)
        {
            if (dictPreview[i].key == key)
            {
                if (value == null)
                    dictPreview[i].value = "null";
                else
                    dictPreview[i].value = ((T)value).ToString();

                alreadyInList = true;
                break;
            }
        }
        if(!alreadyInList)
            dictPreview.Add(new BlackboardPreviewElement(key, ((T)value).ToString()));

        // 옵저버 패턴
        if (OnBlackboardUpdated != null)
            OnBlackboardUpdated.Invoke(key, value);
    }

    /// <summary>
    /// TryGet의 return value는 nullable임에 주의!
    /// </summary>
    public bool TryGet(string key, out object item)
    {
        return dict.TryGetValue(key, out item);
    }

    /// <summary>
    /// TryGet의 return value는 nullable임에 주의!
    /// </summary>
    public bool TryGet<T>(string key, out T item)
    {
        object ret;
        if(dict.TryGetValue(key, out ret))
        {
            item = (T) ret;
            return true;
        }
        else
        {
            item = default(T);
            return false;
        }
    }
}
