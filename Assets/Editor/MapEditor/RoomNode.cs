using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.UIElements;

public class RoomNode : Node
{
    public string ID { get; set; }
    public string RoomName { get; set; }
    public SORoom roomData { get; set; }

    public Vector2 position { get; set; }

    private MapGraphView graphView { get; set; }

    public List<PortData> portDatas;

    public void Initialize(string nodeName, MapGraphView graphView, Vector2 position)
    {
        portDatas = new List<PortData>();

        this.RoomName = nodeName;
        this.graphView = graphView;
        this.position = position;
        //this.roomData = room;

        Draw();

        graphView.AddElement(this);
    }

    public void Draw()
    {
        SetPosition(new Rect(position, Vector2.zero));

        mainContainer.style.borderTopLeftRadius = 0;
        mainContainer.style.borderTopRightRadius = 0;
        mainContainer.style.borderBottomLeftRadius = 0;
        mainContainer.style.borderBottomRightRadius = 0;

        mainContainer.SetSize(400, 400);
        mainContainer.style.backgroundColor = Color.black;

        titleContainer.RemoveFromHierarchy();
        inputContainer.RemoveFromHierarchy();
        outputContainer.RemoveFromHierarchy();

        mainContainer.SetVisualElementStyle(FlexDirection.Row);

        VisualElement lefElement = new VisualElement(); lefElement.SetSize(80, 400);
        VisualElement midElement = new VisualElement(); midElement.SetSize(240, 400);
        VisualElement rigElement = new VisualElement(); rigElement.SetSize(80, 400);

        VisualElement topElement = new VisualElement(); topElement.SetSize(240, 80); midElement.Add(topElement);
        VisualElement cenElement = new VisualElement(); cenElement.SetSize(240, 240); midElement.Add(cenElement);  cenElement.SetVisualElementStyle(FlexDirection.Column);  cenElement.SetRoomName(RoomName);
        VisualElement botElement = new VisualElement(); botElement.SetSize(240, 80); midElement.Add(botElement);

        lefElement.SetVisualElementStyle(FlexDirection.Column);
        rigElement.SetVisualElementStyle(FlexDirection.Column);
        topElement.SetVisualElementStyle(FlexDirection.Row);
        botElement.SetVisualElementStyle(FlexDirection.Row);

        SetPorts(lefElement, 2, Orientation.Horizontal, Direction.Input, PortDirection.Lef);
        SetPorts(rigElement, 2, Orientation.Horizontal, Direction.Output, PortDirection.Rig);
        SetPorts(topElement, 2, Orientation.Vertical, Direction.Input, PortDirection.Top);
        SetPorts(botElement, 2, Orientation.Vertical, Direction.Output, PortDirection.Bot);

        mainContainer.Add(lefElement);
        mainContainer.Add(midElement);
        mainContainer.Add(rigElement);

        RefreshExpandedState();
    }

    public void SetPorts(VisualElement visualElement, int count, Orientation orientation, Direction direction, PortDirection portDirection)
    {
        for (int i = 0; i < count; i++)
        {
            Port port = Port.Create<Edge>(orientation, direction, Port.Capacity.Multi, typeof(bool));
            port.portName = "";
            //port.Q<Label>("type").visible = false;
            //port.Q<Label>("type").SetEnabled(false);
            visualElement.Add(port);
            portDatas.Add(new PortData(port, roomData, portDirection, i));

            Debug.Log($"{portDirection} 방향, {i}인덱스");
        }
    }

    public string GetRoomName()
    {
        return RoomName;
    }
}

public static class VisualElementExtensions
{
    public static void SetRoomName(this VisualElement visualElement, string name)
    { 
        Label label = new Label(name);
        label.style.fontSize = 30;
        label.style.alignSelf = Align.Center;
        visualElement.Add(label);
        label.SetVisualElementStyle(FlexDirection.Row);
    }

    public static void SetVisualElementStyle(this VisualElement visualElement, FlexDirection direction, Align align = Align.Center, Justify justify = Justify.Center)
    {
        visualElement.style.flexDirection = direction;
        visualElement.style.alignContent = align;
        visualElement.style.justifyContent = justify;
    }

    public static void SetSize(this VisualElement visualElement, int width, int height)
    {
        visualElement.style.width = new Length(width, LengthUnit.Pixel);
        visualElement.style.height = new Length(height, LengthUnit.Pixel);
    }
}

public class PortData
{
    public Port port;
    public SORoom room;
    public PortDirection portDirection;
    public int index;

    public PortData(Port port, SORoom room, PortDirection portDirection, int index)
    {
        this.port = port;
        this.room = room;
        this.portDirection = portDirection;
        this.index = index;
    }
}