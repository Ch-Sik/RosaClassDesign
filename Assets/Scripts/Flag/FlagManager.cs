using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagManager : MonoBehaviour
{
    #region Singleton
    private static FlagManager instance;
    public static FlagManager Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    #endregion
    public FlagSO flagSO;
    [ShowInInspector] private Dictionary<string, int> flags = new Dictionary<string, int>();

    private void Start()
    {
        Init();
    }

    //세이브 있을 시 Param으로 save 받기
    private void Init()
    {
        Load();

        foreach (Flag eachFlag in flagSO.flags)
            flags.Add(eachFlag.flag, eachFlag.defaultFlagValue);
    }

    #region Utiles
    public void SetFlag(string flag, int value)
    {
        if (!flags.ContainsKey(flag))
        {
            Debug.LogError($"Flag [{flag}]이 존재하지 않습니다.");
            return;
        }

        flags[flag] = value;
    }

    public Flag GetFlagObject(string flag)
    {
        if (!flags.ContainsKey(flag))
        {
            Debug.LogError($"Flag [{flag}]이 존재하지 않습니다.");
            return null;
        }

        return new Flag(flag, flags[flag]);
    }

    public int GetFlag(string flag)
    {
        if (!flags.ContainsKey(flag))
        {
            Debug.LogError($"Flag [{flag}]이 존재하지 않습니다.");
            return -1;
        }

        return flags[flag];
    }
    #endregion

    #region SaveLoad
    private void Save()
    {
    }

    private void Load()
    {
        
    }
    #endregion
}
