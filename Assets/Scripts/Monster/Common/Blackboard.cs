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

    // item type이 object라서 Inspector에서 볼 수 없음 ㅜㅜ
    private Dictionary<string, object> dict;

    private void Start()
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
                if (dictPreview[i].value == null)
                    dictPreview[i].value = "null";
                else
                    dictPreview[i].value = ((T)value).ToString();

                alreadyInList = true;
                break;
            }
        }
        if(!alreadyInList)
            dictPreview.Add(new BlackboardPreviewElement(key, ((T)value).ToString()));
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
