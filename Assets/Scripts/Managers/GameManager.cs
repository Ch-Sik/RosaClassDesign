using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // 싱글톤
    static GameManager instance;
    public static GameManager GetInstance() { return instance; }

    private void Awake()
    {
        instance = this;
    }

}
