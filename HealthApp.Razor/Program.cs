using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HealthApp.Razor.Data;
using HealthApp.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();

    
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    _ = endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Calendrier}/{action=Index}/{id?}");
});

app.UseAuthorization();

app.MapRazorPages();

app.Run();

public static class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        // Création du rôle Doctor s'il n'existe pas
        if (!await roleManager.RoleExistsAsync("Doctor"))
        {
            await roleManager.CreateAsync(new IdentityRole("Doctor"));
        }

        // Création d'un docteur manuellement
        string doctorEmail = "doctor@example.com";
        string doctorPassword = "Doctor@123";

        if (await userManager.FindByEmailAsync(doctorEmail) == null)
        {
            var doctor = new IdentityUser { UserName = doctorEmail, Email = doctorEmail };
            var result = await userManager.CreateAsync(doctor, doctorPassword);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(doctor, "Doctor");
            }
        }
    }
}


