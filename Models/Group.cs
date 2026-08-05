namespace NoatunCrewing.Models;

public class Group
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public ICollection<ApplicationUserGroup> UserGroups { get; set; } = new List<ApplicationUserGroup>();
}

public class ApplicationUserGroup
{
    public string ApplicationUserId { get; set; } = string.Empty;
    public ApplicationUser ApplicationUser { get; set; } = null!;

    public int GroupId { get; set; }
    public Group Group { get; set; } = null!;
}