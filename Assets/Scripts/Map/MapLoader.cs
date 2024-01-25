using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 하부의 LoadingZone에서 플레이어를 감지한 순간 로드씬과 언로드씬을 배치함. 
/// </summary>

public class MapLoader : MonoBehaviour
{
    [SerializeField] private SceneField thisScene;          //맵 로더가 포함된 씬
    [SerializeField] private SceneField[] scenesToLoad;     //로드해올 씬
    [SerializeField] private SceneField[] scenesToUnload;   //언로드할 씬

    GameObject player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    //씬 로드
    public void SceneLoader()
    {
        for (int i = 0; i < scenesToLoad.Length; i++)
        {
            bool isSceneLoaded = false;
            for (int j = 0; j < SceneManager.sceneCount; j++)
            {
                Scene loadedScene = SceneManager.GetSceneAt(j);
                //중복검사
                if (loadedScene.name == scenesToLoad[i].SceneName)
                {
                    isSceneLoaded = true;
                    break;
                }
            }
            //중복이 아니라면, 씬을 덧댄다.
            if (!isSceneLoaded)
                SceneManager.LoadSceneAsync(scenesToLoad[i], LoadSceneMode.Additive);
        }
    }

    //씬 언로드
    public void SceneUnloader()
    {
        for (int i = 0; i < scenesToUnload.Length; i++)
        {
            for (int j = 0; j < SceneManager.sceneCount; j++)
            {
                Scene loadedScene = SceneManager.GetSceneAt(j);
                if (loadedScene.name == scenesToUnload[i].SceneName)
                    SceneManager.UnloadSceneAsync(scenesToUnload[i]);
            }
        }
    }

    public void SaveMapEntities()
    { 

    }

    public void LoadMapEntities()
    {
        
    }

    public void Loading()
    {
        SceneLoader();
        SceneUnloader();
    }
}
