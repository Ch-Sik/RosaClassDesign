using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomPortObject : MonoBehaviour
{
    public PortDirection reverseDirection;      //방향의
    public List<ConnectedPort> connects;        //콘넥션들

    //이동할 방의 포트와 인덱스
    //포트의 방향이랑, connect
    public void SetRoomToPort(List<ConnectedPort> connects, PortDirection direction)
    {
        this.reverseDirection = GetOppositeDirection(direction);
        this.connects = new List<ConnectedPort>(connects);
    }

    public PortDirection GetOppositeDirection(PortDirection direction)
    {
        switch (direction)
        {
            case PortDirection.Top: return PortDirection.Bot;
            case PortDirection.Bot: return PortDirection.Top;
            case PortDirection.Rig: return PortDirection.Lef;
            case PortDirection.Lef: return PortDirection.Rig;
            default: return PortDirection.Top;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            MapManager.Instance?.Enter(reverseDirection, connects);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        { 
        }
    }
}
