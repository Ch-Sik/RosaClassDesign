using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapData", menuName = "MapData")]
public class MapDataSO : ScriptableObject
{
    [field: SerializeField] [ShowInInspector] public Dictionary<string, List<SORoom>> Flags;
    [field: SerializeField] public List<FlagGroupSaveData> Groups { get; set; }
    [field: SerializeField] public List<RoomNodeSaveData> Nodes { get; set; }
    [field: SerializeField] public List<NNEdgeSaveData> NNEdges { get; set; }
    [field: SerializeField] public List<NGEdgeSaveData> NGEdges { get; set; }
    [field: SerializeField] public List<GGEdgeSaveData> GGEdges { get; set; }



    public void Clear()
    {
        Flags = new Dictionary<string, List<SORoom>>();
        Groups = new List<FlagGroupSaveData>();
        Nodes = new List<RoomNodeSaveData>();

        NNEdges = new List<NNEdgeSaveData>();
        NGEdges = new List<NGEdgeSaveData>();
        GGEdges = new List<GGEdgeSaveData>();
    }
}

public class NNEdgeSaveData
{
    public SORoom port1;
    public PortDirection direction1;
    public int index1;

    public SORoom port2;
    public PortDirection direction2;
    public int index2;
}

public class NGEdgeSaveData
{
    public SORoom port1;
    public PortDirection direction1;
    public int index1;

    public string port2;
    public PortDirection direction2;
    public int index2;
}

public class GGEdgeSaveData
{
    public string port1;
    public PortDirection direction1;
    public int index1;

    public string port2;
    public PortDirection direction2;
    public int index2;
}