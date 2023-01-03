using System.Collections.ObjectModel;

namespace Jem.Profiling22.Data;

/// <summary>Represents a single type of document, and contains page templates for extraction of data from its pages.</summary>
public class ProProfile : IdNamed
{
    public ProProfile(){}
    public ProProfile(string name) : base(name) { }

    public List<string> PhrasesToIgnore { get; set; } = new();

    public ProfilingTag? Tag { get; set; } 

    public bool AssumeAllPages { get; set; }

    /// <summary>If true, a page count field exists and can be used to validate all pages have been found.</summary>
    public bool HasPageCount { get; set; }

    /// <summary>If specified, only extract text matching this colour.</summary>
    public OcrColor? FontColor { get; set; }
        
    /// <summary>Does this profile identify information about the folder, such as the project, employee, customer, supplier, etc.</summary>
    public bool IsFolderTemplate { get; set; }

    public ObservableCollection<string> NamesOfStampsToIgnore { get; set; } = new();

    public Dictionary<string, string> TextFixes { get; set; } = new();

    //public float? LinesTop { get; set; }
    //public float? LinesBottom { get; set; }

    public int? MaxLineCountPerPage { get; set; }

    public ObservableCollection<string> SkipLinesContaining { get; set; } = new();

    public ObservableCollection<string> EndOfDocumentTerminators { get; set; } = new();

    public ProProfile AddEndOfDocumentTerminators(params string[] terminators)
    {
        foreach(var terminator in terminators)
            EndOfDocumentTerminators.Add(terminator);
        return this;
    }

    #region Templates

    public ObservableCollection<ProTemplate> Templates { get; set; } = new();

    public Guid? SelectedTemplateId { get; set; }

    public ProTemplate? SelectedTemplate
    {
        get => Templates.FirstOrDefault(a => a.Id == SelectedTemplateId);
        set => SelectedTemplateId = value?.Id;
    }

    public   int LastTemplateNumberAdded { get; set; }
    

    public void AddTemplate()
    {
        LastTemplateNumberAdded++;

        var template = new ProTemplate("Template #" + LastTemplateNumberAdded);
        Templates.Add(template);

        SelectedTemplateId = template.Id;
    }

    public ProTemplate AddTemplate(string name, ProfileTemplateType type, OcrPage? sourcePage = null, CSize? maxMovement = null)
    {
        var aTemplate = new ProTemplate(this, name, sourcePage, type, maxMovement);
        AddTemplate(aTemplate);
        return aTemplate;
    }

    public T AddTemplate<T>(T aTemplate) where T : ProTemplate
    {
        Templates.Add(aTemplate);
        aTemplate.Profile = this;
        return aTemplate;
    }

    public void Compile()
    {
        foreach (var template in Templates)
        {
            // If the section template is relative to another template, we adjust its 
            // rectangles to handle when the other template is missing.
            if (template.Type == ProfileTemplateType.Page) continue;

            var relatedTemplate = template.RelativeTo;
            while (relatedTemplate != null)
            {
                var distance = relatedTemplate.LinesHeight;
                if (distance != null)
                    template.Move(0, -distance.Value);
                relatedTemplate = relatedTemplate.RelativeTo;
            }
            template.Compile();

            //var distance = template.Rect.Top - template.RelativeTo.Rect.Top;
            //template.Move(0, -distance);
        }
    }

    #endregion
}