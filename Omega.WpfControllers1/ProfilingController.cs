using Bdo.DatabaseLibrary1;

using Jem.CommonLibrary22;
using Jem.Profiling22.Data;

using Microsoft.EntityFrameworkCore.Metadata;

using Omega.WpfCommon1;
using Omega.WpfModels1;
using Omega.WpfModels1.Profiling;

using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Omega.WpfControllers1;

public static class ProfilingCommands
{
    public static RoutedUICommand AddBag { get; } = new("Add Bag", "AddBag", typeof(ProfilingCommands));
}

public class ProfilingController : CNotifyPropertyChanged
{
    #region class

    [SetsRequiredMembers]
    public ProfilingController(MainController main)
    {
        this.Main = main;
    }

    public required MainController Main { get; init; }

    #endregion

    #region Commands

    RelayCommand?
        _SetTemplateRectCommand,
        _AddBagCommand, _AddGroupCommand, _AddProfileCommand, _AddTemplateCommand;

    public ICommand SetTemplateRectCommand => _SetTemplateRectCommand ??= new RelayCommand(SetTemplateRect, CanSetTemplateRect);
    public ICommand AddBagCommand => _AddBagCommand ??= new RelayCommand(AddBag, CanAddBag);
    public ICommand AddGroupCommand => _AddGroupCommand ??= new RelayCommand(AddGroup, CanAddGroup);
    public ICommand AddProfileCommand => _AddProfileCommand ??= new RelayCommand(AddProfile, CanAddProfile);
    public ICommand AddTemplateCommand => _AddTemplateCommand ??= new RelayCommand(AddTemplate, CanAddTemplate);

    private bool CanAddBag() { return true; }
    public bool AddBagEnabled => CanAddBag();
    public void AddBag()
    {
        var bags = this.Bags; if (bags == null) return;

        var name = Main.Explorer.LastRectangleText ?? "(New)";
        var newBag = new ProBagModel { Id = Guid.NewGuid(), Name = name };
        bags.Add(newBag);

        this.SelectedBag = newBag;

        this.SelectedTab = ProfilingTabs.Groups;
    }

    public bool CanAddGroup()
    {
        var bag = this.SelectedBag;
        if (bag == null) return false;

        return true;
    }
    public bool AddGroupEnabled => CanAddGroup();
    public void AddGroup()
    {
        var bags = this.Bags; if (bags == null) return;
        var bag = this.SelectedBag; if (bag == null) return;

        var groups = this.Groups; if (groups == null) return;

        var name = Main.Explorer.LastRectangleText ?? "(New)";
        var newGroup = new ProGroupModel { OwnerBag = bag, Id = Guid.NewGuid(), Name = name };
        groups.Add(newGroup);

        this.SelectedGroup = newGroup;

        this.SelectedTab = ProfilingTabs.Profiles;
    }

    public bool CanAddProfile()
    {
        var group = this.SelectedGroup;
        if (group == null) return false;

        return true;
    }
    public bool AddProfileEnabled => CanAddProfile();
    public void AddProfile()
    {
        //var bags = this.Bags; if (bags == null) return;
        //var bag = this.SelectedBag; if (bag == null) return;

        var group = this.SelectedGroup; if (group == null) return;

        var profiles = this.Profiles; if (profiles == null) return;

        var name = Main.Explorer.LastRectangleText ?? "(New)";
        var newProfile = new ProProfileModel { OwnerGroup = group, Id = Guid.NewGuid(), Name = name };
        profiles.Add(newProfile);

        SelectedProfile = newProfile;

        this.SelectedTab = ProfilingTabs.Profiles;
    }

    public bool CanAddTemplate()
    {
        var profile = this.SelectedProfile;
        if (profile == null) return false;

        return true;
    }
    public bool AddTemplateEnabled => CanAddTemplate();

