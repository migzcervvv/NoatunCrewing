namespace NoatunCrewing.Controllers;

[Route("Admin/UserManagement/[action]")]
[Authorize(Policy = AppPolicies.AdminOnly)]
public class UserManagementController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager) : Controller
{
    // GET: Admin/UserManagement
    public async Task<IActionResult> Index()
    {
        var users = userManager.Users.ToList();
        var list = new List<UserListItemVM>();

        foreach (var u in users)
        {
            list.Add(new UserListItemVM
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                UserName = u.UserName ?? "",
                Email = u.Email ?? "",
                Roles = await userManager.GetRolesAsync(u),
                LockedOut = await userManager.IsLockedOutAsync(u)
            });
        }

        return View(list);
    }

    // GET: Admin/UserManagement/Create
    public ActionResult Create()
    {
        var vm = new UserCreateVM { AllRoles = roleManager.Roles.Select(r => r.Name!).ToList() };
        return View(vm);
    }

    // POST: Admin/UserManagement/Create
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UserCreateVM vm)
    {
        if (!ModelState.IsValid)
        {
            vm.AllRoles = roleManager.Roles.Select(r => r.Name!).ToList();
            return View(vm);
        }

        var user = new ApplicationUser
        {
            FirstName = vm.FirstName,
            LastName = vm.LastName,
            UserName = vm.UserName,
            Email = vm.Email,
        };
        var result = await userManager.CreateAsync(user, vm.Password);

        if (result.Succeeded)
        {
            if (vm.SelectedRoles != null && vm.SelectedRoles.Any())
            {
                await userManager.AddToRolesAsync(user, vm.SelectedRoles);
            }
            TempData.SetStatusMessage(StatusMessageType.Success, "User created successfully.");
            return RedirectToAction(nameof(Index));
        }

        AddErrors(result);
        vm.AllRoles = roleManager.Roles.Select(r => r.Name!).ToList();
        return View(vm);
    }

    // GET: Admin/UserManagement/Edit/5
    public async Task<IActionResult> Edit(string id)
    {
        if (string.IsNullOrEmpty(id)) return BadRequest();

        var user = await userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        var vm = new UserEditVM
        {
            Id = user.Id,
            Email = user.Email ?? "",
            FirstName = user.FirstName,
            LastName = user.LastName,
            UserName = user.UserName ?? "",
            SelectedRoles = (await userManager.GetRolesAsync(user)).ToList(),
            AllRoles = roleManager.Roles.Select(r => r.Name!).ToList()
        };

        return View(vm);
    }

    // POST: Admin/UserManagement/Edit
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UserEditVM vm)
    {
        if (!ModelState.IsValid)
        {
            vm.AllRoles = roleManager.Roles.Select(r => r.Name!).ToList();
            return View(vm);
        }

        var user = await userManager.FindByIdAsync(vm.Id);
        if (user == null) return NotFound();

        user.Email = vm.Email;
        user.FirstName = vm.FirstName;
        user.LastName = vm.LastName;
        user.UserName = vm.UserName;

        var updateResult = await userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            AddErrors(updateResult);
            vm.AllRoles = roleManager.Roles.Select(r => r.Name!).ToList();
            return View(vm);
        }

        var currentRoles = await userManager.GetRolesAsync(user);
        var rolesToRemove = currentRoles.Except(vm.SelectedRoles ?? new List<string>()).ToArray();
        var rolesToAdd = (vm.SelectedRoles ?? new List<string>()).Except(currentRoles).ToArray();

        if (rolesToRemove.Any()) await userManager.RemoveFromRolesAsync(user, rolesToRemove);
        if (rolesToAdd.Any()) await userManager.AddToRolesAsync(user, rolesToAdd);

        if (rolesToRemove.Any() || rolesToAdd.Any())
        {
            await userManager.UpdateSecurityStampAsync(user);
        }

        TempData.SetStatusMessage(StatusMessageType.Success, "User edited successfully.");
        return RedirectToAction(nameof(Index));
    }

    // GET: Admin/UserManagement/Delete/5
    public async Task<IActionResult> Delete(string id)
    {
        if (string.IsNullOrEmpty(id)) return BadRequest();
        var user = await userManager.FindByIdAsync(id);
        if (user == null) return NotFound();
        return View(user);
    }

    // POST: Admin/UserManagement/Delete/5
    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        if (userManager.GetUserId(User) == id)
        {
            ModelState.AddModelError("", "Cannot delete your own account while logged in.");
            return View("Delete", user);
        }

        var result = await userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            AddErrors(result);
            return View("Delete", user);
        }

        TempData.SetStatusMessage(StatusMessageType.Success, "User deleted successfully.");
        return RedirectToAction(nameof(Index));
    }

    // GET: Admin/UserManagement/ChangePassword/5
    public async Task<IActionResult> ChangePassword(string id)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        return View(new AdminChangePasswordVM { UserId = user.Id, UserName = user.UserName ?? "" });
    }

    // POST: Admin/UserManagement/ChangePassword
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(AdminChangePasswordVM vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var user = await userManager.FindByIdAsync(vm.UserId);
        if (user == null) return NotFound();

        var removeResult = await userManager.RemovePasswordAsync(user);
        if (!removeResult.Succeeded)
        {
            AddErrors(removeResult);
            return View(vm);
        }

        var addResult = await userManager.AddPasswordAsync(user, vm.NewPassword);
        if (!addResult.Succeeded)
        {
            AddErrors(addResult);
            return View(vm);
        }

        TempData.SetStatusMessage(StatusMessageType.Success, $"Password changed for {user.UserName}.");
        return RedirectToAction(nameof(Index));
    }

    // POST: Admin/UserManagement/ForcePasswordReset/5
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ForcePasswordReset(string id)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        var code = await userManager.GeneratePasswordResetTokenAsync(user);
        var encodedCode = System.Net.WebUtility.UrlEncode(code);
        var callbackUrl = Url.Action("ResetPassword", "Account",
            new { area = "", userId = user.Id, code = encodedCode }, protocol: Request.Scheme);

        // Email sending not yet implemented — see IEmailSender stub.
        // await _emailSender.SendEmailAsync(user.Email!, "Reset Password",
        //     $"Please reset your password by clicking <a href=\"{callbackUrl}\">here</a>.");

        TempData.SetStatusMessage(StatusMessageType.Success, $"Password reset link generated for {user.Email}.");
        return RedirectToAction(nameof(Index));
    }

    // POST: Admin/UserManagement/ToggleLock/5
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleLock(string id)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        if (await userManager.IsLockedOutAsync(user))
        {
            await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(-1));
        }
        else
        {
            await userManager.SetLockoutEnabledAsync(user, true);
            await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(100));
        }

        return RedirectToAction(nameof(Index));
    }

    private void AddErrors(IdentityResult result)
    {
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
            TempData.SetStatusMessage(StatusMessageType.Error, error.Description);
        }
    }
}