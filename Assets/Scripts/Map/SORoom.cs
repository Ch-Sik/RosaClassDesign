using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "", fileName = "")]
public class SORoom : ScriptableObject
{
    public SceneField scene;
    public string flagName;
    public int flagIndex;
    public string title;
    public string subTitle;

    public Vector2Int size;
    public Vector2Int offset;

    public HashSet<Vector2Int> tiles = new HashSet<Vector2Int>();
    public List<RoomPort> topPorts = new List<RoomPort>();
    public List<RoomPort> botPorts = new List<RoomPort>();
    public List<RoomPort> rigPorts = new List<RoomPort>();
    public List<RoomPort> lefPorts = new List<RoomPort>();

    public void SetRoomPort(List<RoomPort> top, List<RoomPort> bot, List<RoomPort> rig, List<RoomPort> lef)
    {
        topPorts = new List<RoomPort>(top);
        botPorts = new List<RoomPort>(bot);
        rigPorts = new List<RoomPort>(rig);
        lefPorts = new List<RoomPort>(lef);
    }

    public void ClearConnectedPort()
    {
        foreach (RoomPort port in topPorts) { port.connectedPorts = new List<ConnectedPort>(); }
        foreach (RoomPort port in botPorts) { port.connectedPorts = new List<ConnectedPort>(); }
        foreach (RoomPort port in rigPorts) { port.connectedPorts = new List<ConnectedPort>(); }
        foreach (RoomPort port in lefPorts) { port.connectedPorts = new List<ConnectedPort>(); }
    }
}

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
}

[Serializable]
public class ConnectedPort
{
    public string flag;
    public int flagIndex;
    public SceneField scene;
    public int index;

    public ConnectedPort(SceneField scene, int index)
    {
        this.scene = scene;
        this.index = index;
    }
}

public enum PortDirection
{
    Top,
    Bot,
    Rig,
    Lef
};