using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill_0", menuName = "Inventory/Skill")]
public class SOSkill : ScriptableObject
{
    public bool isUnlock = false;
    public SkillCode skillCode;
    public Sprite skillImage;
    public string skillName;
    [TextArea(0, 10)] public string skillDescription;
}
