using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GimmickSignalSender : MonoBehaviour
{
    [HideInInspector] public bool isActive = false;
    private GimmickSignalConnector handler;

    //GimmickSignalHandler에 넣을 시, 이벤트 송신을 위한 Init 
    public void SetHandler(GimmickSignalConnector hander) { this.handler = hander; }

    //상태 변화 시 SendSignal을 통해 Signal 점검
    public void SendSignal()
    {
        if (handler != null)
            handler.Signal();
    }
}
