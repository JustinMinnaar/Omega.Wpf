

using Jem.Profiling22;

using System.Collections.ObjectModel;

namespace Jem.ProfilingLibrary23;

public sealed class JProProfile : IdNamed
{

    public JProProfile(ProProfile profile)
    {
        this.profile = profile;

        this.Id = profile.Id;
        this.Name = profile.Name;
        this.Tag = profile.Tag;
        this.MaxLineCountPerPage = profile.MaxLineCountPerPage;
        this.FontColor = profile.FontColor;
        this.EndOfDocumentTerminators = profile.EndOfDocumentTerminators.ToArray();
        this.AssumeAllPages = profile.AssumeAllPages;

        this.Templates = CreateTemplates(profile.Templates).ToArray();
        if (Templates.Length == 0)
            throw new FailedProfilingException($"Profile '{Name}' does not have any templates!");
    }

    private IEnumerable<JProTemplate> CreateTemplates(ObservableCollection<ProTemplate> templates)
    {
        foreach (var template in templates)
        {
            yield return new JProTemplate(template);
        }
    }

    ProProfile profile { get; }

    #region Properties

    public ProfilingTag? Tag { get; set; }
    public JProTemplate[] Templates { get; set; }
    public bool AssumeAllPages { get; set; }
    public int? MaxLineCountPerPage { get; }
    public string[] EndOfDocumentTerminators { get; set; }
    public OcrColor? FontColor { get; set; }


    #endregion Templates

    #region Identify

    public JProProfileResult? IdentifyPage(string? documentPath, CompiledOcrPage cPage)
    {
        JProProfileResult? bestResult = null;

        foreach (var template in Templates)
        {
            if (template.Type != ProfileTemplateType.Page) continue;

            var templateResult = template.IdentifyPage(cPage);
            if (templateResult == null) continue;

            if (bestResult == null || bestResult.Score < templateResult.Score)
            {
                bestResult = new JProProfileResult
                {
                    DocumentPath = documentPath,
                    PageIndex = cPage.PageIndex,
                    Profile = this,
                    Template = templateResult.Template,
                    JIdentifier = templateResult.JIdentifier,
                    Theory = templateResult.Theory,
                    Score = templateResult.Score,
                };
            }
        }

        return bestResult;
    }

    #endregion Identify
}