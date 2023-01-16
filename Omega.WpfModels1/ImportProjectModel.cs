using Bdo.DatabaseLibrary1;

using Jem.CommonLibrary22;

using Microsoft.EntityFrameworkCore;

using Omega.WpfCommon1;
using Omega.WpfModels1;
using Omega.WpfModels1.old;

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace Omega.WpfModels1;

public class ImportProjectModelDeleted
{   
    


    //private async Task<DocPage> AccessFilePage(BdoDbContext db, Guid fileId, JPage page)
    //{
    //    var dbPage = await db.DocPages.FirstOrDefaultAsync(p => p.OwnerFileId == fileId && p.PageIndex == page.PageIndex);
    //    if (dbPage == null)
    //    {
    //        dbPage = new DocPage
    //        {
    //            OwnerFileId = fileId,
    //            PageIndex = page.PageIndex,
    //            IsBlank = page.IsBlank,
    //            ProfileId = page.ProfileId,
    //            ProfileName = page.ProfileName,
    //            ProfileVersion = page.ProfileVersion,
    //            IsError = page.IsError,
    //        };
    //        db.DocPages.Add(dbPage);
    //        //await db.SaveChangesAsync();
    //    }

    //    return dbPage;
    //}

}