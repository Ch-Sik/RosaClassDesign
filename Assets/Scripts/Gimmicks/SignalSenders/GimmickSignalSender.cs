using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GimmickSignalSender : MonoBehaviour
{
    public bool isActive = false;
    private GimmickSignalConnector handler;

    private void Awake()
    {
        tag = "Sender";
    }

    //GimmickSignalHandler에 넣을 시, 이벤트 송신을 위한 Init 
    public void SetHandler(GimmickSignalConnector hander) { this.handler = hander; }

    //상태 변화 시 SendSignal을 통해 Signal 점검
    public void SendSignal()
    {
        if (handler != null)
            handler.Signal();
    }

    public void ImmediateSendSignal()
    {
        Debug.Log("전송");
        if (handler != null)
            handler.ImmediateSignal();
    }

    // 0 은 기본 상태, 그 외는 내부에서 상태를 정의함. 
    private int state = 0;

    public int GetState()
    {
        return state;
    }

    public void SetState(int state)
    {
        this.state = state;
        //이외 상태에 따른 변경
    }

    public abstract void Init(int state);
}
