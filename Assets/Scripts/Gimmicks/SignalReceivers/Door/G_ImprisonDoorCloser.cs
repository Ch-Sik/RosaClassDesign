using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G_ImprisonDoorCloser : MonoBehaviour
{
    [ShowInInspector] public G_ImprisonDoor door;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        door.Close();
    }
}
