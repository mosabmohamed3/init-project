namespace REM.Application.Resources.Static;

public static class Enums
{
    public static string GetLocalizedName(this System.Enum Value)
    {
        var key = $"{Value.GetType().Name}.{Value}";
        return CultureHelper.GetResource(nameof(Enums), key);
    }
}