    public void AddTemplate()
    {
        var profile = this.SelectedProfile;
        if (profile == null) return;

        var templates = this.Templates;
        if (templates == null) return;

        var name = Main.Explorer.LastRectangleText ?? "(New)";
        var newTemplate = new ProTemplateModel
        {
            OwnerProfile = profile,
            Id = Guid.NewGuid(),
            Name = name,
            Rect = Main.Explorer.LastRectangleDrawn,
            RectText = Main.Explorer.LastRectangleText,
        };
        templates.Add(newTemplate);

        SelectedTemplate = newTemplate;
    }

    public bool CanSetTemplateRect()
    {
        var template = this.SelectedTemplate;
        if (template == null) return false;

        return true;
    }
    public bool SetTemplateRectEnabled => CanSetTemplateRect();

    public void SetTemplateRect()
    {
        var template = this.SelectedTemplate;
        if (template == null) return;

        template.Rect = Main.Explorer.LastRectangleDrawn;
        template.RectText = Main.Explorer.LastRectangleText;        
    }

    #endregion

    public enum ProfilingTabs { Bags, Groups, Profiles, Templates }

    public ProfilingTabs? SelectedTab { get; set; } = ProfilingTabs.Bags;

    
    #region Bags

    public ObservableCollection<ProBagModel> Bags { get; set; } = new();
    public ProBagModel? SelectedBag
    {
        get { return Bags.FirstOrDefault(p => p.Id == Main.Settings.SelectedProBagId); }
        set { Main.Settings.SelectedProBagId = value?.Id; }
    }
    public event EventHandler? SelectedBagChanged;
    protected virtual void OnSelectedBagChanged() => SelectedBagChanged?.Invoke(this, EventArgs.Empty);

    #endregion

    #region Groups

    public ObservableCollection<ProGroupModel> Groups { get; set; } = new();
    public ProGroupModel? SelectedGroup
    {
        get => Groups.FirstOrDefault(g => g.Id == SelectedBag?.SelectedGroupId);
        set { var b = SelectedBag; if (b == null) return; b.SelectedGroupId = value?.Id; }
    }
    public event EventHandler? SelectedGroupChanged;
    protected virtual void OnSelectedGroupChanged() => SelectedGroupChanged?.Invoke(this, EventArgs.Empty);

    #endregion

    #region Profiles

    public ObservableCollection<ProProfileModel> Profiles { get; set; } = new();
    public ProProfileModel? SelectedProfile
    {
        get => Profiles.FirstOrDefault(p => p.Id == SelectedGroup?.SelectedProfileId);
        set { var g = SelectedGroup; if (g == null) return; g.SelectedProfileId = value?.Id; }
    }
    public event EventHandler? SelectedProfileChanged;
    protected virtual void OnSelectedProfileChanged() => SelectedProfileChanged?.Invoke(this, EventArgs.Empty);

    #endregion

    #region Templates

    public ObservableCollection<ProTemplateModel> Templates { get; set; } = new();
    public ProfileTemplateType[] TemplateTypes { get; set; } = (ProfileTemplateType[])Enum.GetValues(typeof(ProfileTemplateType));
    public ProTemplateModel? SelectedTemplate
    {
        get => Templates.FirstOrDefault(t => t.Id == SelectedProfile?.SelectedTemplateId);
        set { var p = SelectedProfile; if (p == null) return; p.SelectedTemplateId = value?.Id; }
    }
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

    public void LoadProfiles()
    {
        var bag = new ProBagModel { Id = Guid.NewGuid(), Name = "Consent" };
        Bags.Add(bag);
        SelectedBag = bag;

        var group  = new ProGroupModel { OwnerBag = bag, Id = Guid.NewGuid(), Name = "Consent" };
        Groups.Add(group);
        SelectedGroup = group;

        var profile = new ProProfileModel { OwnerGroup = group, Id = Guid.NewGuid(), Name = "Consent" };
        Profiles.Add(profile);
        SelectedProfile = profile;

        var template = new ProTemplateModel { OwnerProfile = profile, Id = Guid.NewGuid(), Name = "Page", Type = ProfileTemplateType.Page };
        Templates.Add(template);
        SelectedTemplate = template;
    }


}
