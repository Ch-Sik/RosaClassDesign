using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 저장해야하만 하는 정보
/// 1. 드랍아이템
/// 2. 나비장
/// 3. 몬스터
/// 4. 문
/// 5. 작동장치
/// P.S. 굳이 이놈들을 리스트를 따로따로 할 이유가 있을까? 있긴함.
/// </summary>

[Serializable]
public class SceneData
{
    public SceneField scene;

    public SO_SceneMapData data;

    public List<bool> dropItems = new List<bool>();
    public List<bool> cages = new List<bool>();
    public List<bool> mobs = new List<bool>();
    public List<bool> levers = new List<bool>();
    public List<bool> doors = new List<bool>();
}
