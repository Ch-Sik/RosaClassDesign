using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

        Initialize();
    }

    private void Initialize()
    {
        CreateNode(Vector2.zero);
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

    private RoomNode CreateNode(Vector2 position)
    {
        RoomNode node = (RoomNode)Activator.CreateInstance(typeof(RoomNode));

        node.Initialize("", this, position);

        return node;
    }
}