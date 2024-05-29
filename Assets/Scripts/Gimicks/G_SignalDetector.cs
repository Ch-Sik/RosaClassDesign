using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Signal에 대응하여 모든 기믹에 대한 움직임을 통제함.
/// </summary>
/// 
public class G_SignalHandler : MonoBehaviour
{
    public List<G_Lever> levers = new List<G_Lever>();
    public List<G_PressurePlate> plates = new List<G_PressurePlate>();

    public List<G_Lazer> lazers = new List<G_Lazer>();
    public List<MovePlatform> platforms = new List<MovePlatform>(); 

    public bool AllisActive = false;
    public bool curActive = false;

    private void Start()
    {
        for(int i = 0; i < platforms.Count; i++)
            platforms[i].enabled = false;
    }

    private void Update()
    {
        curActive = isAllTriggerActive();
        if (AllisActive != curActive)
        {
            Debug.Log("On Gimick");
            AllisActive = curActive;
            SetGimicks(AllisActive);
        }
    }

    private void SetGimicks(bool val)
    {
        if (val)
        {
            for (int i = 0; i < lazers.Count; i++)
                lazers[i].InactivateLazer();

            for (int i = 0; i < platforms.Count; i++)
            {
                Debug.Log("True");
                platforms[i].enabled = true;
            }
        }
        else
        {
            for (int i = 0; i < lazers.Count; i++)
                lazers[i].ActivateLazer();

            for (int i = 0; i < platforms.Count; i++)
            {
                Debug.Log("False");
                platforms[i].enabled = false;
            }
        }
    }

    private bool isAllTriggerActive()
    {
        int targetCount = levers.Count + plates.Count;
        int count = 0;

        for (int i = 0; i < levers.Count; i++)
            if (levers[i].isActive)
                count++;
        for (int i = 0; i < plates.Count; i++)
            if (plates[i].isActive)
                count++;

        if (targetCount == count)
            return true;

        return false;
    }
}
