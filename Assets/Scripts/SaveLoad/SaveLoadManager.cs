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
            Debug.LogError($"{filePath}를 찾을 수 없다.");
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
    public string MapName;
    public List<GimmickSignalSender> a;
    public List<GimmickSignalReceiver> b;
    public List<GimmickSignalConnector> c;
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