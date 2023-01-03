namespace Jem.CommonLibrary22;

public static class CoreConstants
{
    public const float MillimetersPerInch = 25.4f;

    public static readonly string[] MonthCodes =
        {"", "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

    public static readonly string[] MonthNames=
        {"", "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };

    public static readonly int MaxIdentifierLength = 20;

    /// <summary>The maximum length of text used for a person first, last or nick name, inventory item name, vehicle name, or other name.</summary>
    public static readonly int MaxNameTextLength = 50;

    public static readonly int MaxPasswordTextLength = 50;

    public static int MaxGeneralTextLength = 2000;

    public static readonly Guid GuidPublisher_Cyclops = new Guid("{f5a1e53e-c6ff-499e-acca-27b9acbcaf0c}");

    public static string MatchEmailPattern =
          @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
        + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
        + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
        + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";

    public static string Digits = "1234567890";

    public static string NumericDate = Digits + "/-";

    public static string Numeric = Digits + "R+-,.'";

    public static string LettersLowercase = "abcdefghijklmnopqrstuvwxyz";

    public static string LettersUppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    public static string Letters = CoreConstants.LettersUppercase + CoreConstants.LettersLowercase;

    public static string LettersAndDigits = CoreConstants.LettersUppercase + CoreConstants.LettersLowercase + CoreConstants.Digits;

    public static string QuoteSingle = "'";

    public static string QuoteDouble = "\"";

    public static string CommonSymbols = "/\\!@#$%&*-+=:;\"'<>,.?"; // ^ throws an error

    public static string Brackets = "{}[]()";

    public static string LettersDigitsSymbolsBrackets = LettersAndDigits + CommonSymbols + Brackets;

    public static string LettersDigitsSymbols = LettersAndDigits + CommonSymbols;
}