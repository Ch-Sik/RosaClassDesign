using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hittable : MonoBehaviour
{
    public virtual void GetHitt(int damage, float attackAngle)
    {
        Debug.LogWarning("virtual method NOT OVERRIDED!\n");
    }
}
