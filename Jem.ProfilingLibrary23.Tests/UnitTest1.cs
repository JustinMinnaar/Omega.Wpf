//using Jem.CommonLibrary22;
//using Jem.OcrLibrary22;
//using Jem.Profiling22.Data;

//using Org.BouncyCastle.Crypto.Tls;

//using System.Runtime;

//namespace Jem.ProfilingLibrary23.Tests;

//[TestClass]
//public class UnitTest1
//{
//    [TestMethod]
//    public async Task GivenOcrPage_CanBuildAndLocateStencil()
//    {        
//        var samples = new BdoSamples();
//        var oDocument = await samples.LoadOcr(EmployeeName.Hartzenberg, "Fnb05", "62644878368");
//        var oPage0 = oDocument.Pages[0];

//        var pFnb = new ProProfile(name: "Fnb");
//        var tPage = pFnb.AddTemplate("Page", ProfileTemplateType.Page, oPage0);
//        CSize partialMovement = new(10,10);
//        tPage.AddIdentifier(oPage0, partialMovement)
//            .AddPhrase(new CRect(447, 2795, 462, 20));

//        var profiler = new OptimisedProfiler();
//        profiler.AddProfile(pFnb);
        
//        var pResult = profiler.IdentifyPage(oPage0);
//        Assert.IsNotNull(pResult);
//        Assert.AreEqual(pFnb.Id, pResult.ProfileResult?.Id);
//    }

//    // var fnbProfile1 = profiler.AddProfile(new JProfile("Fnb"));
//    // fnbProfile1.AddTemplate(new JProfileTemplate("Page", JProfileTemplateType.Section));
    
//}