using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRoomManager : MonoBehaviour
{
    [SerializeField] Blackboard bossBlackboard;
    [SerializeField] AudioClip[] bgmClip;       // 각 페이즈 별 브금 클립
    
    BGMPlayer bgmPlayer;


    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(bossBlackboard != null);
        bossBlackboard.OnBlackboardUpdated += OnBossBlackboardUpdated;
    }

    // 보스의 상태가 변화되었을 때 호출.
    void OnBossBlackboardUpdated(string key, object value)
    {
        switch(key)
        {
            case BBK.Enemy:
                if ((GameObject)value != null)
                {
                    OnPlayerEnteredBossRoom();
                }
                break;
            case BBK.CurrentPhase:
                OnBossPhaseChanged((int)value);
                break;
            case BBK.isDead:
                OnBossDead();
                break;
            default: 
                // 아무것도 안함
                break;
        }
    }

    void OnPlayerEnteredBossRoom()
    {
        if(bgmPlayer == null)
        {
            bgmPlayer = Camera.main.gameObject.GetComponentInChildren<BGMPlayer>();
        }
        bgmPlayer.PlayBGM(bgmClip[0]);
    }

    void OnBossPhaseChanged(int phase)
    {
        if(phase < bgmClip.Length && bgmClip[phase] != null)
        {
            bgmPlayer.PlayBGM(bgmClip[phase]);
        }
    }

    void OnBossDead()
    {
        bgmPlayer.PlayDefaultBGM();
    }

    [Button("테스트: 보스 즉시 사망")]
    void Test_KillBossImmediatly()
    {
        bossBlackboard.gameObject.GetComponent<MonsterDamageReceiver>().GetHitt(999, 0);
    }
}
