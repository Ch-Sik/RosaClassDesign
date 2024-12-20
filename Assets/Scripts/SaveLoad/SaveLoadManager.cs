using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveLoadManager : MonoBehaviour
{
    #region Singleton
    private static SaveLoadManager instance;
    public static SaveLoadManager Instance
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

    public bool useSaveLoad = true;
    public bool useProjectSave = true;      //컴퓨터에 저장할지 프로젝트에 저장할지

    [FoldoutGroup("Paths"), ReadOnly] public string pathName = "SaveFile";
    [FoldoutGroup("Paths"), ReadOnly] public string flagPathName = "Flag";
    [FoldoutGroup("Paths"), ReadOnly] public string mapPathName = "Maps";
    [FoldoutGroup("Paths"), ReadOnly] public string playerPathName = "Player";


    private void Start()
    {
        Debug.Log($"세이브 사용이 {useSaveLoad}로 설정되어 있습니다.");
        MakeDirectoryHierarchy();
    }

    private void MakeDirectoryHierarchy()
    {
#if UNITY_EDITOR
        if (useProjectSave)
            MakeDirectory($"{Application.dataPath}/{pathName}");
        else
            MakeDirectory($"{Application.persistentDataPath}/{pathName}");
#else
            MakeDirectory($"{Application.persistentDataPath}/{pathName}");
#endif
        MakeDirectory(GetPath(flagPathName));
        MakeDirectory(GetPath(mapPathName));
        MakeDirectory(GetPath(playerPathName));
    }

    #region Utils
    //Path 병합해서 전달
    private string GetPath(string path)
    {
#if UNITY_EDITOR
        if (useProjectSave)
            return $"{Application.dataPath}/{pathName}/{path}";
        else
            return $"{Application.persistentDataPath}/{pathName}/{path}";
#else
            return $"{Application.persistentDataPath}/{pathName}/{path}";
#endif
    }

    private void MakeDirectory(string path)
    {
        //폴더가 존재하지 않는 경우 생성
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
    }
    #endregion

    #region Map
    public void SaveMap(string sceneName, List<int> senders, List<int> receivers, List<int> connectors)
    {
        MapSaveData Data = new MapSaveData()
        {
            sceneName = sceneName,
        };
        Data.SaveSender(senders);
        Data.SaveReceivers(receivers);
        Data.SaveConnectors(connectors);

        string filePath = GetPath(mapPathName) + $"/{sceneName}.json";
        string json = JsonUtility.ToJson(Data);
        File.WriteAllText(filePath, json);
    }

    public bool CanLoadMap(string sceneName) { return File.Exists(GetPath(mapPathName) + $"/{sceneName}.json"); }

    public MapSaveData LoadMap(string sceneName)
    {
        string filePath = GetPath(mapPathName) + $"/{sceneName}.json";
        if (!File.Exists(filePath))
        {
            Debug.LogError($"[Map Data] {filePath}를 찾을 수 없다.");
            return null;
        }

        string json = File.ReadAllText(filePath);
        MapSaveData Data = JsonUtility.FromJson<MapSaveData>(json);

        return Data;
    }
    #endregion

    #region Flag
    [Button]
    public void SaveFlag()
    {
        List<string> fs = new List<string>();
        List<int> vs = new List<int>();

        (fs, vs) = FlagManager.Instance.GetFlagsData();

        FlagSaveData Data = new FlagSaveData()
        {
            Flags = fs,
            Values = vs
        };

        string filePath = GetPath(flagPathName) + "/flag.json";
        string json = JsonUtility.ToJson(Data);
        File.WriteAllText(filePath, json);
    }

    [Button]
    public FlagSaveData LoadFlag()
    {
        string filePath = GetPath(flagPathName) + "/flag.json";
        if (!File.Exists(filePath)) {
            Debug.LogError($"[Flag Data] {filePath}를 찾을 수 없다.");
            return null;
        }

        string json = File.ReadAllText(filePath);
        FlagSaveData Data = JsonUtility.FromJson<FlagSaveData>(json);

        return Data;
    }
    #endregion

    #region Player
    #endregion
}

[Serializable]
public class MapSaveData 
{
    public string sceneName;
    public List<_MapSaveData> senders       = new List<_MapSaveData>();
    public List<_MapSaveData> receivers     = new List<_MapSaveData>();
    public List<_MapSaveData> connectors    = new List<_MapSaveData>();

    public void SaveSender(List<int> states)
    {
        for (int i = 0; i < states.Count; i++)
            senders.Add(new _MapSaveData(i, states[i]));
    }

    public void SaveReceivers(List<int> states)
    {
        for (int i = 0; i < states.Count; i++)
            receivers.Add(new _MapSaveData(i, states[i]));
    }

    public void SaveConnectors(List<int> states)
    {
        for (int i = 0; i < states.Count; i++)
            connectors.Add(new _MapSaveData(i, states[i]));
    }

    public List<int> LoadSenders()
    {
        List<int> l = new List<int>();

        for (int i = 0; i < senders.Count; i++)
            l.Add(senders[i].state);

        return l;
    }
    public List<int> LoadReceivers()
    {
        List<int> l = new List<int>();

        for (int i = 0; i < receivers.Count; i++)
            l.Add(receivers[i].state);

        return l;
    }
    public List<int> LoadConenctors()
    {
        List<int> l = new List<int>();

        for (int i = 0; i < connectors.Count; i++)
            l.Add(connectors[i].state);

        return l;
    }

    [Serializable]
    public class _MapSaveData
    {
        public int index;
        public int state;

        public _MapSaveData(int index, int state)
        {
            this.index = index;
            this.state = state;
        }
    }
}

[Serializable]
public class FlagSaveData
{
    public List<string> Flags = new List<string>();
    public List<int> Values = new List<int>();
}

[Serializable]
public class PlayerSaveData
{ 
}