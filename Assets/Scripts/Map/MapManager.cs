using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MapManager : MonoBehaviour
{
    // 싱글톤
    private static MapManager instance;

    public static MapManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<MapManager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject();
                    instance = obj.AddComponent<MapManager>();
                }
            }
            return instance;
        }
    }

    [Button]
    public void OpenScene(SORoom roomData)
    {
        StartCoroutine(OpenScene(roomData, Vector2Int.zero));
    }

    //동기화를 위한 코루틴 사용
    public IEnumerator OpenScene(SORoom roomData, Vector2Int anchor)
    {
        SceneField scene = roomData.scene;
        if (!SceneManager.GetSceneByName(scene.SceneName).isLoaded)
        {
            //비동기 로드
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene.SceneName, LoadSceneMode.Additive);

            //로드 완료될 때까지 대기
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }

        Scene sce = SceneManager.GetSceneByName(scene.SceneName);
        GameObject[] objects = sce.GetRootGameObjects();
        foreach (GameObject obj in objects)
            obj.GetComponent<Room>()?.Init();

        LoadScene();
    }

    [Button]
    public void CloseScene(SORoom scenea)
    {
        SaveScene();

        SceneField scene = scenea.scene;

        SceneManager.UnloadSceneAsync(scene);
    }

    public void SaveScene()
    { 
    }

    public void LoadScene()
    {
    }
}