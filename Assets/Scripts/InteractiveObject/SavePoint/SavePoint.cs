using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour
{
    [SerializeField] private int healAmount = 5;


    public void Save()
    {
        OnSave();
    }

    private void OnSave()
    {
        PlayerRef.Instance.state.Heal(healAmount);
    }
}
