using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomPortObject : MonoBehaviour
{
    public Room room;
    public PortDirection direction;
    public int index;

    public void SetRoomToPort(Room room, PortDirection direction, int index)
    {
        this.room = room;
        this.direction = direction;
        this.index = index;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
            room.Enter(direction, index);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
            room.Exit(direction, index);
    }
}
