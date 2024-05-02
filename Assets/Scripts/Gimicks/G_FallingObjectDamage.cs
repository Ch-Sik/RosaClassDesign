using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G_FallingObjectDamage : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Damage");
    }
}
