using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class MapGraphView : GraphView
{
    MapEditorWindow editorWindow;
    public List<SORoom> rooms;
    public List<RoomNode> roomNodes;
    public MapDataSO mapData;


    public MapGraphView(MapEditorWindow mapEditorWindow, MapDataSO mapData)
    {
        rooms = new List<SORoom>();
        roomNodes = new List<RoomNode>();

        editorWindow = mapEditorWindow;
        this.mapData = mapData;

        AddGridBackground();

        AddManipulators();

        OnGraphViewChanged();

        Initialize();
    }

    private void OnGraphViewChanged()
    {
        graphViewChanged = (changes) =>
        {
            //엣지 연결 시
            if (changes.edgesToCreate != null)
            {
                foreach (Edge edge in changes.edgesToCreate)
                {
                    List<PortData> inPorts = GetPortDatas(edge.input);
                    List<PortData> outPorts = GetPortDatas(edge.output);

                    foreach (PortData data in outPorts)
                    {
                        if (data.port == edge.output)
                        {
                            //Debug.Log($"{data.portDirection}방향 {data.index}");
                        }
                    }

                    foreach (PortData data in inPorts)
                    {
                        if (data.port == edge.input)
                        {
                            //Debug.Log($"{data.portDirection}방향 {data.index}");
                        }
                    }
                    //Debug.Log($"{outNode.RoomName}에서 {inNode.RoomName}로 연결됌");
                }
            }

            if (changes.elementsToRemove != null)
            {
                Type edgeType = typeof(Edge);

                foreach (GraphElement element in changes.elementsToRemove)
                {
                    if (element.GetType() != edgeType)
                    {
                        continue;
                    }

                    //Edge edge = (Edge)element;
                    //DSChoiceSaveData choiceData = (DSChoiceSaveData)edge.output.userData;

                    //choiceData.NodeID = "";
                }
            }

            return changes;
        };
    }

    private List<PortData> GetPortDatas(Port port)
    {
        if (port.node is RoomNode roomNode)
        {
            return roomNode.portDatas;
        }
        else
        {
            // 부모 요소를 따라가면서 FlagGroup을 찾음
            VisualElement parent = port;
            while (parent != null)
            {
                if (parent is FlagGroup flagGroup)
                {
                    //Debug.Log("발견");
                    return flagGroup.portDatas;
                }
                parent = parent.parent;
            }
        }
        return null;
    }

    //포트 규칙 선언
    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        List<Port> compatiblePorts = new List<Port>();

        ports.ForEach(port =>
        {
            if (startPort == port)
            {
                return;
            }

            if (startPort.node is RoomNode &&
                port.node is RoomNode &&
                startPort.node == port.node)
            {
                return;
            }


            if (startPort.connections.Count() != 0 ||
                port.connections.Count() != 0)
            {
                return;
            }

            compatiblePorts.Add(port);
        });

        return compatiblePorts;
    }

    private bool isSamePorts(Port port1, Port port2)
    {
        List<PortData> data1 = GetPortDatas(port1);
        List<PortData> data2 = GetPortDatas(port2);

        if (data1.Count != data2.Count)
            return false;

        int[] ports1 = new int[4] { 0, 0, 0, 0 };
        int[] ports2 = new int[4] { 0, 0, 0, 0 };

        foreach (PortData portData in data1)
            ports1[(int)portData.portDirection]++;

        foreach (PortData portData in data2)
            ports2[(int)portData.portDirection]++;

        for (int i = 0; i < ports1.Length; i++)
            if (ports1[i] == ports2[i])
                return false;

        return true;
    }

    private void Initialize()
    {
        rooms = GetRooms();

        Load();
    }

    private void AddManipulators()
    {
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
    }

    //창을 그리드 창으로 사용
    private void AddGridBackground()
    {
        GridBackground gridBackground = new GridBackground();

        gridBackground.StretchToParentSize();

        Insert(0, gridBackground);

        AddStyle();
    }

    private void AddStyle()
    {
        StyleSheet styleSheet = (StyleSheet)EditorGUIUtility.Load("Assets/Editor/MapEditor/Styles/GridStyle.uss");

        this.styleSheets.Add(styleSheet);
    }

    private void CreateNodes()
    {
        foreach (SORoom room in rooms)
        {
            roomNodes.Add(CreateNode(room, Vector2.zero));
        }
    }

    public void LoadDatas()
    {
        List<SORoom> containedRoom = new List<SORoom>();
        List<string> containedFlag = new List<string>();

        foreach (var element in graphElements)
        {
            if (element is RoomNode node)
            {
                containedRoom.Add(node.roomData);
            }
            else if (element is FlagGroup flag)
            {
                containedFlag.Add(flag.title);
            }
        }

        foreach (var room in rooms)
        {
            if (!containedRoom.Contains(room))
                CreateNode(room, Vector2.zero);
        }

        foreach (var flag in mapData.Groups)
        {
            if (!containedFlag.Contains(flag.FlagName))
                CreateGroup(flag.FlagName, Vector2.zero);
        }
    }

    public void Save()
    {
        mapData.Clear();

        foreach (var element in graphElements)
        {
            if (element is RoomNode)
            {
                RoomNode node = (RoomNode)element;

                mapData.Nodes.Add(new RoomNodeSaveData(node.roomData,
                                                       node.GetPosition().position,
                                                       node.inGroup ? node.FlagName : "",
                                                       node.FlagIndex));
            }
            else if (element is FlagGroup)
            {
                FlagGroup group = (FlagGroup)element;

                mapData.Groups.Add(new FlagGroupSaveData(group.title,
                                                         group.GetPosition().position));

                List<SORoom> rooms = new List<SORoom>();
                foreach (var elements in group.containedElements)
                    if (elements is RoomNode node)
                    {
                        SORoom room = node.roomData;
                        room.flagName = group.title;
                        room.flagIndex = node.FlagIndex;

                        rooms.Add(room);

                        EditorUtility.SetDirty(room);
                        AssetDatabase.SaveAssets();
                    }

                mapData.Flags.Add(group.title, rooms);
            }
            else if (element is Edge)
            { 
                Edge edge = (Edge)element;

            }
        }

        EditorUtility.SetDirty(mapData);
        AssetDatabase.SaveAssets();
    }

    public void Load()
    {
        Dictionary<string, FlagGroup> groupDatas = new Dictionary<string, FlagGroup>();

        foreach (FlagGroupSaveData group in mapData.Groups)
        {
            FlagGroup newGroup = CreateGroup(group.FlagName, group.Position);

            groupDatas.Add(group.FlagName, newGroup);
        }

        foreach (RoomNodeSaveData node in mapData.Nodes)
        {
            RoomNode newNode = CreateNode(node.Room, node.Position, node.FlagIndex);

            if (node.FlagName != "")
            {
                newNode.FlagName = node.FlagName;
                newNode.FlagIndex = node.FlagIndex;
                groupDatas[node.FlagName].AddElement(newNode);
            }
        }

        foreach (var element in graphElements)
        {
            if (element is RoomNode node)
            {
                //ConnectEdge(node);
            }
        }
    }

    /*
    public Port GetPort(List<PortData> ports, PortDirection direction, int index)
    {
        PortDirection composite = PortDirection.Top;
        switch (direction)
        {
            case PortDirection.Top:
                composite = PortDirection.Bot;
                break;
            case PortDirection.Bot:
                composite = PortDirection.Top;
                break;
            case PortDirection.Rig:
                composite = PortDirection.Lef;
                break;
            case PortDirection.Lef:
                composite = PortDirection.Rig;
                break;
        }

        foreach (PortData port in ports)
        {
            if (port.portDirection == composite &&
                port.index == index)
                return port.port;
        }
    }
    */

    public void ConnectEdge(RoomNode node)
    {
        //node.GetFirstAncestorOfType<FlagGroup>();
        if (node.inGroup)
        {
        }
        else
        {
            //Edge edge = port.port.ConnectTo(GetPort(room.portDatas, port.portDirection, port));
            //node의 connectedPorts와 같은 얘들을 찾아야 함.반대의 connectedPort의 Index로 저장됨.
        }
    }

    public void Apply()
    {
        foreach (var room in rooms)
        {
            room.ClearConnectedPort();
        }

        foreach (var element in graphElements)
        {
            if (element is RoomNode)
            {
                continue;
            }
            else if (element is FlagGroup)
            {
                continue;
            }
            else if (element is Edge)
            {
                Debug.Log("Edge");
                Edge edge = (Edge)element;

                PortParent input = PortParent.None;
                PortParent output = PortParent.None;

                input = GetPortParent(edge.input);
                output = GetPortParent(edge.output);

                Debug.Log(input + " - " + output);

                if (input == PortParent.Node && output == PortParent.Node)
                    NodeToNode(edge.input, edge.output, GetRoomNodeToPort(edge.input), GetRoomNodeToPort(edge.output));
                else if (input == PortParent.Group && output == PortParent.Node)
                    NodeToGroup(edge.output, edge.input, GetRoomNodeToPort(edge.output), GetFlagGroupToPort(edge.input));
                else if (input == PortParent.Node && output == PortParent.Group)
                    NodeToGroup(edge.input, edge.output, GetRoomNodeToPort(edge.input), GetFlagGroupToPort(edge.output));
                else if (input == PortParent.Group && output == PortParent.Group)
                    GroupToGroup(edge.input, edge.output, GetFlagGroupToPort(edge.input), GetFlagGroupToPort(edge.output));
            }
        }
    }

    public PortParent GetPortParent(Port port)
    {
        VisualElement parent = port.parent;
        while (parent != null)
        {
            if (parent is FlagGroup)
            {
                return PortParent.Group;
            }
            else if (parent is RoomNode)
            {
                return PortParent.Node;
            }
            parent = parent.parent;
        }

        return PortParent.None;
    }

    public RoomNode GetRoomNodeToPort(Port port)
    {
        VisualElement parent = port.parent;
        while (parent != null)
        {
            if (parent is RoomNode)
            {
                return (RoomNode)parent;
            }
            parent = parent.parent;
        }

        return null;
    }

    public FlagGroup GetFlagGroupToPort(Port port)
    {
        VisualElement parent = port.parent;
        while (parent != null)
        {
            if (parent is FlagGroup)
            {
                return (FlagGroup)parent;
            }
            parent = parent.parent;
        }

        return null;
    }

    public void NodeToNode(Port input, Port output, RoomNode inputNode, RoomNode outputNode)
    {
        //포트 찾기
        PortData a = null;
        PortData b = null;

        foreach (var t in inputNode.portDatas)
            if (t.port == input)
            {
                Debug.Log("Node a 발견");
                a = t;

                Debug.Log($"{a.index}");
            }

        foreach (var t in outputNode.portDatas)
            if (t.port == output)
            {
                Debug.Log("Node b 발견");

                Debug.Log($"{b.index}");
                b = t;
            }

        Debug.Log(inputNode.RoomName + "과 " + outputNode.RoomName + "연결 됨");

        ConnectPortData(a, b);
    }

    public void ConnectPortData(PortData a, PortData b, int aindex = 0, int bindex = 0)
    {
        List<ConnectedPort> portsA = GetPort(a); bool containsA = false;
        List<ConnectedPort> portsB = GetPort(b); bool containsB = false;

        if (!containsA)
            portsA.Add(new ConnectedPort(b.room.scene, bindex));
        if (!containsB)
            portsB.Add(new ConnectedPort(a.room.scene, aindex));

        EditorUtility.SetDirty(a.room);
        EditorUtility.SetDirty(b.room);

        AssetDatabase.SaveAssets();
    }

    public List<ConnectedPort> GetPort(PortData a)
    {
        switch (a.portDirection)
        {
            case PortDirection.Top:
                return a.room.topPorts[a.index].connectedPorts;
            case PortDirection.Bot:
                return a.room.botPorts[a.index].connectedPorts;
            case PortDirection.Rig:
                return a.room.rigPorts[a.index].connectedPorts;
            case PortDirection.Lef:
                return a.room.lefPorts[a.index].connectedPorts;
        }

        return null;
    }

    public void NodeToGroup(Port input, Port output, RoomNode node, FlagGroup group)
    {
        PortData a = null;
        PortData b = null;
        List<RoomNode> c = new List<RoomNode>();

        foreach (var t in node.portDatas)
            if (t.port == input)
            {
                Debug.Log("Node a 발견");
                a = t;
            }

        foreach (var t in group.portDatas)
            if (t.port == output)
            {
                Debug.Log("Group b 발견");
                b = t;
            }

        Debug.Log(node.RoomName + "과 " + group.FlagName + "연결 됨");

        foreach (var t in group.containedElements)
        {
            if (t is RoomNode roomNode)
            {
                c.Add(roomNode);
            }
        }

        foreach (var i in c)
        {
            PortData data = new PortData(null, i.roomData, b.portDirection, a.index);
            ConnectPortData(a, data, 0, a.index);
        }
    }

    public void GroupToGroup(Port input, Port output, FlagGroup group1, FlagGroup group2)
    {
        PortData a = null;
        PortData b = null;
        List<RoomNode> c = new List<RoomNode>();
        List<RoomNode> d = new List<RoomNode>();

        foreach (var t in group1.portDatas)
            if (t.port == input)
            {
                Debug.Log("Group a 발견");
                a = t;
            }

        foreach (var t in group2.portDatas)
            if (t.port == output)
            {
                Debug.Log("Group b 발견");
                b = t;
            }

        foreach (var t in group1.containedElements)
        {
            if (t is RoomNode roomNode)
            {
                c.Add(roomNode);
            }
        }

        foreach (var t in group2.containedElements)
        {
            if (t is RoomNode roomNode)
            {
                d.Add(roomNode);
            }
        }

        Debug.Log(group1.FlagName + "과 " + group2.FlagName + "연결 됨");

        foreach (var i in c)
        {
            PortData data1 = new PortData(null, i.roomData, a.portDirection, a.index);
            foreach (var j in d)
            {
                PortData data2 = new PortData(null, j.roomData, b.portDirection, b.index);
                ConnectPortData(data1, data2, a.index, b.index);
            }
        }
    }

    private FlagGroup CreateGroup(string name, Vector2 position)
    {
        FlagGroup group = new FlagGroup();

        group.Initialize(name, this, position);

        return group;
    }

    private RoomNode CreateNode(SORoom room, Vector2 position, int flagIndex = 0)
    {
        RoomNode node = (RoomNode)Activator.CreateInstance(typeof(RoomNode));

        node.Initialize(room, this, position, flagIndex);

        return node;
    }

    private List<SORoom> GetRooms()
    {
        string[] guids = AssetDatabase.FindAssets("t:SORoom");
        List<SORoom> rooms = new List<SORoom>();

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            rooms.Add(AssetDatabase.LoadAssetAtPath<SORoom>(path));
        }

        return rooms;
    }

    public void DisconnectAllPorts(Port port)
    {
        var connections = port.connections.ToList();

        foreach (var connection in connections)
        {
            connection.input.Disconnect(connection);
            connection.output.Disconnect(connection);
            RemoveElement(connection);
        }
    }

    public enum PortParent
    {
        None,
        Node,
        Group
    }
}