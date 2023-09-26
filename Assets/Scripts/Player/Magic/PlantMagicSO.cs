using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlantMagicSO : ScriptableObject
{
    public PlantMagicCode code;
    public Sprite icon;
    public Sprite previewSprite;
    public MagicCastType castType;
}
