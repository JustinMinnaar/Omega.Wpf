using Jem.CommonLibrary22;
using Jem.OcrLibrary22;
using Jem.ProfilingLibrary23;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jem.Profiling22.Tests
{
    [TestClass]
    public class UnitTest1
    {
        //[TestMethod]
        //public void GivenSlopedText_CanFindPhraseAndDetermineSlope()
        //{
        //    var profiler = new JEMProfiler();
        //    var profile = profiler.AddProfile(CreateSampleProfile1());
        //    var template = profile.Templates[0];

        //    for (double rotation = -20f; rotation <= 20f; rotation++)
        //    {
        //        var page = CreateSamplePage1(rotation);
        //        var result = profiler.IdentifyBestIdentifier(page, template);
        //        Assert.IsNotNull(result);
        //        Assert.AreEqual(1f, result.Score);
        //        Assert.AreSame(template.Identifiers[0].Phrases[0], result.Phrase);
        //    }

        //    //var leftSymbol = new OcrSymbol("A", new CRect(100, 100, 10, 10));
        //    //var rightSymbol = new OcrSymbol("Z", new CRect(900, 100, 10, 10));

        //    //var ocr = new CompiledOcrPage(page);

        //}


        

        //[TestMethod]
        //public async Task CanIdentifyPhraseOnPage()
        //{
        //    var samples = new BdoSamples();

        //    var oDocument = await samples.LoadOcr_Shadrack_Consent();
        //    var oPage = oDocument.Pages[0];

        //    var profile = new Profile("consent");
        //    var template = profile.AddTemplate("header", ProfileTemplateType.Fixed, oPage);
        //    var sectionMovement = new CSize(50, 50);
        //    var phrase = template.AddIdentifier(oPage, sectionMovement)
        //        .AddPhrase(100, 150, 800, 50, "Terms and Condition Consent");

        //    var profiler = new JemProfiler();
        //    profiler.Profiles.Add(profile);

        //    var source = "Shadrack/Consent";
        //    var result = profiler.IdentifyBestProfile(oDocument, oPage, source);

        //    Assert.Fail("Test not completed.");

        //}
    }
}