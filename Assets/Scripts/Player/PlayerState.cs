using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

/// <summary>
/// 플레이어의 현재 상태를 보관하는 클래스
/// 최대 체력, 현재 체력, 공격력, 식물마법 언락 유무 등을 여기서 관리함
/// </summary>
public class PlayerState : MonoBehaviour
{
    // 컴포넌트 참조
    private PlayerStateUI stateUI;

    // states
    [SerializeField] private int maxHP;
    [SerializeField] private int currentHP;
    [SerializeField] private int attackDmg;
    [SerializeField] private int[] magicLevel;    // 0이면 안배움, 1이면 기본, 2이면 업그레이드 상태

    // states getter
    public int AttackDmg { get { return attackDmg; } }

    public void Init(/*int maxHP, int attackDmg, bool[] plantMagicUnlock ...*/)
    {
        // HP, 공격력 등의 값 초기화하기
        stateUI = PlayerStateUI.Instance;
    }

    public void Heal(int amount) { }
    public void TakeDamage(int amount) { currentHP -= amount; }
    public void UpgradePlantMagic(SkillCode magicCode) { } // 획득 및 업그레이드
}
