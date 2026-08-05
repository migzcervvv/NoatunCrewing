namespace NoatunCrewing.Models;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string ProfilePhotoPath { get; set; } = string.Empty;
    public string SignatureImagePath { get; set; } = string.Empty;
    public ICollection<ApplicationUserGroup> UserGroups { get; set; } = new List<ApplicationUserGroup>();
} 