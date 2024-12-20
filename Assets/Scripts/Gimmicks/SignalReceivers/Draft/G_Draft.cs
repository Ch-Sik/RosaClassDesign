using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G_Draft : GimmickSignalReceiver
{
    public bool isActivated = false;
    public float risingPower = 3.0f;
    public GameObject[] particles;

    private void Start()
    {
        if(isActivated)
            ToggleParticles(true);
        else
            ToggleParticles(false);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!isActivated)
            return;
        if (!collision.CompareTag("Player"))
            return;

        PlayerRef.Instance.movement.Rising(risingPower);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!isActivated)
            return;
        if (!collision.CompareTag("Player"))
            return;

        PlayerRef.Instance.movement.CancleRising();
    }

    public override void OnAct()
    {
        isActivated = true;
        ToggleParticles(true);
    }

    public override void OffAct()
    {
        isActivated = false;
        ToggleParticles(false);
        // TODO: 파티클 자연스럽게 멈추도록 수정하기
    }

    private void ToggleParticles(bool value)
    {
        for(int i=0; i<particles.Length; i++)
        {
            particles[i].SetActive(value);
        }
    }

    public override void ImmediateOnAct()
    {
        isActivated = true;
        ToggleParticles(true);
    }

    public override void ImmediateOffAct()
    {
        isActivated = false;
        ToggleParticles(false);
    }
}
