/// <summary>
/// 플레이어와 관련한 상수 모음
/// </summary>

public enum PlayerMoveState
{
    DEFAULT, 
    CLIMBING,
    SUPERDASH_READY, // 스쿼팅 오이를 타고 발사되기 직전 상태
    SUPERDASH,      // 스쿼팅 오이를 타고 실제 발사된 상태
    NO_MOVE, // 피격 등으로 인해 일시적으로 움직일 수 없는 상태
}

public enum PlayerActionState
{
    DEFAULT,
    MAGIC_READY,
    NO_ACTION,  // 대화 등으로 인해 공격이 불가능한 상태
}

public enum PlayerCubeActionState
{
    DEFAULT,           //플레이어가 큐브를 집지 않은 상태
    GRAB,           //플레이어가 큐브를 집은 상태
    PUSH,           //플레이어가 큐브를 집고 미는 상태
    PULL            //플레이어가 큐브를 집고 당기는 상태
}

/// Util/Enums.cs로 옮겨짐.
/*
public enum LR { LEFT, RIGHT }

public static class EnumExtensions
{
    public static bool isLEFT(this LR dir)
    {
        return dir == LR.LEFT;
    }
    public static bool isRIGHT(this LR dir)
    {
        return dir == LR.RIGHT;
    }
}
*/