namespace NoatunCrewing.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = AppPolicies.AdminOnly)]
public class RoleManagementController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager) : Controller
{
    // GET: Admin/RoleManagement
    public async Task<IActionResult> Index()
    {
        var roles = new List<RoleVM>();

        foreach (var r in roleManager.Roles.ToList())
        {
            var usersInRole = await userManager.GetUsersInRoleAsync(r.Name!);
            roles.Add(new RoleVM
            {
                Id = r.Id,
                Name = r.Name!,
                UserCount = usersInRole.Count
            });
        }

        return View(roles);
    }

    // GET: Admin/RoleManagement/Create
    public ActionResult Create()
    {
        return View(new RoleVM());
    }

    // POST: Admin/RoleManagement/Create
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RoleVM vm)
    {
        if (!ModelState.IsValid) return View(vm);

        if (await roleManager.RoleExistsAsync(vm.Name))
        {
            ModelState.AddModelError("", "Role already exists.");
            return View(vm);
        }

        var result = await roleManager.CreateAsync(new IdentityRole(vm.Name));
        if (!result.Succeeded)
        {
            AddErrors(result);
            return View(vm);
        }

        TempData.SetSuccess("Successfully created role.");
        return RedirectToAction(nameof(Index));
    }

    // GET: Admin/RoleManagement/Edit/5
    public async Task<IActionResult> Edit(string id)
    {
        var role = await roleManager.FindByIdAsync(id);
        if (role == null) return NotFound();
        return View(new RoleVM { Id = role.Id, Name = role.Name! });
    }

    // POST: Admin/RoleManagement/Edit
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(RoleVM vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var role = await roleManager.FindByIdAsync(vm.Id);
        if (role == null) return NotFound();

        role.Name = vm.Name;
        var result = await roleManager.UpdateAsync(role);
        if (!result.Succeeded)
        {
            AddErrors(result);
            return View(vm);
        }

        TempData.SetSuccess("Successfully edited role.");
        return RedirectToAction(nameof(Index));
    }

    // GET: Admin/RoleManagement/Delete/5
    public async Task<IActionResult> Delete(string id)
    {
        var role = await roleManager.FindByIdAsync(id);
        if (role == null) return NotFound();
        return View(role);
    }

    // POST: Admin/RoleManagement/Delete/5
    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        var role = await roleManager.FindByIdAsync(id);
        if (role == null) return NotFound();

        var usersInRole = await userManager.GetUsersInRoleAsync(role.Name!);
        if (usersInRole.Any())
        {
            ModelState.AddModelError("", $"Cannot delete: {usersInRole.Count} user(s) assigned to this role.");
            return View("Delete", role);
        }

        var result = await roleManager.DeleteAsync(role);
        if (!result.Succeeded)
        {
            AddErrors(result);
            return View("Delete", role);
        }

        TempData.SetSuccess("Successfully deleted role.");
        return RedirectToAction(nameof(Index));
    }

    private void AddErrors(IdentityResult result)
    {
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
            TempData.SetError(error.Description);
        }
    }
}