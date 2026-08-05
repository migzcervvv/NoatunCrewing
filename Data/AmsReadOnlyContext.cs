using Microsoft.EntityFrameworkCore;

namespace NoatunCrewing.Data;

/// <summary>
/// AMS is an externally-owned system of record for Filipino crew.
/// This context is read-only in three layers, matching the original plan:
///   1. Compile-time: no write helpers exposed, callers only ever get IQueryable.
///   2. Runtime: SaveChanges/SaveChangesAsync are overridden to throw.
///   3. Database: the SQL login backing this connection string must be
///      granted db_datareader ONLY on the AMS database. That grant is a
///      DBA/deployment step, not something this class can enforce by itself.
/// </summary>
public class AmsReadOnlyContext : DbContext
{
    public AmsReadOnlyContext(DbContextOptions<AmsReadOnlyContext> options) : base(options)
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    // DbSet<FilipinoCrewMember> FilipinoCrew => Set<FilipinoCrewMember>();
    // Map DbSets to the existing AMS schema with .ToTable(...)/.ToView(...) as needed.

    public override int SaveChanges() =>
        throw new InvalidOperationException("AmsReadOnlyContext is read-only. AMS data cannot be written from Noatun MGT.");

    public override int SaveChanges(bool acceptAllChangesOnSuccess) =>
        throw new InvalidOperationException("AmsReadOnlyContext is read-only. AMS data cannot be written from Noatun MGT.");

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        throw new InvalidOperationException("AmsReadOnlyContext is read-only. AMS data cannot be written from Noatun MGT.");

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default) =>
        throw new InvalidOperationException("AmsReadOnlyContext is read-only. AMS data cannot be written from Noatun MGT.");
}
