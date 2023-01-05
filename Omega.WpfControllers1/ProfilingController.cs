using Jem.CommonLibrary22;

using Omega.WpfModels1;

using System;
using System.Collections.ObjectModel;

namespace Omega.WpfControllers1;

public class ProfilingController : CNotifyPropertyChanged
{
    public required MainController Main { get; init; }

    #region Profiles

    public ObservableCollection<ProfileModel>? Profiles { get; set; } = new();
    public ProfileModel? SelectedProfile { get; set; }

    public event EventHandler? SelectedProfileChanged;
    protected virtual void OnSelectedProfileChanged() => SelectedProfileChanged?.Invoke(this, EventArgs.Empty);

    #endregion

    //public async Task LoadProfiles()
    //{
    //    SelectedProfile = null;
    //    Profiles = new ObservableCollection<ProfileModel>();

    //    using var db = new BdoDbContext();
    //    var dbProfiles = await db.ProProfiles.ToListAsync();

    //    foreach (var dbProfile in dbProfiles)
    //    {
    //        var mProfile = new ProfileModel { Id = dbProfile.Id, Name = dbProfile.Name };
    //        Profiles.Add(mProfile);
    //        SelectedProfile ??= mProfile;
    //    }
    //}

    //public async Task LoadProfiles()
    //{

    //        var profile = new ProfileModel { Name = "Standard Statement" };
    //        profile.Templates.Add(new TemplateModel { Name = "Page" });
    //        mRoot.Profiles.Add(profile);

    //}

}
