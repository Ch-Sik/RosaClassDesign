using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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



}