using Jem.ProDatabaseLibrary1.Entities;

namespace Jem.ProDatabaseLibrary1;

public class ProDbContext : DbContext
{
    #region Bags

    public DbSet<ProBag> ProBags { get; set; } = default!;

    public async Task<ProBag> AddBagAsync(ID<ProBag> id, string name)
    {
        var row = new ProBag
        {
            Id = id,
            Name = name,
        };

        ProBags.Add(row);
        await SaveChangesAsync();
        return row;
    }

    public async Task<ProBag> GetBagAsync(ID<ProBag> id) =>
        await TryGetBagAsync(id) ??
        throw new NotFoundException($"Bag {id} not found!");

    public async Task<ProBag?> TryGetBagAsync(ID<ProBag> id) =>
        await ProBags.FindAsync(id.Guid);

    public async Task<ProBag?> TryGetBagAsync(string name) =>
        await ProBags.FirstOrDefaultAsync(p => p.Name == name);

    public void DropBags(params ProBag[] rows) =>
        ProBags.RemoveRange(rows);

    #endregion

    #region Groups

    public DbSet<ProGroup> ProGroups { get; set; } = default!;

    public async Task<ProGroup> AddGroupAsync(ID<ProBag> bagId, ID<ProGroup> id, string name)
    {
        var row = new ProGroup
        {
            OwnerBagId= bagId,
            Id = id,
            Name = name,
        };

        ProGroups.Add(row);
        await SaveChangesAsync();
        return row;
    }

    public async Task<ProGroup> GetGroupAsync(ID<ProGroup> id) =>
        await TryGetGroupAsync(id) ??
        throw new NotFoundException($"Group {id} not found!");

    public async Task<ProGroup?> TryGetGroupAsync(ID<ProGroup> id) =>
        await ProGroups.FindAsync(id.Guid);

    public async Task<ProGroup?> TryGetGroupAsync(ID<ProBag> bagId, string name) =>
        await ProGroups.FirstOrDefaultAsync(p => p.OwnerBagId == bagId && p.Name == name);

    public void DropGroups(params ProGroup[] rows) =>
        ProGroups.RemoveRange(rows);

    #endregion

    #region Profiles

    public DbSet<ProProfile> ProProfiles { get; set; } = default!;

    public async Task<ProProfile> AddProfileAsync(ID<ProGroup> groupId, ID<ProProfile> id, string name)
    {
        var row = new ProProfile
        {
            OwnerGroupId = groupId,
            Id = id,
            Name = name,
        };

        ProProfiles.Add(row);
        await SaveChangesAsync();
        return row;
    }

    public async Task<ProProfile> GetProfileAsync(ID<ProProfile> id) =>
        await TryGetProfileAsync(id) ??
        throw new NotFoundException($"Profile {id} not found!");

    public async Task<ProProfile?> TryGetProfileAsync(ID<ProProfile> id) =>
        await ProProfiles.FindAsync(id.Guid);

    public async Task<ProProfile?> TryGetProfileAsync(ID<ProGroup> groupId, string name) =>
        await ProProfiles.FirstOrDefaultAsync(p => p.OwnerGroupId == groupId     && p.Name == name);

    public void DropProfiles(params ProProfile[] rows) =>
        ProProfiles.RemoveRange(rows);

    #endregion

    #region Templates

    public DbSet<ProTemplate> ProTemplates { get; set; } = default!;

    public async Task<ProTemplate> AddTemplateAsync(ID<ProProfile> profileId, ID<ProTemplate> id, string name)
    {
        var row = new ProTemplate
        {
            OwnerProfileId = profileId,
            Id = id,
            Name = name,
        };

        ProTemplates.Add(row);
        await SaveChangesAsync();
        return row;
    }

    public async Task<ProTemplate> GetTemplateAsync(ID<ProTemplate> id) =>
        await TryGetTemplateAsync(id) ??
        throw new NotFoundException($"Template {id} not found!");

    public async Task<ProTemplate?> TryGetTemplateAsync(ID<ProTemplate> id) =>
        await ProTemplates.FindAsync(id.Guid);

    public async Task<ProTemplate?> TryGetTemplateAsync(ID<ProProfile> profileId, string name) =>
        await ProTemplates.FirstOrDefaultAsync(p => p.OwnerProfileId == profileId && p.Name == name);

    public void DropTemplates(params ProTemplate[] rows) =>
        ProTemplates.RemoveRange(rows);

    #endregion

    #region ctor

    private readonly string? _connectionString;

    public ProDbContext(string connectionString) => _connectionString = connectionString;

    public ProDbContext(DbContextOptions options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (_connectionString != null)
            optionsBuilder.UseSqlServer(_connectionString);
    }

    #endregion

    #region model

    protected override void OnModelCreating(ModelBuilder m)
    {
        base.OnModelCreating(m);

        ConfigBags(m);
        ConfigGroups(m);
        ConfigProfiles(m);
        ConfigTemplates(m);        
    }

    private static void ConfigBags(ModelBuilder m)
    {
        m.Entity<ProBag>()
            .HasMany<ProGroup>(aBag => aBag.Groups)
            .WithOne(aGroup => aGroup.OwnerBag)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }

    private static void ConfigGroups(ModelBuilder m)
    {
        m.Entity<ProGroup>()
            .HasMany<ProProfile>(aGroup => aGroup.Profiles)
            .WithOne(aProfile => aProfile.OwnerGroup)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }

    private static void ConfigProfiles(ModelBuilder m)
    {
        m.Entity<ProProfile>()
            .HasMany<ProTemplate>(aProfile => aProfile.Templates)
            .WithOne(aTemplate => aTemplate.OwnerProfile)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }

    private static void ConfigTemplates(ModelBuilder m)
    {
    }

    #endregion
}

