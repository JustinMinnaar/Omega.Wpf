using Jem.OcrLibrary22;
using Jem.PdfProcessingLibrary;
using Jem.OcrLibrary22.Windows;

public class BdoSamples
{
    public const string oldPart1 = "Bdo.ConsoleApp4\\bin\\Debug\\net7.0-windows\\Samples";
    public const string oldPart2 = "Bdo.Samples.Benchmarking\\bin\\Debug\\net7.0-windows\\Samples";
    public const string oldPart3 = "Bdo.L5.ExtractStatements\\bin\\Debug\\net7.0-windows\\Samples";
    public const string oldPart4 = "Bdo.ConsoleApp5\\bin\\Debug\\net7.0-windows\\Samples";
    public const string newPart = "Bdo.Samples\\Samples";

    public string ReplaceOldNew(string path)
    {
        var oldPart5 = "Jem.WpfProfilingLibrary1.WpfApp1\\bin\\Debug\\net7.0-windows\\Samples";
        if (RootFolder.Contains(oldPart1))
            path = path.Replace(oldPart1, newPart);
        else if (RootFolder.Contains(oldPart2))
            path = path.Replace(oldPart2, newPart);
        else if (RootFolder.Contains(oldPart3))
            path = path.Replace(oldPart3, newPart);
        else if (RootFolder.Contains(oldPart4))
            path = path.Replace(oldPart4, newPart);
        else if (RootFolder.Contains(oldPart5))
            path = path.Replace(oldPart5, newPart);
        else
            throw new NotImplementedException();

        return path;
    }
    #region class

