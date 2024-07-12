using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.U2D.Aseprite;

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
        AddToolbar();
    }

    private void AddGraphView()
    {
        string[] guid = AssetDatabase.FindAssets("t:MapDataSO");
        string path = AssetDatabase.GUIDToAssetPath(guid[0]);
        MapDataSO mapData = AssetDatabase.LoadAssetAtPath<MapDataSO>(path);

        graphView = new MapGraphView(this, mapData);

        graphView.StretchToParentSize();

        rootVisualElement.Add(graphView);
    }

    private void AddToolbar()
    {
        Toolbar toolbar = new Toolbar();

        Button saveBtn = CreateButton("Save", Save);
        Button applyBtn = CreateButton("Apply", Apply);

        toolbar.Add(saveBtn);
        toolbar.Add(applyBtn);
        

        toolbar.style.height = 50;

        rootVisualElement.Add(toolbar);
    }

    private void Save()
    {
        graphView.Save();
    }

    private void Apply()
    {
        graphView.Apply();
    }

    private Button CreateButton(string btnName, Action onClick = null)
    {
        Button button = new Button(onClick)
        {
            text = btnName
        };

        button.style.width = 100;
        button.style.height = 50;

        return button;
    }
}
