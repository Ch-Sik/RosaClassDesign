using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class RoomNode : Node
{
    public string ID { get; set; }
    public string FlagName { get; set; }
    public int FlagIndex { get; set; }
    public string RoomName { get; set; }
    public SORoom roomData { get; set; }

    public Vector2 position { get; set; }

    private MapGraphView graphView { get; set; }

    public List<PortData> portDatas;

    public bool inGroup = false;

    private TextField intInputField;

    public void Initialize(SORoom room, MapGraphView graphView, Vector2 position, int flagIndex)
    {
        portDatas = new List<PortData>();

        this.roomData = room;
        this.RoomName = room.name;
        this.graphView = graphView;
        this.position = position;
        this.FlagIndex = flagIndex;
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

        VisualElement topElement = new VisualElement(); topElement.SetSize(240, 20); midElement.Add(topElement);
        VisualElement cenElement = new VisualElement(); cenElement.SetSize(240, 360); midElement.Add(cenElement);  cenElement.SetVisualElementStyle(FlexDirection.Column);  cenElement.SetRoomName(RoomName);
        VisualElement botElement = new VisualElement(); botElement.SetSize(240, 20); midElement.Add(botElement);

        intInputField = new TextField("Flag Index :");
        intInputField.value = FlagIndex.ToString();
        intInputField.labelElement.style.fontSize = 25;
        intInputField.style.width = 250;
        intInputField.style.height = 35;
        intInputField.style.marginBottom = 10;
        intInputField.style.fontSize = 25;
        intInputField.RegisterValueChangedCallback(evt =>
        {
            if (int.TryParse(evt.newValue, out int result))
                FlagIndex = result;

        });
        cenElement.Add(intInputField);
        DisableInputField();

        lefElement.SetVisualElementStyle(FlexDirection.ColumnReverse);
        rigElement.SetVisualElementStyle(FlexDirection.ColumnReverse);
        topElement.SetVisualElementStyle(FlexDirection.Row);
        botElement.SetVisualElementStyle(FlexDirection.Row);

        SetPorts(lefElement, roomData.lefPorts.Count, Orientation.Horizontal, Direction.Input, PortDirection.Lef);
        SetPorts(rigElement, roomData.rigPorts.Count, Orientation.Horizontal, Direction.Output, PortDirection.Rig);
        SetPorts(topElement, roomData.topPorts.Count, Orientation.Vertical, Direction.Input, PortDirection.Top);
        SetPorts(botElement, roomData.botPorts.Count, Orientation.Vertical, Direction.Output, PortDirection.Bot);

        mainContainer.Add(lefElement);
        mainContainer.Add(midElement);
        mainContainer.Add(rigElement);

        RefreshExpandedState();
    }

    public void SetPorts(VisualElement visualElement, int count, Orientation orientation, Direction direction, PortDirection portDirection)
    {
        for (int i = 0; i < count; i++)
        {
            Port port = Port.Create<Edge>(orientation, direction, Port.Capacity.Single, typeof(bool));
            port.portName = "";
            visualElement.Add(port);
            portDatas.Add(new PortData(port, roomData, portDirection, i));
        }
    }

    public void SetGroupState(bool isGrouping, string str = "")
    {
        inGroup = isGrouping;
        FlagName = str;

        if (inGroup)
        {
            DisablePorts();
            EnableInputField();
        }
        else
        {
            EnablePorts();
            DisableInputField();
        }
    }

    private void EnablePorts()
    {
        foreach (PortData portData in portDatas)
        {
            portData.port.SetEnabled(true);
            portData.port.visible = true;
        }
    }

    private void DisablePorts()
    {
        foreach (PortData portData in portDatas)
        {
            graphView.DisconnectAllPorts(portData.port);
            portData.port.SetEnabled(false);
            portData.port.visible = false;
        }
    }

    private void EnableInputField()
    {
        intInputField.SetEnabled(true);
        intInputField.visible = true;
    }

    private void DisableInputField()
    {
        intInputField.SetEnabled(false);
        intInputField.visible = false;
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

    public void PrintPortData()
    {
        //Debug.Log($"{room.name}의 {portDirection}의 {index}번");
    }
}