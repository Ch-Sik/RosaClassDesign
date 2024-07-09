using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;

public class Task_CooltimeCheck : MonoBehaviour
{
    // 마지막으로 해당 스킬이 사용된 시간을 기록
    Dictionary<string, Timer> timers;

    // 쿨타임이 다 지났다면 Succeed, 남았다면 Fail
    [Task]
    private bool CheckCooldown(string moveName, float cooldown)
    {
        if(timers == null)
            timers = new Dictionary<string, Timer>();

        // 해당 스킬의 사용기록이 남아있을 경우
        if(timers.ContainsKey(moveName))
        {
            // 쿨타임 지났다면 Succeed
            if(timers[moveName].duration > cooldown)
            {
                timers[moveName].Reset();   // 마지막 사용기록을 갱신
                return true;
            }
            // 쿨타임 안지났다면 Fail
            else
            {
                ThisTask.debugInfo = $"left Cooltime:{cooldown - timers[moveName].duration}";
                return false;
            }
        }
        // 해당 스킬의 사용기록이 남아있지 않을 경우 = 쿨타임 지났을리 없음
        else
        {
            // 무조건 Succeed
            timers.Add(moveName, Timer.StartTimer());
            return true;
        }
    }

    // '마지막 시전 이후 지난 시간'을 강제로 지정된 값으로 설정
    [Task]
    private bool ResetCooldown(string moveName, float t)
    {
        if(timers == null)
            timers = new Dictionary<string, Timer>();

        // 해당 스킬의 사용기록이 남아있을 경우
        if (timers.ContainsKey(moveName))
        {
            timers[moveName] = Timer.StartTimer(t);
        }
        // 해당 스킬의 사용기록이 남아있지 않을 경우 신규 등록
        else
        {
            timers.Add(moveName, Timer.StartTimer(t));
        }
        // 무조건 성공하는 Task
        return true;
    }
}
