namespace Jem.CommonLibrary22;

public static class CoreNicknames
{
    /// <summary>
    ///     Used to reflect a friendly class name for use in messages to users, xml and json files.
    ///     Use the [Nickname(name)] attribute or override with a friendly name.
    ///     If not specified, the class' actual name is returned.
    /// </summary>
    public static string ToNickname(this Type type, bool inherit = false)
    {
        foreach (var attr in type.GetCustomAttributes(inherit))
        {
            if (attr is NicknameAttribute zattr) return zattr.Nickname;
        }
        return type.Name;
    }

    /// <summary>
    ///     Used to reflect a friendly class name for use in messages to users, xml and json files.
    ///     Use the [Nickname(name)] attribute or override with a friendly name.
    ///     If not specified, the class' actual name is returned.
    /// </summary>
    public static string ToNickname(this object obj, bool inherit = false)
    {
        return ToNickname(obj.GetType(), inherit);
    }
}