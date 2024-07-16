using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GimmickSignalReceiver : MonoBehaviour
{
    //모든 Trigger가 On일 때 작동할 함수
    public abstract void OnAct();
    //모든 Trigger가 Off일 때 작동할 함수
    public abstract void OffAct();
}
