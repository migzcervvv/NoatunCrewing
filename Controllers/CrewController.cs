namespace NoatunCrewing.Controllers;

// Single controller serving both AMS-backed (Filipino) and NoatunCrewing-backed
// (Kenyan) crew, per section 4.2 of the RBAC plan: role checks are applied
// per action, not per controller, because write actions are only valid for
// the NoatunCrewing-sourced half of the data.
[Authorize]
public class CrewController(ICrewDataService crewDataService) : Controller
{
    [Authorize(Policy = AppPolicies.CanReadAmsData)]
    public IActionResult Index()
    {
        // TODO: return combined listing via _crewDataService once crew entities exist.
        return View();
    }

    [Authorize(Policy = AppPolicies.CanReadAmsData)]
    public IActionResult Details(string id, string nationality)
    {
        var source = crewDataService.ResolveSource(nationality);
        ViewBag.Source = source;
        return View();
    }

    [Authorize(Policy = AppPolicies.CanWriteCrewData)]
    [HttpGet]
    public IActionResult Create() => View();

    //[Authorize(Policy = AppPolicies.CanWriteCrewData)]
    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public IActionResult Create(/* CrewMemberViewModel model */)
    //{
    //    // Writes only ever target NoatunCrewingContext. AMS has no write path;
    //    // ICrewDataService intentionally exposes no AMS create/update/delete method.
    //    return RedirectToAction(nameof(Index));
    //}
}
