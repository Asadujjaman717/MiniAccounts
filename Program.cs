using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MiniAccounts.Data;

var builder = WebApplication.CreateBuilder(args);

// 🔌 Configure Connection String
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// 🔐 Add Identity with Role Support
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // Set false for easy testing
})
.AddRoles<IdentityRole>() // ⬅️ This enables role management
.AddEntityFrameworkStores<ApplicationDbContext>();

// ➕ Add Razor Pages
builder.Services.AddRazorPages();

// 🔧 Build App
var app = builder.Build();

// 🌱 Seed Roles and Admin User
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await MiniAccounts.Data.DbInitializer.SeedRolesAndAdmin(services); // ⬅️ We’ll create this next
}

// 🌐 Middleware Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // ⬅️ Required before Authorization
app.UseAuthorization();

//app.MapRazorPages();
// ✅ Require login globally unless a page explicitly allows anonymous access
app.MapRazorPages().RequireAuthorization();

app.Run();
