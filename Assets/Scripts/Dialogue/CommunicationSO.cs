using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

[CreateAssetMenu(fileName = "Communication_0", menuName = "CommunicationData")]
public class CommunicationSO : ScriptableObject
{
    public int ID;
    public string textFileName;
    [TableList(AlwaysExpanded = true, MinScrollViewHeight = 500)]
    public List<CommunicationData> data = new List<CommunicationData>();

    [Button]
    public void FillText()
    {
        //CSV 파싱 및 텍스트 파일 추출
        List<Dictionary<string, object>> CSV = CSVReader.Read("Dialogue", textFileName);
        string key = "KOR";
        int textIndex = 0;
        for (int i = 0; i < data.Count; i++)
        {
            if (data[i].type == CommunicationType.TargetText ||
                data[i].type == CommunicationType.PlayerText)
            {
                data[i].text = CSV[textIndex][key].ToString();
                textIndex++;
            }
        }
        if(CSV.Count != textIndex)
        {
            Debug.LogWarning("데이터의 text 항목이 적어 텍스트를 모두 옮기지 못함");
        }
    }
}
