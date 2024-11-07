using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Communication_0", menuName = "CommunicationData")]
public class CommunicationSO : ScriptableObject
{
    public int ID;
    public string textFileName;
    public List<CommunicationData> data = new List<CommunicationData>();
}
