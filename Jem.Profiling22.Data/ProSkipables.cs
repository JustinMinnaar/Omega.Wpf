namespace Jem.Profiling22.Data;

using System.Collections.Generic;

public class ProSkipables : Dictionary<string, List<string>>
{
    public void AddSkipable(string key) => AddSingleSkipable(key, key);

    public void AddSkipable(string key, params string[] values)
    {
        foreach (var value in values)
            AddSingleSkipable(key, value);
    }

    private void AddSingleSkipable(string key, string? value)
    {
        value = value?.NoSpaces();
        if (value == null) throw new EmptySkipableException($"Can't create empty skipable '{key}'!");

        if (!this.ContainsKey(key))
            this.Add(key, new() { value });
        else
            this[key].Add(value);
    }


}
