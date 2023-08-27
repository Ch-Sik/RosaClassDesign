using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어의 현재 상태를 보관하는 클래스
/// 최대 체력, 현재 체력, 공격력, 식물마법 언락 유무 등을 여기서 관리함
/// </summary>
public class PlayerState : MonoBehaviour
{
    private int maxHP;
    private int attackDmg;
    private int[] magicLevel;    // 0이면 안배움, 1이면 기본, 2이면 업그레이드 상태

    private int currentHP;

    public void Init(/*int maxHP, int attackDmg, bool[] plantMagicUnlock ...*/)
    {

    }

    public void Heal(int amount) { }
    public void Damage(int amount) { }
    public void SetAttackDmg() { }
    public void UpgradePlantMagic(PlantMagicCode magicCode) { } // 획득 및 업그레이드
}
