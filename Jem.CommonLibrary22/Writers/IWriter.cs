namespace Jem.CommonLibrary22;

public interface IWriter
{
    void ClearLine();

    void Write(char character) => Write(new String(character, 1));
    void Write(string message);

    void WriteLine() => WriteLine("");
    void WriteLine(char character) => WriteLine(new String(character, 1));
    void WriteLine(string? message = null);

    void WriteLine(Exception? ex);
}