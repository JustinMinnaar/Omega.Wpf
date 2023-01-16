using Jem.CommonLibrary22;
using Jem.ProfilingLibrary23;

using System.Text;

namespace Jem.Profiling22;

public class JProTemplateResult : JProIdentifierResult
{
    public JProTemplateResult()
    {
    }

    public JProTemplateResult(JProTemplate jTemplate, CTheory? theory)
        : base()
    {
        Template = jTemplate;
        Theory = theory;
    }
    
    public JProTemplateResult(JProTemplate jTemplate, JProIdentifier jIdentifier, double score, CTheory? theory)
        : base(jIdentifier, score, theory) 
    {
        Template = jTemplate;
        JIdentifier = jIdentifier;
        Theory = theory;
        Score = score;
    }

    public override string ToString() => $"Template:'{Template?.Name}' {base.ToString()}";

    /// <summary>Template that was identified.</summary>
    public JProTemplate? Template { get; init; }

    /// <summary>Rows (usually 1 except for line type) extracted by this template.</summary>
    public List<RowResult> RowResults { get; set; } = new();
    
    public bool IsEndOfPage { get; set; }
    public bool ReachedEndOfDocument { get; set; }
    public bool ReachedEndOfLines { get; set; }
}