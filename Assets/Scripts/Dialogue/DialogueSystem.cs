using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// CSV를 파싱떠서 데이터를 가공하기 위한 스크립트
/// </summary>

public class DialogueSystem : MonoBehaviour
{

}

public class DialogueData
{
    public int id;
    public D_Type type;
    public D_Position pos;
    public D_Char cha;
    public D_Expression expr;
    public string name;
    public string text;

    public DialogueData(int id, string type, string pos, string cha, string expr, string name, string text)
    {
        
    }
}

public enum D_Type      //DialogueType
{
    Show,
    Hide,
    Line,
}

public enum D_Position  //DialoguePosition
{
    Left,
    Center,
    Right
}

public enum D_Char      //DialogueCharacter
{
    Null,
    Char1,
    Char2,
    Char3
}

public enum D_Expression    //DialogueExpression
{
    Default,
    Happy,
    Sad,
    Mad,
    Surprise,
    ETC1,
    ETC2
}