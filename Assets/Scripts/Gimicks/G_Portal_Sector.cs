using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G_Portal_Sector : MonoBehaviour
{
    public G_Portal portal;
    public bool canUse;
    public int id = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Player")
            return;

        canUse = true;
        portal.SetEvent(id, collision.transform);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Player")
            return;

        canUse = false;
        portal.RemoveAllInteraction();
    }
}
