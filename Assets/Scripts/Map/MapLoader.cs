using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Loading;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 하부의 LoadingZone에서 플레이어를 감지한 순간 로드씬을 배치하고, 언로드 씬을 제거함.
/// 로드씬을 배치할 때, 씬 매니져에게 데이터를 받아 로드되는 씬을 해당 데이터를 기준으로 재구성함.
/// 언로드씬을 제거할 때, 해당 씬에게 데이터를 받아 씬 매니져에게 업로드함.
/// </summary>

public class MapLoader : MonoBehaviour
{
    [SerializeField] private SceneField thisScene;          //맵 로더가 포함된 씬
    [SerializeField] private SceneField[] scenesToLoad;     //로드해올 씬
    [SerializeField] private SceneField[] scenesToUnload;   //언로드할 씬
    [SerializeField] private SceneData data;

    #region 씬 내의 저장해야하는 오브젝트
    [SerializeField] private List<DropItem> dropItems = new List<DropItem>();
    [SerializeField] private List<ButterflyCage> cages = new List<ButterflyCage>();
    [SerializeField] private List<bool> mobs = new List<bool>();
    [SerializeField] private List<bool> levers = new List<bool>();
    [SerializeField] private List<bool> doors = new List<bool>();
    #endregion

    GameObject player;
    private void Awake()
    {
        data.scene = thisScene;
        CollectMapData();

        //데이터 추적
        int index = MapLoadManager.Instance.HaveScene(thisScene);
        if (index == -1)
            UploadMapData();
        else
            DownloadMapData(index);
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public SceneField GetScene() { return thisScene; }

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
        MapLoader[] mapLoaders = GameObject.FindObjectsOfType<MapLoader>();
        for (int i = 0; i < scenesToUnload.Length; i++)
        {
            for (int j = 0; j < SceneManager.sceneCount; j++)
            {
                Scene loadedScene = SceneManager.GetSceneAt(j);
                if (loadedScene.name == scenesToUnload[i].SceneName)
                {
                    for (int k = 0; k < mapLoaders.Length; k++)
                        if (mapLoaders[k].GetScene().SceneName == scenesToUnload[i].SceneName)
                            mapLoaders[k].UploadMapData();
                    SceneManager.UnloadSceneAsync(scenesToUnload[i]);
                }
            }
        }
    }

    //씬을 로드해온다
    public void Loading()
    {
        SceneLoader();
        SceneUnloader();
    }

    #region 맵 저장 관련
    //변경가능성이 있는 모든 맵 데이터들을 수집한다.
    //아이템의 정렬은 같은 타입으로 묶어 리스트를 구성하고 각 리스트를 zero 벡터를 기준으로한 거리로 정렬한다 (준 Unique값)
    private void CollectMapData()
    {
        dropItems.Clear();
        DropItem[] _dropItems = GameObject.FindObjectsOfType<DropItem>();
        for (int i = 0; i < _dropItems.Length; i++)
            if (_dropItems[i].gameObject.scene.name == thisScene.SceneName)
                dropItems.Add(_dropItems[i]);
        dropItems = new List<DropItem>(dropItems.OrderBy(item => Vector2.Distance(item.transform.position, Vector2.zero)));

        cages.Clear();
        ButterflyCage[] _cages = GameObject.FindObjectsOfType<ButterflyCage>();
        for (int i = 0; i < _cages.Length; i++)
            if (_cages[i].gameObject.scene.name == thisScene.SceneName)
                cages.Add(_cages[i]);
        cages = new List<ButterflyCage>(cages.OrderBy(item => Vector2.Distance(item.transform.position, Vector2.zero)));

    }

    //Collect한 데이터들 내에 게임오브젝트들을 추적한다.
    private void TrackingMapDataState()
    {
        List<bool> dropItemsResult = new List<bool>();
        for (int i = 0; i < dropItems.Count; i++)
            dropItemsResult.Add(dropItems[i].isTrue);
        data.dropItems = new List<bool>(dropItemsResult);

        List<bool> cagesResult = new List<bool>();
        for (int i = 0; i < cages.Count; i++)
            cagesResult.Add(cages[i].isRelease);
        data.cages = new List<bool>(cagesResult);
    }

    //언로드 될 때에도 적용
    private void UploadMapData()
    {
        TrackingMapDataState();

        MapLoadManager.Instance.UploadSceneData(data);
    }

    private void DownloadMapData(int index)
    { 
        data = MapLoadManager.Instance.GetSceneData(index);

        CollectMapData();
        ApplyMapData();
    }

    //Data를 기준으로 씬의 데이터를 재구성한다.
    private void ApplyMapData()
    {
        for (int i = 0; i < dropItems.Count; i++)
            dropItems[i].isTrue = data.dropItems[i];

        for (int i = 0; i < cages.Count; i++)
        {
            cages[i].isRelease = data.cages[i];
            if (cages[i].isRelease)
                cages[i].ReleaseImmediate();
        }
    }
    #endregion
}
