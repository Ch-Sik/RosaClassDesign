using Codice.Client.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class FlagGroup : Group
{
    public string ID { get; set; }
    public string FlagName { get; set; }
    public List<SORoom> roomDatas { get; set; }

    public Vector2 position { get; set; }

    private MapGraphView graphView { get; set; }

    public List<PortData> portDatas;
    public int[] portCount = new int[4] { 0, 0, 0, 0 };

    public VisualElement topElement;
    public VisualElement botElement;
    public VisualElement lefElement;
    public VisualElement rigElement;

    public void Initialize(string flagName, MapGraphView graphView, Vector2 position)
    {
        FlagName = flagName;
        this.graphView = graphView;
        this.position = position;

        graphView.AddElement(this);

        SetPosition(new Rect(position, Vector2.zero));

        roomDatas = new List<SORoom>();
        portDatas = new List<PortData>();

        this.title = FlagName;

        this.style.fontSize = 25;

        var titleLabel = this.Q<Label>();
        titleLabel.style.fontSize = 25;

        style.flexDirection = FlexDirection.Row;
        style.alignContent = Align.Center;
        style.justifyContent = Justify.Center;

        autoUpdateGeometry = true;

        CreateSurroundingElements();
    }

    public void Draw(SORoom roomData)
    {
        lefElement.SetVisualElementStyle(FlexDirection.ColumnReverse);
        rigElement.SetVisualElementStyle(FlexDirection.ColumnReverse);
        topElement.SetVisualElementStyle(FlexDirection.Row);
        botElement.SetVisualElementStyle(FlexDirection.Row);

        SetPorts(lefElement, roomData.lefPorts.Count, Orientation.Horizontal, Direction.Input, PortDirection.Lef);
        SetPorts(rigElement, roomData.rigPorts.Count, Orientation.Horizontal, Direction.Output, PortDirection.Rig);
        SetPorts(topElement, roomData.topPorts.Count, Orientation.Vertical, Direction.Input, PortDirection.Top);
        SetPorts(botElement, roomData.botPorts.Count, Orientation.Vertical, Direction.Output, PortDirection.Bot);
    }

    private void CreateSurroundingElements()
    {
        topElement = new VisualElement
        {
            style =
            {
                height = 25,
                position = Position.Absolute,
                top = 30,
                left = 50,
                right = 50
            }
        }; Add(topElement);

        botElement = new VisualElement
        {
            style =
            {
                height = 25,
                position = Position.Absolute,
                bottom = 0,
                left = 50,
                right = 50
            }
        }; Add(botElement);

        lefElement = new VisualElement
        {
            style =
            {
                width = 40,
                position = Position.Absolute,
                top = 50,
                bottom = 50,
                left = 0
            }
        }; Add(lefElement);

        rigElement = new VisualElement
        {
            style =
            {
                width = 40,
                position = Position.Absolute,
                top = 50,
                bottom = 50,
                right = 0
            }
        }; Add(rigElement);
    }

    public void SetPorts(VisualElement visualElement, int count, Orientation orientation, Direction direction, PortDirection portDirection)
    {
        for (int i = 0; i < count; i++)
        {
            Port port = Port.Create<Edge>(orientation, direction, Port.Capacity.Single, typeof(bool));
            port.portName = "";
            visualElement.Add(port);
            portDatas.Add(new PortData(port, null, portDirection, i));
        }
    }

    protected override void OnElementsAdded(IEnumerable<GraphElement> elements)
    {
        base.OnElementsAdded(elements);

        //한 번에 여러 번 넣기 불가
        if (elements.Count() > 1)
            RemoveElements(elements);

        //처음이라면, 셋업
        if (containedElements.Count() == 1)
           Setup(((RoomNode)elements.First()).roomData);
        
        //들어온 모든 노드들의 속성 변경
        foreach (GraphElement element in elements)
        {
            RoomNode node = ((RoomNode)element);

            if (node.roomData.topPorts.Count == portCount[0] &&
                node.roomData.botPorts.Count == portCount[1] &&
                node.roomData.rigPorts.Count == portCount[2] &&
                node.roomData.lefPorts.Count == portCount[3])
            {
                node.SetGroupState(true, title);
                roomDatas.Add(node.roomData);
            }
            else
                RemoveElement(node);
        }
    }

    protected override void OnElementsRemoved(IEnumerable<GraphElement> elements)
    {
        base.OnElementsRemoved(elements);

        //나가는 모든 노드들의 속성 변경
        foreach (GraphElement element in elements)
        {
            RoomNode node = ((RoomNode)element);
            node.SetGroupState(false);

            roomDatas.Remove(node?.roomData);
        }

        //아무것도 없다면, 리셋
        if (containedElements.Count() == 0)
            Reset();
    }

    protected override void OnGroupRenamed(string oldName, string newName)
    {
        base.OnGroupRenamed(oldName, newName);
        OnTitleChanged(newName);
    }

    private void OnTitleChanged(string newTitle)
    {
        FlagName = newTitle;

        foreach (var element in this.containedElements)
        {
            if (element is RoomNode)
            {
                RoomNode node = (RoomNode)element;
                node.FlagName = newTitle;
            }
        }
    }

    private void Setup(SORoom room)
    {
        portCount = new int[4] { room.topPorts.Count,
                                 room.botPorts.Count,
                                 room.rigPorts.Count,
                                 room.lefPorts.Count };
        Draw(room);
    }

    private void Reset()
    {
        portCount = new int[4] { 0, 0, 0, 0 };

        foreach (PortData portData in portDatas)
            graphView.DisconnectAllPorts(portData.port);

        roomDatas = new List<SORoom>();
        portDatas = new List<PortData>();

        topElement.Clear();
        botElement.Clear();
        lefElement.Clear();
        rigElement.Clear();
    }

    public Port GetPort(PortDirection direction, int index)
    {
        foreach (PortData port in portDatas)
        {
            if (port.portDirection == direction &&
                port.index == index)
            {
                return port.port;
            }
        }

        return null;
    }
}
