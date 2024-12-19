using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Savable : MonoBehaviour
{
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
}
