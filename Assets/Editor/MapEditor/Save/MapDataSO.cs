using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapData", menuName = "MapData")]
public class MapDataSO : ScriptableObject
{
    [field: SerializeField] public List<FlagGroupSaveData> Groups { get; set; }
    [field: SerializeField] public List<RoomNodeSaveData> Nodes { get; set; }

    public void Clear()
    {
        Groups = new List<FlagGroupSaveData>();
        Nodes = new List<RoomNodeSaveData>();
    }
}