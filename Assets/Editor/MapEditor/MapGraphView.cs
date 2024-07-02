using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class MapGraphView : GraphView
{
    MapEditorWindow editorWindow;

    public MapGraphView(MapEditorWindow mapEditorWindow)
    {
        editorWindow = mapEditorWindow;

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
                    RoomNode outNode = (RoomNode)edge.output.node;
                    RoomNode inNode = (RoomNode)edge.input.node;

                    foreach (PortData data in outNode.portDatas)
                    {
                        if (data.port == edge.output)
                        {
                            Debug.Log($"{data.portDirection}방향 {data.index}");
                        }
                    }

                    foreach (PortData data in inNode.portDatas)
                    {
                        if (data.port == edge.input)
                        {
                            Debug.Log($"{data.portDirection}방향 {data.index}");
                        }
                    }
                    Debug.Log($"{outNode.RoomName}에서 {inNode.RoomName}로 연결됌");
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

            if (startPort.node == port.node)
            {
                return;
            }

            /*
            if (startPort.direction == port.direction)
            {
                return;
            }
            */

            compatiblePorts.Add(port);
        });

        return compatiblePorts;
    }

    private void Initialize()
    {
        CreateNode("Room1", Vector2.zero);
        CreateNode("Room2", Vector2.zero);
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

    private RoomNode CreateNode(string name, Vector2 position)
    {
        RoomNode node = (RoomNode)Activator.CreateInstance(typeof(RoomNode));

        node.Initialize(name, this, position);

        return node;
    }
}