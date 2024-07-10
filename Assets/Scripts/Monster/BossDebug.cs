using Panda;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BossDebug : MonoBehaviour
{
    public TMP_Text text;
    public Task_BossPhaseChange phaseChanger;
    public GameObject boss;
    public MonsterState monsterState;

    private void Start()
    {
        monsterState = boss.GetComponent<MonsterState>();
    }

    // Update is called once per frame
    void Update()
    {
        if (boss != null)
        {
            text.text = $"Phase: {phaseChanger.currentPhase + 1}\n"
                + $"HP: {monsterState.HP}\n";
        }
        else
        {
            text.text = "No Boss";
        }
    }
}
