using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateUI : MonoBehaviour
{
    // 싱글턴
    static PlayerStateUI instance;
    int currentHP;      // 체력의 증가/감소 중 어느쪽인지 판단해서 올바른 연출을 하기 위해 기존 체력값 보관

    public static PlayerStateUI GetInstance() { return instance; }

    private void Awake()
    {
        instance = this;
    }

    void UpdateUI()
    {

    }
}
