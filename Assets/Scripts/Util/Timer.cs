using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Timer
{
    // 필드
    float _startTime;
    public float duration { get { return Time.time - _startTime; } }

    // offset이 있을 경우 offset만큼 시간이 이미 경과한 것으로 취급함.
    public static Timer StartTimer(float offset = 0)
    {
        Timer instance = new Timer();
        instance._startTime = Time.time - offset;
        return instance;
    }

    public static Timer GetEmptyTimer()
    {
        Timer instance = new Timer();
        instance._startTime = 0;
        return instance;
    }

    public Timer Reset()
    {
        this._startTime = Time.time;
        return this;
    }
}
