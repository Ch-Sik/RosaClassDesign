using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SceneMapData_", menuName = "SceneMapData")]
public class SO_SceneMapData : ScriptableObject
{
    public List<Vector2Int> tiles = new List<Vector2Int>();
    public Vector2 anchor;
    public Vector2Int size;

    public SceneField scene;
}
