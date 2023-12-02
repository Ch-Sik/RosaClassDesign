using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class TileData : SerializedScriptableObject
{
    public TileBase[] tiles;    // 이 데이터를 적용할 타일들
    public bool magicAllowed;   // 이 타일로 구성된 지형에 식물 마법 시전이 가능할지 말지
}
