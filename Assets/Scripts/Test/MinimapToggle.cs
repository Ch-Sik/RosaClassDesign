using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapToggle : MonoBehaviour
{
    [SerializeField] GameObject Minimap;

    public void Toggle(bool value)
    {
        Minimap.SetActive(value);
    }
}
