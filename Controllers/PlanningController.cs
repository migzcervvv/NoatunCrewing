using Microsoft.AspNetCore.Mvc;

namespace NoatunCrewing.Controllers;

public class PlanningController : Controller
{
    public async Task<IActionResult> Index() => View();
}