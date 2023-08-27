using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PlayerRef 클래스는 GameObject 최상단에서 존재하며 단순히 Player 관련 클래스에 대한 레퍼런스를 모아놓은 클래스임
/// </summary>
public class PlayerRef : MonoBehaviour
{
    static PlayerRef instance;

    public PlayerState State;
    public PlayerMove Move;
    public PlayerMagic Magic;
    public PlayerAnimation Animation;
    public PlayerSpawn LastSpawned;


    private void Awake()
    {
        if(instance == null)
            instance = this;
        if(State == null) State = GetComponent<PlayerState>();
        if(Move == null) Move = GetComponent<PlayerMove>();
        if(Magic == null) Magic = GetComponent<PlayerMagic>();
        if(Animation == null) Animation = GetComponent<PlayerAnimation>();
    }
}
