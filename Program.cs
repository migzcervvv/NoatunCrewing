using Microsoft.EntityFrameworkCore;
using NoatunCrewing.Services.Implementations;

var builder = WebApplication.CreateBuilder(args);

// ---- Data: two contexts, two connection strings, matching the RBAC plan's dual-DB split ----
builder.Services.AddDbContext<NoatunCrewingContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("NoatunCrewingContext")));

//builder.Services.AddDbContext<AmsReadOnlyContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("AmsDb")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedAccount = false; // admin-created accounts, not self-registered
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
})
    .AddEntityFrameworkStores<NoatunCrewingContext>()
    .AddDefaultTokenProviders();
builder.Services.Configure<SecurityStampValidatorOptions>(options =>
{
    options.ValidationInterval = TimeSpan.FromMinutes(1);
});
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
});

// ---- Authorization: policy-based, as recommended in section 4.4 of the RBAC plan ----
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(AppPolicies.AdminOnly, p =>
        p.RequireRole(AppRoles.SuperAdmin));

    options.AddPolicy(AppPolicies.CanWriteCrewData, p =>
        p.RequireRole(AppRoles.SuperAdmin, AppRoles.ReadWriteAccess, AppRoles.Manager));

    options.AddPolicy(AppPolicies.CanReadAmsData, p =>
        p.RequireRole(AppRoles.SuperAdmin, AppRoles.ReadWriteAccess, AppRoles.ReadOnlyAccess,
                       AppRoles.Manager, AppRoles.Staff));
});

builder.Services.AddScoped<ICrewDataService, CrewDataService>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

//await IdentitySeeder.SeedAsync(app.Services);

app.Run();