    public BdoSamples()
    {
        RootFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory ?? Environment.CurrentDirectory, "Samples");
        RootFolder = ReplaceOldNew(RootFolder);
    }

    public BdoSamples(string rootFolder)
    {
        RootFolder = rootFolder;
    }

    /// <summary>Path of the folder containing pdf folders to initialise these templates from.</summary>
    public string RootFolder;

    #endregion

    public string GetPdfPath(EmployeeName employee, string folderName, string filename)
        => $"{RootFolder}\\{employee}\\{folderName}\\{filename}.pdf";
    public string GetOcrPath(EmployeeName employee, string folderName, string filename)
        => $"{RootFolder}\\{employee}\\{folderName}\\{filename}.bocr";
    public string GetTxtPath(EmployeeName employee, string folderName, string filename)
        => $"{RootFolder}\\{employee}\\{folderName}\\{filename}.txt";
    public string GetCsvPath(EmployeeName employee, string folderName, string filename)
        => $"{RootFolder}\\{employee}\\{folderName}\\{filename}.csv";

    public async Task<OcrDocument> LoadOcr(EmployeeName employee, string folderName, string filename)
    {
        var ocrPath = GetOcrPath(employee, folderName, filename);
        var oDocument = await OcrDocument.TryLoadFromBinaryFileAsync(ocrPath);

        if (oDocument == null)
        {
            var pdfPath = GetPdfPath(employee, folderName, filename);
            var p = new PdfProcessor { ExtractFromImagePages = true };
            var result = await p.ProcessPdfAsync(pdfPath, null);
            oDocument = result.Document ??
                            throw new OcrException(pdfPath);
            oDocument.Clean();

            await oDocument.SaveToBinaryFileAsync(ocrPath);
        }

        oDocument.ResizeTo2100();
        //oDocument.Clean();
        oDocument.SavePageBitmaps(true, false);
        return oDocument;
    }

    #region Pdf

    public string PathPdf_Kalpesh_Consent => GetOcrPath(EmployeeName.Kalpesh, "Consent", "Consent");
    public string PathPdf_Lebogang_Consent => GetOcrPath(EmployeeName.Lebogang, "Consent", "Consent");
    public string PathPdf_Peter_Consent => GetOcrPath(EmployeeName.Peter, "Consent", "Consent");
    public string PathPdf_Shadrack_Consent => GetOcrPath(EmployeeName.Shadrack, "Consent", "Consent");
    public string PathPdf_Thando_Consent => GetOcrPath(EmployeeName.Thando, "Consent", "Consent");

    #endregion
    #region PathOcr

    public string PathOcr_Kalpesh_Consent => GetOcrPath(EmployeeName.Kalpesh, "Consent", "Consent");
    public string PathOcr_Lebogang_Consent => GetOcrPath(EmployeeName.Lebogang, "Consent", "Consent");
    public string PathOcr_Peter_Consent => GetOcrPath(EmployeeName.Peter, "Consent", "Consent");
    public string PathOcr_Shadrack_Consent => GetOcrPath(EmployeeName.Shadrack, "Consent", "Consent");
    public string PathOcr_Thando_Consent => GetOcrPath(EmployeeName.Thando, "Consent", "Consent");
    public string PathOcr_Peter_Absa01 => GetOcrPath(EmployeeName.Peter, "Absa01", "39581a75-db89-434d-92a6-0e23b8222d3c_BKS_0005f2a2-96e9-43af-974c-d-7054_AUDIT 3");
    public string Path_ThandoCertificate1Ocr => GetOcrPath(EmployeeName.Thando, "Certificate1",
    "53c7ec5a-0300-48c8-9ffa-24fa0c182172_03dbdec0-d454-43eb-98fa-8-4086_Proof of qualification");
    public string PathThandoCertificate1Pdf => GetOcrPath(EmployeeName.Thando, "Certificate1",
        "53c7ec5a-0300-48c8-9ffa-24fa0c182172_03dbdec0-d454-43eb-98fa-8-4086_Proof of qualification");

    #endregion

    public string GetTxtPath_Charnley_Absa202202() => GetTxtPath(EmployeeName.Charnley, "Absa202202", "7623bbce");
    public string GetTxtPath_Chonela_Absa01() => GetTxtPath(EmployeeName.Chonela, "Absa01", "1762450c");
    public string GetTxtPath_Moffat_Absa01() => GetTxtPath(EmployeeName.Moffat, "Absa01", "0db592e4");
    public string GetTxtPath_Molapo_Absa01() => GetTxtPath(EmployeeName.Molapo, "Absa01", "06af0545");
    public string GetTxtPath_Peter_Absa01() => GetTxtPath(EmployeeName.Peter, "Absa01", "39581a75");
    public string GetTxtPath_Peter_Absa02() => GetTxtPath(EmployeeName.Peter, "Absa02", "b70c880a");
    public string GetTxtPath_Princess_Capitec06() => GetTxtPath(EmployeeName.Princess, "Capitec06", "74d5fe48");
    public string GetTxtPath_Rama_Absa01() => GetTxtPath(EmployeeName.Rama, "Absa01", "059d8fa3");
    public string GetTxtPath_Shadrack_Nedbank1() => GetTxtPath(EmployeeName.Shadrack, "Nedbank1", "2021");

    public string GetCsvPath_Charnley_Absa02() => GetCsvPath(EmployeeName.Charnley, "Absa02", "7623bbce");
    public string GetCsvPath_Chonela_Absa01() => GetCsvPath(EmployeeName.Chonela, "Absa01", "1762450c");
    public string GetCsvPath_Moffat_Absa01() => GetCsvPath(EmployeeName.Moffat, "Absa01", "0db592e4");
    public string GetCsvPath_Molapo_Absa01() => GetCsvPath(EmployeeName.Molapo, "Absa01", "06af0545");
    public string GetCsvPath_Peter_Absa01() => GetCsvPath(EmployeeName.Peter, "Absa01", "39581a75");
    public string GetCsvPath_Peter_Absa02() => GetCsvPath(EmployeeName.Peter, "Absa02", "b70c880a");
    public string GetCsvPath_Princess_Capitec06() => GetCsvPath(EmployeeName.Princess, "Capitec06", "74d5fe48");
    public string GetCsvPath_Rama_Absa01() => GetCsvPath(EmployeeName.Rama, "Absa01", "059d8fa3");
    public string GetCsvPath_Shadrack_Nedbank1() => GetCsvPath(EmployeeName.Shadrack, "Nedbank1", "2021");

    public async Task<OcrDocument> LoadOcr_Abel_Capitec202203() => await LoadOcr(EmployeeName.Abel, "Capitec03", "2022-03");
    public async Task<OcrDocument> LoadOcr_Abel_Capitec202106() => await LoadOcr(EmployeeName.Abel, "Capitec06", "2021-06");
    public async Task<OcrDocument> LoadOcr_Abel_Capitec202109() => await LoadOcr(EmployeeName.Abel, "Capitec09", "2021-09");
    public async Task<OcrDocument> LoadOcr_Abel_Capitec202112() => await LoadOcr(EmployeeName.Abel, "Capitec12", "2021-12");


    public async Task<OcrDocument> LoadOcr_Botes_Absa202201() => await LoadOcr(EmployeeName.Botes, "Absa202201", "1762450c");
    public async Task<OcrDocument> LoadOcr_Botes_Absa202204() => await LoadOcr(EmployeeName.Botes, "Absa202204", "c0cf4751");
    public async Task<OcrDocument> LoadOcr_Botes_Capitec202106() => await LoadOcr(EmployeeName.Botes, "Capitec202106", "7f4eeeba");

    public async Task<OcrDocument> LoadOcr_Chonela_Absa01() => await LoadOcr(EmployeeName.Chonela, "Absa01", "1762450c");
    public async Task<OcrDocument> LoadOcr_Charnley_Absa202106() => await LoadOcr(EmployeeName.Charnley, "Absa202106", "442bce89");
    public async Task<OcrDocument> LoadOcr_Charnley_Absa202107() => await LoadOcr(EmployeeName.Charnley, "Absa202107", "36df4220");
    public async Task<OcrDocument> LoadOcr_Charnley_Absa202111() => await LoadOcr(EmployeeName.Charnley, "Absa202111", "21c0c5e1");
    public async Task<OcrDocument> LoadOcr_Charnley_Absa202202() => await LoadOcr(EmployeeName.Charnley, "Absa202202", "7623bbce");
    public async Task<OcrDocument> LoadOcr_Diale_Nedbank202205() => await LoadOcr(EmployeeName.Diale, "Nedbank202205", "610ad798");
    public async Task<OcrDocument> LoadOcr_DuPlooy_Fnb202109() => await LoadOcr(EmployeeName.DuPlooy, "Fnb202109", "1f6ce6f8");
    public async Task<OcrDocument> LoadOcr_DuPlooy_Fnb202203() => await LoadOcr(EmployeeName.DuPlooy, "Fnb202203", "4a3b725d");
    public async Task<OcrDocument> LoadOcr_Govindasamy_Sb202103() => await LoadOcr(EmployeeName.Govindasamy, "StandardBank202103", "18e804c3");
    public async Task<OcrDocument> LoadOcr_Harripersadh_Fnb202201() => await LoadOcr(EmployeeName.Harripersadh, "Fnb202201", "0a7b97b7_62918392218 2022-01-28");
    public async Task<OcrDocument> LoadOcr_Hartzenberg_Fnb202203() => await LoadOcr(EmployeeName.Hartzenberg, "Fnb202203", "62644878368");
    public async Task<OcrDocument> LoadOcr_Jarryd_FnbEasy06() => await LoadOcr(EmployeeName.Jarryd, "FnbEasy06", "107e4812");    
    public async Task<OcrDocument> LoadOcr_Kalpesh_Consent() => await LoadOcr(EmployeeName.Kalpesh, "Consent", "Consent");
    public async Task<OcrDocument> LoadOcr_Lebogang_Consent() => await LoadOcr(EmployeeName.Lebogang, "Consent", "Consent");
    public async Task<OcrDocument> LoadOcr_Menziwa_Fnb02() => await LoadOcr(EmployeeName.Menziwa, "Fnb02", "fef2c227");
    public async Task<OcrDocument> LoadOcr_Moffat_Absa01() => await LoadOcr(EmployeeName.Moffat, "Absa01", "0db592e4");
    public async Task<OcrDocument> LoadOcr_Moffat_Absa02() => await LoadOcr(EmployeeName.Moffat, "Absa02", "059d8fa3");
    public async Task<OcrDocument> LoadOcr_Moloio_Nedbank202204() => await LoadOcr(EmployeeName.Moloio, "Nedbank202204", "5d2d7bc4");
    public async Task<OcrDocument> LoadOcr_Manuel_Fnb202201() => await LoadOcr(EmployeeName.Manuel, "Fnb202201", "01a4c854");
    public async Task<OcrDocument> LoadOcr_Mahlatji_Nedbank202203() => await LoadOcr(EmployeeName.Mahlatji, "Nedbank202203", "8db3036c");    
    public async Task<OcrDocument> LoadOcr_Molapo_Absa01() => await LoadOcr(EmployeeName.Molapo, "Absa01", "06af0545");
    public async Task<OcrDocument> LoadOcr_Naidoo_SB04() => await  LoadOcr(EmployeeName.Naidoo, "SB04", "0081c407");
    public async Task<OcrDocument> LoadOcr_Ngcemu_Nedbank202105() => await LoadOcr(EmployeeName.Ngcemu, "Nedbank202105", "04e5f04a");
    public async Task<OcrDocument> LoadOcr_Nojoko_Absa202205()=>await LoadOcr(EmployeeName.Nojoko, "Absa202205", "480490e4");
    public async Task<OcrDocument> LoadOcr_Princess_Capitec06() => await LoadOcr(EmployeeName.Princess, "Capitec06", "74d5fe48");
    public async Task<OcrDocument> LoadOcr_Peter_Consent() => await LoadOcr(EmployeeName.Peter, "Consent", "Consent");
    public async Task<OcrDocument> LoadOcr_Peter_Absa01() => await LoadOcr(EmployeeName.Peter, "Absa01", "39581a75");
    public async Task<OcrDocument> LoadOcr_Peter_Absa02() => await LoadOcr(EmployeeName.Peter, "Absa02", "b70c880a");
    public async Task<OcrDocument> LoadOcr_Peter_Absa03() => await LoadOcr(EmployeeName.Peter, "Absa03", "ab26470f");
    public async Task<OcrDocument> LoadOcr_Radebe_Fnb09() => await LoadOcr(EmployeeName.Radebe, "Fnb09", "f4751373");
    public async Task<OcrDocument> LoadOcr_Reddy_AbsaCredit11() => await LoadOcr(EmployeeName.Reddy, "AbsaCredit11", "1544177a");
    public async Task<OcrDocument> LoadOcr_Rama_Absa01() => await LoadOcr(EmployeeName.Rama, "Absa01", "059d8fa3");
    public async Task<OcrDocument> LoadOcr_Shadrack_Consent() => await LoadOcr(EmployeeName.Shadrack, "Consent", "Consent");
    public async Task<OcrDocument> LoadOcr_Shadrack_Nedbank1() => await LoadOcr(EmployeeName.Shadrack, "Nedbank1", "2021");
    public async Task<OcrDocument> LoadOcr_Schutte_Capitec202106() => await LoadOcr(EmployeeName.Botes, "Capitec202106", "7f4eeeba");
    public async Task<OcrDocument> LoadOcr_Thando_Consent() => await LoadOcr(EmployeeName.Thando, "Consent", "Consent");
    public async Task<OcrDocument> LoadOcr_Thando_Capitec02() => await LoadOcr(EmployeeName.Thando, "Capitec02", "bd904459");
    public async Task<OcrDocument> LoadOcr_Themba_Fnb202103() => await LoadOcr(EmployeeName.Themba, "Fnb202103", "c9225eff_MARCH 2021");
    private async Task<OcrDocument> LoadOcr_Thando_Nedbank1() => await LoadOcr(EmployeeName.Thando, "Nedbank1", "3b3691c2");

    
}