using UnityEngine;

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
    public static LR opposite(this LR dir)
    {
        if (dir == LR.LEFT)
            return LR.RIGHT;
        else
            return LR.LEFT;
    }

    public static Vector2 toVector2(this LR dir)
    {
        return dir == LR.LEFT ? Vector2.left : Vector2.right;
    }

    public static LR toLR(this Vector3 v)
    {
        return v.x < 0 ? LR.LEFT : LR.RIGHT;
    }

    public static LR toLR(this Vector2 v)
    {
        return v.x < 0 ? LR.LEFT : LR.RIGHT;
    }
}