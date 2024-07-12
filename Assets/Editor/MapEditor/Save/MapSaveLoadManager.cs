using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class MapSaveLoadManager
{
    private static MapGraphView graphView;

    private static MapDataSO mapData;
    private static List<RoomNode> nodes;
    private static List<FlagGroup> groups;

    public static void Initialize(MapGraphView _graphView)
    {
        graphView = _graphView;

        nodes = new List<RoomNode>();
        groups = new List<FlagGroup>();

        string[] guid = AssetDatabase.FindAssets("t:MapDataSO");
        string path = AssetDatabase.GUIDToAssetPath(guid[0]);
        mapData = AssetDatabase.LoadAssetAtPath<MapDataSO>(path);
    }

    public static void Save()
    {
        
    }

    public static void Load()
    {
        
    }
}
