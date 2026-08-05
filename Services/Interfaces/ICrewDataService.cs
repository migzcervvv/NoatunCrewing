namespace NoatunCrewing.Services.Interfaces;

/// <summary>
/// Central place that knows which database a given crew record lives in.
/// Controllers call this instead of touching NoatunCrewingContext / AmsReadOnlyContext
/// directly, so the read-only rule for AMS lives in exactly one place.
/// </summary>
public interface ICrewDataService
{
    CrewSource ResolveSource(string nationality);

    // Example shape once crew entities exist:
    // Task<IReadOnlyList<CrewMemberDto>> GetCrewAsync(CrewSource source, CancellationToken ct = default);
    // Task<CrewMemberDto?> GetCrewByIdAsync(string id, CrewSource source, CancellationToken ct = default);
    // Task CreateNoatunCrewAsync(CrewMemberDto dto, CancellationToken ct = default); // NoatunCrewing only - no AMS write path exists.
}
