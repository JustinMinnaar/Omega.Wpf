namespace Jem.Profiling22.Data;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ProSkipable : IdNamed
{
    public ProSkipable() : base() { }

    public ProSkipable(string name, params string[] texts)
    {
        this.Name = name;
        this.Texts.AddRange(texts);
    }

    public List<string> Texts { get; } = new();
}