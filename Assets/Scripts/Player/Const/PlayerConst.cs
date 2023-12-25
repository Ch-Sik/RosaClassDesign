/// <summary>
/// 플레이어와 관련한 상수 모음
/// </summary>

public enum PlayerMoveState
{
    GROUNDED, 
    MIDAIR,
    WALK,
    CLIMBING,
    SUPERDASH_READY, // 스쿼팅 오이를 타고 발사되기 직전 상태
    SUPERDASH,      // 스쿼팅 오이를 타고 실제 발사된 상태
    CANNOTMOVE, // 피격 등으로 인해 일시적으로 움직일 수 없는 상태
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