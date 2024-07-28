using Sirenix.Utilities;
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
    Dictionary<string, FlagGroup> groupDatas;


    public MapGraphView(MapEditorWindow mapEditorWindow, MapDataSO mapData)
    {
        rooms = new List<SORoom>();
        roomNodes = new List<RoomNode>();
        groupDatas = new Dictionary<string, FlagGroup>();

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
        groupDatas = new Dictionary<string, FlagGroup>();

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

        ConnectEdges();
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


    public void ConnectEdges()
    {
        List<RoomLoadDataSet> sets = new List<RoomLoadDataSet>(mapData.GetRoomLoadDataSet());

        foreach (RoomLoadDataSet set in sets)
        {
            //셋이 그룹인 경우
            if (set.isGroup)
            {
                //내 그룹
                FlagGroup group = groupDatas[set.flag];

                int rigIndex = 0;
                foreach(var rig in set.room.rigPorts)
                {
                    if (rig.connectedPorts.IsNullOrEmpty())
                        continue;

                    if (rig.connectedPorts.Count < 2)
                    {
                        Debug.Log("그룹-노드");
                        RoomNode _node = GetNode(rig.connectedPorts[0].scene);
                        Connect(group.GetPort(PortDirection.Rig, rigIndex),
                                _node.GetPort(PortDirection.Lef, rig.connectedPorts[0].index));
                    }
                    else
                    {
                        Debug.Log("그룹-그룹");
                        //대상 그룹 찾기
                        FlagGroup _group = GetGroup(rig.connectedPorts[0].scene.SceneName);
                        Connect(group.GetPort(PortDirection.Rig, rigIndex),
                                _group.GetPort(PortDirection.Lef, rig.connectedPorts[0].index));
                        Debug.Log($"{group.FlagName} {_group.FlagName}");
                    }
                    rigIndex++;
                }

                int botIndex = 0;
                foreach (var bot in set.room.botPorts)
                {
                    if (bot.connectedPorts.IsNullOrEmpty())
                        continue;

                    if (bot.connectedPorts.Count < 2)
                    {
                        Debug.Log("그룹-노드");
                        RoomNode _node = GetNode(bot.connectedPorts[0].scene);
                        Connect(group.GetPort(PortDirection.Bot, botIndex),
                                _node.GetPort(PortDirection.Top, bot.connectedPorts[0].index));
                    }
                    else
                    {
                        Debug.Log("그룹-그룹");
                        FlagGroup _group = GetGroup(bot.connectedPorts[0].scene.SceneName);
                        Connect(group.GetPort(PortDirection.Bot, botIndex),
                                _group.GetPort(PortDirection.Top, bot.connectedPorts[0].index));
                    }
                    botIndex++;
                }
            }
            //셋이 노드인 경우
            else
            {
                RoomNode node = GetNode(set.room.scene);

                int rigIndex = 0;
                foreach (var rig in set.room.rigPorts)
                {
                    if (rig.connectedPorts.IsNullOrEmpty())
                        continue;

                    if (rig.connectedPorts.Count < 2)
                    {
                        Debug.Log("노드-노드");
                        RoomNode _node = GetNode(rig.connectedPorts[0].scene);
                        if (_node == null)  //
                            continue;
                        Connect(node.GetPort(PortDirection.Rig, rigIndex),
                                _node.GetPort(PortDirection.Lef, rig.connectedPorts[0].index));
                    }
                    else
                    {
                        Debug.Log("노드-그룹");
                        FlagGroup _group = GetGroup(rig.connectedPorts[0].scene.SceneName);
                        if (_group == null) //
                            continue;
                        Connect(node.GetPort(PortDirection.Rig, rigIndex),
                                _group.GetPort(PortDirection.Lef, rig.connectedPorts[0].index));
                    }
                    rigIndex++;
                }

                int botIndex = 0;
                foreach (var bot in set.room.botPorts)
                {
                    if (bot.connectedPorts.IsNullOrEmpty())
                        continue;

                    if (bot.connectedPorts.Count < 2)
                    {
                        Debug.Log("노드-노드");
                        RoomNode _node = GetNode(bot.connectedPorts[0].scene);
                        Debug.Log("길이 : " + roomNodes.Count);
                        if (_node == null)
                            continue;
                        Connect(node.GetPort(PortDirection.Bot, botIndex),
                                _node.GetPort(PortDirection.Top, bot.connectedPorts[0].index));
                    }
                    else
                    {
                        Debug.Log("노드-그룹");
                        FlagGroup _group = GetGroup(bot.connectedPorts[0].scene.SceneName);
                        if (_group == null)
                            continue;
                        Connect(node.GetPort(PortDirection.Bot, botIndex),
                                _group.GetPort(PortDirection.Top, bot.connectedPorts[0].index));
                    }
                    botIndex++;
                }
            }
        }
    }

    public void Connect(Port output, Port input)
    {
        // 엣지 생성
        var edge = new Edge
        {
            output = output,
            input = input
        };

        // 엣지 연결
        edge.output.Connect(edge);
        edge.input.Connect(edge);

        // 그래프 뷰에 엣지 추가
        this.AddElement(edge);
    }

    public RoomNode GetNode(SceneField scene)
    {
        foreach (var node in roomNodes)
        {
            if (node.roomData.scene.SceneName == scene.SceneName)
            {
                return node;
            }
        }

        return null;
    }

    public FlagGroup GetGroup(string SceneName)
    {
        foreach (var node in roomNodes)
        {
            if (!node.inGroup)
                continue;

            if (node.roomData.scene.SceneName == SceneName)
                return groupDatas[node.FlagName];
        }

        return null;
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
                Edge edge = (Edge)element;

                PortParent input = PortParent.None;
                PortParent output = PortParent.None;

                input = GetPortParent(edge.input);
                output = GetPortParent(edge.output);

                //Debug.Log(input + " - " + output);

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

        mapData.Rooms = new List<SORoom>(rooms);

        EditorUtility.SetDirty(mapData);
        AssetDatabase.SaveAssets();
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
                a = t;
                //a.PrintPortData();
            }

        foreach (var t in outputNode.portDatas)
            if (t.port == output)
            {
                b = t;
                //b.PrintPortData();
            }

        ConnectPortData(a, b, "", 0, "", 0);
    }

                                //노드      //그룹      //노드 인덱스     //그룹 인덱스
    public void ConnectPortData(PortData a, PortData b, string aFlag, int aIndex, string bFlag, int bIndex) //플래그명과 인덱스를 모두 받아야 함.
    {
        a.PrintPortData();
        b.PrintPortData();

        RoomPort portA = GetPort(a);
        RoomPort portB = GetPort(b);

        List<ConnectedPort> portsA = GetConnectedPort(a);// bool containsA = false;
        List<ConnectedPort> portsB = GetConnectedPort(b);// bool containsB = false;

        //포트가 노드인지 그룹인지 확인 필요
        //if (!containsA)
        if (String.IsNullOrWhiteSpace(bFlag))
        {
            portA.flag = "";
            portsA.Add(new ConnectedPort(b.room.scene, b.index));
        }
        else
        {
            portA.flag = bFlag;
            portsA.Add(new ConnectedPort(b.room.scene, b.index, bFlag, bIndex));
        }
        //if (!containsB)
        if (String.IsNullOrWhiteSpace(aFlag))
        {
            portB.flag = "";
            portsB.Add(new ConnectedPort(a.room.scene, a.index));
        }
        else
        {
            portB.flag = aFlag;
            portsB.Add(new ConnectedPort(a.room.scene, b.index, aFlag, aIndex));
        }

        EditorUtility.SetDirty(a.room);
        EditorUtility.SetDirty(b.room);

        AssetDatabase.SaveAssets();
    }

    public RoomPort GetPort(PortData a)
    {
        switch (a.portDirection)
        {
            case PortDirection.Top:
                return a.room.topPorts[a.index];
            case PortDirection.Bot:
                return a.room.botPorts[a.index];
            case PortDirection.Rig:
                return a.room.rigPorts[a.index];
            case PortDirection.Lef:
                return a.room.lefPorts[a.index];
        }

        return null;
    }

    public List<ConnectedPort> GetConnectedPort(PortData a)
    {
        a.PrintPortData();

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
        PortData a = null;                          //노드의 포트
        PortData b = null;                          //그룹의 포트
        List<RoomNode> c = new List<RoomNode>();    //그룹의 룸 데이터

        foreach (var t in node.portDatas)
            if (t.port == input)
            {
                a = t;
            }

        foreach (var t in group.portDatas)
            if (t.port == output)
            {
                b = t;
            }

        //그룹 데이터를 C에 옮김.
        foreach (var t in group.containedElements)
        {
            if (t is RoomNode roomNode)
            {
                c.Add(roomNode);
            }
        }

        //C에 있는 모든 룸 순회
        foreach (var i in c)
        {
            PortData data = new PortData(null, i.roomData, b.portDirection, b.index);       //a.index
            ConnectPortData(a, data, "", 0, group.FlagName, i.FlagIndex);                       //0
            Debug.Log(i.FlagIndex);
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
                a = t;
            }

        foreach (var t in group2.portDatas)
            if (t.port == output)
            {
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

        foreach (var i in c)
        {
            PortData data1 = new PortData(null, i.roomData, a.portDirection, a.index);
            foreach (var j in d)
            {
                PortData data2 = new PortData(null, j.roomData, b.portDirection, b.index);
                Debug.Log("i : " + i.FlagIndex + ", j : " + j.FlagIndex);
                ConnectPortData(data1, data2, group1.FlagName, i.FlagIndex,  group2.FlagName, j.FlagIndex);
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

        roomNodes.Add(node);

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