using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill_0", menuName = "Skill/Move Ability")]
public class SO_Skill : ScriptableObject
{
    /// TODO: isUnlock 필드 삭제
    public bool isUnlock = false;
    public SkillCode skillCode;
    public Sprite icon;
    public string skillName;
    [TextArea(0, 10)] public string description;
}
