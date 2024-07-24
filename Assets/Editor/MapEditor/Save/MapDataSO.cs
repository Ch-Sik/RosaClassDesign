using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapData", menuName = "MapData")]
public class MapDataSO : ScriptableObject
{
    [field: SerializeField] [ShowInInspector] public Dictionary<string, List<SORoom>> Flags;
    [field: SerializeField] public List<FlagGroupSaveData> Groups { get; set; }
    [field: SerializeField] public List<RoomNodeSaveData> Nodes { get; set; }

    public List<RoomLoadDataSet> GetRoomLoadDataSet()
    {
        List<RoomLoadDataSet> datas = new List<RoomLoadDataSet>();

        foreach (var data in Nodes)
        {
            if (data.FlagName == "")
                datas.Add(new RoomLoadDataSet(data.Room));
            else
            {
                List<SORoom> rooms = new List<SORoom>();
                bool isContained = false;
                foreach (var a in datas)
                {
                    if (a.flag == data.FlagName)
                    {
                        isContained = true;
                        break;
                    }
                }
                if (isContained)
                    continue;


                foreach (var node in Nodes)
                {
                    if (data.FlagName == node.FlagName)
                        rooms.Add(node.Room);
                }

                datas.Add(new RoomLoadDataSet(data.FlagName,
                                              rooms));
            }
        }

        return datas;
    }

    public void Clear()
    {
        Flags = new Dictionary<string, List<SORoom>>();
        Groups = new List<FlagGroupSaveData>();
        Nodes = new List<RoomNodeSaveData>();
    }
}

public class RoomLoadDataSet
{
    public bool isGroup = false;
    public string flag = "";
    public SORoom room;
    public List<SORoom> rooms;

    public RoomLoadDataSet(SORoom room)
    {
        this.room = room;
    }
    public RoomLoadDataSet(string flag, List<SORoom> rooms)
    {
        isGroup = true;
        this.flag = flag;
        this.room = rooms[0];
        this.rooms = new List<SORoom>(rooms);
    }
    public bool IsContain(string SceneName)
    {
        if (rooms.IsNullOrEmpty())
            return false;

        foreach (var room in rooms)
        {
            if (room.name == SceneName)
                return true;
        }

        return false;
    }
}