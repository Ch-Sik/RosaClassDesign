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
}