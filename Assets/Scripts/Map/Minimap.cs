using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    public float defaultScale = 5;

    public GameObject minimapUI;                //미니맵 UI
    public GameObject minimapSector;            //미니맵의 각 맵 파츠
    public GameObject minimapPanel;             //미니맵 섹터를 자식으로 가질 패널

    [Button]
    public void OnOffMinimap()
    { 
        minimapUI.SetActive(!minimapUI.activeSelf);

        if (minimapUI.activeSelf)
        {
            //On
            List<SO_SceneMapData> loadedSceneMapData = new List<SO_SceneMapData>(MapLoadManager.Instance.GetLoadedSceneMapData());

            for(int i = 0; i < loadedSceneMapData.Count; i++)
                AddSector(loadedSceneMapData[i]);
        }
        else
        {
            //Off
            Clear();
        }
    }

    public void Clear()
    {
        for (int i = 0; i < minimapPanel.transform.childCount; i++)
            Destroy(minimapPanel.transform.GetChild(minimapPanel.transform.childCount - 1 - i));
    }

    public void AddSector(SO_SceneMapData data)
    {
        GameObject sector = Instantiate(minimapSector);
        sector.transform.SetParent(minimapPanel.transform);
        sector.GetComponent<MiniMapSector>().SetMapSector(data, defaultScale);
    }
}
