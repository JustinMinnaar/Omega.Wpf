namespace Omega.WpfModels1.Profiling;

public class ProTemplateModel : IdNamedModel
{
    public enum Types { Page, Section, Header, Line, Footer }

    public Types Type { get; set; }
}
