using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FlagSO", fileName = "GameFlag")]
public class FlagSO : ScriptableObject
{
    public List<Flag> flags = new List<Flag>();
}

[Serializable]
public class Flag
{
    public string flag;
    public int value = 0;

    public Flag(string flag, int flagValue)
    {
        this.flag = flag;
        this.value = flagValue;
    }
}
