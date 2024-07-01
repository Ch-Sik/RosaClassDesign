using System.IO;
using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine.UIElements;


public class MapEditorWindow : EditorWindow
{
    MapGraphView graphView;

    [MenuItem("Window/Map/Map Graph")]
    public static void Open()
    {
        GetWindow<MapEditorWindow>("Map Graph");
    }

    private void OnEnable()
    {
        AddGraphView();
    }

    private void AddGraphView()
    {
        graphView = new MapGraphView(this);

        graphView.StretchToParentSize();

        rootVisualElement.Add(graphView);
    }
}
