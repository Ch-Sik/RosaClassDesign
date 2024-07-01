using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.UIElements;
using PlasticPipe.PlasticProtocol.Messages;

public class RoomNode : Node
{
    public string ID { get; set; }
    public string RoomName { get; set; }
    public SORoom roomData { get; set; }

    public Vector2 position { get; set; }

    private MapGraphView graphView { get; set; }

    public void Initialize(string nodeName, MapGraphView graphView, Vector2 position)
    {
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
        VisualElement cenElement = new VisualElement(); cenElement.SetSize(240, 240); midElement.Add(cenElement);  cenElement.SetVisualElementStyle(FlexDirection.Column);  cenElement.SetRoomName("Room1");
        VisualElement botElement = new VisualElement(); botElement.SetSize(240, 80); midElement.Add(botElement);

        lefElement.SetVisualElementStyle(FlexDirection.Column);
        rigElement.SetVisualElementStyle(FlexDirection.Column);
        topElement.SetVisualElementStyle(FlexDirection.Row);
        botElement.SetVisualElementStyle(FlexDirection.Row);

        lefElement.SetPorts(2, Orientation.Horizontal, Direction.Input);
        rigElement.SetPorts(2, Orientation.Horizontal, Direction.Output);
        topElement.SetPorts(2, Orientation.Vertical, Direction.Input);
        botElement.SetPorts(2, Orientation.Vertical, Direction.Output);

        mainContainer.Add(lefElement);
        mainContainer.Add(midElement);
        mainContainer.Add(rigElement);

        RefreshExpandedState();
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

    public static void SetPorts(this VisualElement visualElement, int count, Orientation orientation, Direction direction)
    {
        for (int i = 0; i < count; i++)
        {
            var port = Port.Create<Edge>(orientation, direction, Port.Capacity.Single, typeof(bool));
            port.portName = "";
            port.Q<Label>("type").visible = false;
            port.Q<Label>("type").SetEnabled(false);
            visualElement.Add(port);
        }
    }

    public static void SetSize(this VisualElement visualElement, int width, int height)
    {
        visualElement.style.width = new Length(width, LengthUnit.Pixel);
        visualElement.style.height = new Length(height, LengthUnit.Pixel);
    }
}
