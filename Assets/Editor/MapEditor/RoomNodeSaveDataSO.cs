using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RoomNodeSaveDataSO : ScriptableObject
{
    public string ID { get; set; }
    public Vector2 Position { get; set; }
    public string Group { get; set; }
}
