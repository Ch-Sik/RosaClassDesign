using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomPortObject : MonoBehaviour
{
    public Room room;

    public void SetRoomToPort(Room room) { this.room = room; }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
            room.Enter();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
            room.Exit();
    }
}
