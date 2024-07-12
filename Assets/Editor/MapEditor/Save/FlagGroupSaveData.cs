using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FlagGroupSaveData
{
    [field: SerializeField] public string FlagName { get; set; }
    [field: SerializeField] public Vector2 Position { get; set; }

    public FlagGroupSaveData(string flagName, Vector2 position)
    {
        FlagName = flagName;
        Position = position;
    }
}
