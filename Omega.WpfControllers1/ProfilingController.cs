using Jem.CommonLibrary22;

using Omega.WpfCommon1;
using Omega.WpfModels1;
using Omega.WpfModels1.Profiling;

using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;

namespace Omega.WpfControllers1;

public static class ProfilingCommands
{
    public static RoutedUICommand AddBag { get; } = new("Add Bag", "AddBag", typeof(ProfilingCommands));
}

public class ProfilingController : CNotifyPropertyChanged
{
    RelayCommand _AddBagCommand, _AddGroupCommand, _AddProfileCommand;

    public ICommand AddBagCommand => _AddBagCommand ??= new RelayCommand(AddBag, CanAddBag);
    public ICommand AddGroupCommand => _AddGroupCommand ??= new RelayCommand(AddGroup, CanAddGroup);
    public ICommand AddProfileCommand => _AddProfileCommand ??= new RelayCommand(AddProfile, CanAddProfile);

    private bool CanAddBag() { return true; }
    public void AddBag()
    {
        var bags = this.Bags; if (bags == null) return;
        var bag = this.SelectedBag; if (bag == null) return;

        var name = Main.Explorer.LastRectangleText ?? "(New)";
        var newBag = new ProBagModel { Id = Guid.NewGuid(), Name = name };
        bags.Add(newBag);

        this.SelectedBag = newBag;
    }

    public bool CanAddGroup()
    {
        var bag = this.SelectedBag;
        if (bag == null) return false;

        return true;
    }
    public void AddGroup()
    {
        var bags = this.Bags; if (bags == null) return;
        var bag = this.SelectedBag; if (bag == null) return;

        var groups = bag.Groups; if (groups == null) return;
        var group = bag.SelectedGroup; if (group == null) return;

        var name = Main.Explorer.LastRectangleText ?? "(New)";
        var newGroup = new ProGroupModel { Id = Guid.NewGuid(), Name = name };
        groups.Add(newGroup);

        bag.SelectedGroup = newGroup;
    }

    public bool CanAddProfile()
    {
        var bag = this.SelectedBag;
        if (bag == null) return false;

        var group = bag.SelectedGroup;
        if (group == null) return false;

        return true;
    }
    public void AddProfile()
    {
        var bags = this.Bags; if (bags == null) return;
        var bag = this.SelectedBag; if (bag == null) return;

        var groups = bag.Groups; if (groups == null) return;
        var group = bag.SelectedGroup; if (group == null) return;

        var profiles = group.Profiles; if (profiles == null) return;
        var profile = group.SelectedProfile; if (profile == null) return;

        var name = Main.Explorer.LastRectangleText ?? "(New)";
        var newProfile = new ProProfileModel { Id = Guid.NewGuid(), Name = name };
        profiles.Add(newProfile);

        group.SelectedProfile = newProfile;
    }

    [SetsRequiredMembers]
    public ProfilingController(MainController main)
    {
        this.Main = main;
    }

    public required MainController Main { get; init; }

    public event EventHandler? SelectedBagChanged;

    public event EventHandler? SelectedGroupChanged;

    #region Bags

    public ObservableCollection<ProBagModel> Bags { get; set; } = new();
    public ProBagModel? SelectedBag { get; set; }
    protected virtual void OnSelectedBagChanged() { SelectedBagChanged?.Invoke(this, EventArgs.Empty); }

    #endregion


    #region Profiles

    public ObservableCollection<ProProfileModel> Profiles { get; set; } = new();
    public ProProfileModel? SelectedProfile { get; set; }

    public event EventHandler? SelectedProfileChanged;
    protected virtual void OnSelectedProfileChanged() => SelectedProfileChanged?.Invoke(this, EventArgs.Empty);

    #endregion

    #region Templates

    public ObservableCollection<ProTemplateModel> Templates { get; set; } = new();
    public ProTemplateModel? SelectedTemplate { get; set; }

    public event EventHandler? SelectedTemplateChanged;
    protected virtual void OnSelectedTemplateChanged() => SelectedTemplateChanged?.Invoke(this, EventArgs.Empty);

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
