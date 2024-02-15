using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

/// <summary>
/// 맵 데이터를 관리하는 매니저 클래스
/// </summary>

public class MapLoadManager : MonoBehaviour
{
    private static MapLoadManager instance;
    public static MapLoadManager Instance
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

    [SerializeField] private SceneField mainScene;
    [ShowInInspector] private List<SceneData> sceneDatas = new List<SceneData>();

    public SceneField GetMainScene() { return mainScene; }

    //씬이 있는지 파악하는 함수, 없다면 -1 있다면 인덱스 반환
    public int HaveScene(SceneField scene)
    {
        for (int i = 0; i < sceneDatas.Count; i++)
            if (sceneDatas[i].scene.isEqual(scene)) //같은 씬이 있다면,
                return i;

        return -1;
    }

    public void UploadSceneData(SceneData sceneData)
    {
        int index = HaveScene(sceneData.scene);
        if (index == -1)
            sceneDatas.Add(sceneData);
        else
            sceneDatas[index] = sceneData;
    }

    public SceneData DownloadeSceneData(int i)
    {
        return sceneDatas[i];
    }
}