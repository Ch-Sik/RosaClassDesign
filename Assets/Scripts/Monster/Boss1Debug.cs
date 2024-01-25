using Panda;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Boss1Debug : MonoBehaviour
{
    public TMP_Text text;
    public AITask_BossPhaseChange phaseChanger;
    public MonsterState monsterState;

    // Update is called once per frame
    [Task]
    void Update()
    {
        text.text = $"Phase: {phaseChanger.currentPhase + 1}\n"
            + $"HP: {monsterState.HP}\n";
    }
}
