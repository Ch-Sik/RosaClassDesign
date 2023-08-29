using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어와 관련한 여러가지 상수 모음
/// </summary>

public enum PlantMagicCode  // 더 좋은 이름 없을지 고민중
{

}

public enum PlayerMoveState
{
    WALK,
    CLIMB,
    MONKEY, // 천장에 매달린 상태. 더 적절한 이름이 있으면 바꿀 것
    CANNOTMOVE, // 피격 등으로 인해 일시적으로 움직일 수 없는 상태
}