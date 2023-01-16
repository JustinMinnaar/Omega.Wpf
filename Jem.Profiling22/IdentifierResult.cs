using Jem.CommonLibrary22;

namespace Jem.Profiling22;

public class IdentifierResult
{
    public IdentifierResult(double score, CTheory theory, ProfilePhrase phrase)
    {
        Score = score;
        Theory = theory;
        Phrase = phrase;
    }

    public double Score { get; set; }
    public CTheory Theory { get; set; }
    public ProfilePhrase Phrase { get; set; }

    //public SearchResult(
    //    double score, CTheory theory,
    //    ProfileTemplate? profileTemplate = null,
    //    ProfileIdentifierTemplate? identifierTemplate = null,
    //    ProfileExtractor? extractorTemplate = null)
    //{
    //    Score = score;
    //    Theory = theory ?? throw new ArgumentNullException(nameof(theory));
    //    ProfileTemplate = profileTemplate;
    //    IdentifierTemplate = identifierTemplate;
    //    ExtractorTemplate = extractorTemplate;
    //}

    //public Profile? Profile { get; set; }
    //public ProfileTemplate? ProfileTemplate { get; set; }
    //public ProfileIdentifierTemplate? IdentifierTemplate { get; set; }
    //public ProfileExtractor? ExtractorTemplate { get; set; }
}

