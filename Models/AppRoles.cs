namespace NoatunCrewing.Models;

// [Authorize(Roles = "...")] and RoleManager calls both need plain strings,
// so this stays a static class rather than an enum, but centralizes every
// role name in one place instead of duplicating literals per controller.
public static class AppRoles
{
    public const string SuperAdmin = "SuperAdmin";
    public const string ReadWriteAccess = "ReadWriteAccess";
    public const string ReadOnlyAccess = "ReadOnlyAccess";
    public const string Manager = "Manager";
    public const string Staff = "Staff";

    public static readonly string[] All =
    {
        SuperAdmin, ReadWriteAccess, ReadOnlyAccess, Manager, Staff
    };
}
