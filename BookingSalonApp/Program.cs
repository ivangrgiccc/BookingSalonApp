using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BookingSalonApp.Data;
using BookingSalonApp.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<User, IdentityRole>() 
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login"; 
    options.AccessDeniedPath = "/Account/AccessDenied"; 
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();  
await SeedDataAsync(app); 

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

async Task SeedDataAsync(WebApplication app)
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<User>>();

        try
        {
            string[] roleNames = { "Admin", "User" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            var adminEmail = "ivangrgic0301@gmail.com"; 
            var adminPassword = "Jedrenjaci1!"; 
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new User { UserName = adminEmail, Email = adminEmail };
                var createResult = await userManager.CreateAsync(adminUser, adminPassword);
                if (!createResult.Succeeded)
                {
                    Console.WriteLine("Error creating admin user.");
                    return;
                }
            }

            if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error seeding data: {ex.Message}");
        }
    }
}
