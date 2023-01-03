namespace Jem.CommonLibrary22;

using System;

/// <summary>
///     Provides a name that overrides the class, property or field name, for use in definitions, xml files, etc.
///     The <seealso cref="ToNickname()" /> extension function returns the nickname.
/// </summary>
public class NicknameAttribute : Attribute
{
    public NicknameAttribute(string nickname)
    {
        Nickname = nickname;
    }

    public string Nickname { get; set; }
}