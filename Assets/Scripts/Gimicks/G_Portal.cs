using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G_Portal : MonoBehaviour
{
    public G_Portal_Sector pos1;
    public G_Portal_Sector pos2;

    private Transform player;

    public void SetEvent(int id, Transform player)
    {
        this.player = player;
        RemoveAllInteraction();

        if (id == 0)
            Imp_Interaction.Instance.onInteraction.AddListener(MoveToPos1);
        else
            Imp_Interaction.Instance.onInteraction.AddListener(MoveToPos2);
    }

    public void MoveToPos1()
    {
        player.position = pos2.transform.position;
    }

    public void MoveToPos2()
    {
        player.position = pos1.transform.position;
    }

    public void RemoveAllInteraction()
    {
        Imp_Interaction.Instance.onInteraction.RemoveAllListeners();
    }
}