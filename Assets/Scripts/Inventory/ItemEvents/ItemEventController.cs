using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEventController : MonoBehaviour
{
    public GameObject sortPanel;
    public GameObject eventPanel;

    public void AddItem(SOItem item, int quantity)
    {
        GameObject panel = Instantiate(eventPanel);
        panel.transform.SetParent(sortPanel.transform);
        ItemEventPanel ep = panel.GetComponent<ItemEventPanel>();
        ep.SetData(item, quantity);
        ep.PlayEvent();
    }

    public void UnlockSkill(SOSkill skill)
    {
        GameObject panel = Instantiate(eventPanel);
        panel.transform.SetParent(sortPanel.transform);
        ItemEventPanel ep = panel.GetComponent<ItemEventPanel>();
        ep.SetData(skill);
        ep.PlayEvent();
    }
}
