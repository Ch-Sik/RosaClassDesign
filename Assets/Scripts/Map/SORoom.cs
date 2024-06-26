using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

[CreateAssetMenu(menuName = "", fileName = "")]
public class SORoom : ScriptableObject
{
    public SceneField scene;
    public string title;
    public string subTitle;

    public Vector2Int size;
    public Vector2Int offset;

    public HashSet<Vector2Int> tiles = new HashSet<Vector2Int>();
    public List<RoomPort> topPorts = new List<RoomPort>();
    public List<RoomPort> botPorts = new List<RoomPort>();
    public List<RoomPort> rigPorts = new List<RoomPort>();
    public List<RoomPort> lefPorts = new List<RoomPort>();

    #region Port Methods
    public void SetRoomPorts(List<RoomPort> top, List<RoomPort> bot, List<RoomPort> rig, List<RoomPort> lef)
    {
        topPorts = new List<RoomPort>(top);
        botPorts = new List<RoomPort>(bot);
        rigPorts = new List<RoomPort>(rig);
        lefPorts = new List<RoomPort>(lef);
    }

    public List<ConnectedPort> GetConnectedPort(PortDirection direction, int index)
    {
        switch (direction)
        {
            case PortDirection.Top:
                return new List<ConnectedPort>(topPorts[index].connectedPorts);
            case PortDirection.Bot:
                return new List<ConnectedPort>(botPorts[index].connectedPorts);
            case PortDirection.Rig:
                return new List<ConnectedPort>(rigPorts[index].connectedPorts);
            case PortDirection.Lef:
                return new List<ConnectedPort>(lefPorts[index].connectedPorts);

            default: return null;
        }
    }
    #endregion

    #region Room Methods
    public SORoom GetConnectedRoom(PortDirection direction, int index, int flag = 0)
    {
        switch (direction)
        {
            case PortDirection.Top:
                return topPorts[index].connectedPorts[flag].room;
            case PortDirection.Bot:
                return botPorts[index].connectedPorts[flag].room;
            case PortDirection.Rig:
                return rigPorts[index].connectedPorts[flag].room;
            case PortDirection.Lef:
                return lefPorts[index].connectedPorts[flag].room;

            default: return null;
        }
    }

    public List<SORoom> GetConnectedRooms()
    {
        List<SORoom> scenes = new List<SORoom>();

        foreach (RoomPort port in topPorts)
            scenes.AddRange(port.GetConnectedRoom());
        foreach (RoomPort port in botPorts)
            scenes.AddRange(port.GetConnectedRoom());
        foreach (RoomPort port in rigPorts)
            scenes.AddRange(port.GetConnectedRoom());
        foreach (RoomPort port in lefPorts)
            scenes.AddRange(port.GetConnectedRoom());

        return scenes;
    }
    #endregion
}

#region Room Port
[Serializable]
public class RoomPort
{
    public int index;
    public PortDirection direction;
    public List<Vector2Int> ports = new List<Vector2Int>();
    public List<ConnectedPort> connectedPorts = new List<ConnectedPort>();

    public RoomPort(PortDirection direction, List<Vector2Int> ports, int index)
    {
        this.direction = direction;
        this.ports = ports;
        this.index = index;
    }

    public List<SORoom> GetConnectedRoom()
    {
        List<SORoom> scenes = new List<SORoom>();

        foreach (ConnectedPort port in connectedPorts)
            scenes.Add(port.room);

        return scenes;
    }
}
#endregion

#region Connected Port
[Serializable]
public class ConnectedPort
{
    public SORoom room;
    public int index;
}
#endregion

public enum PortDirection
{
    Top,
    Bot,
    Rig,
    Lef
};