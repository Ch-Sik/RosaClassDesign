using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RoomNodeSaveData
{
    [field: SerializeField] public SORoom Room { get; set; }
    [field: SerializeField] public Vector2 Position { get; set; }
    [field: SerializeField] public string FlagName = "";
    [field: SerializeField] public int FlagIndex = -1;


    public RoomNodeSaveData(SORoom room, Vector2 position, string flag, int flagIndex)
    {
        Room = room;
        Position = position;
        FlagName = flag;
        FlagIndex = flagIndex;
    }
}
