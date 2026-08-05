namespace NoatunCrewing.Services.Implementations;

public class CrewDataService : ICrewDataService
{
    private readonly NoatunCrewingContext _noatunCrewingContext;
    //private readonly AmsReadOnlyContext _amsContext;

    public CrewDataService(NoatunCrewingContext noatunCrewingContext /*, AmsReadOnlyContext amsContext*/)
    {
        _noatunCrewingContext = noatunCrewingContext;
        //_amsContext = amsContext;
    }

    //public CrewSource ResolveSource(string nationality) =>
    //    nationality?.Trim().Equals("Filipino", StringComparison.OrdinalIgnoreCase) == true
    //        ? CrewSource.Ams
    //        : CrewSource.NoatunCrewing;

    public CrewSource ResolveSource(string nationality) => CrewSource.NoatunCrewing;

    // Implement CRUD here as crew entities are added. Every write path must
    // check ResolveSource(...) == CrewSource.NoatunCrewing before touching
    // _noatunCrewingContext; AMS never receives writes through this service,
    // and _amsContext has no SaveChanges path anyway (see AmsReadOnlyContext).
}
