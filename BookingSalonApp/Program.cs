using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BookingSalonApp.Data;
using BookingSalonApp.Models;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Dodavanje potrebnih servisa
builder.Services.AddControllersWithViews();

// Dohvatanje ConnectionString-a iz appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Dodavanje DbContext-a sa SQL Serverom
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null
        ))
);

// Dodavanje Identity servisa
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Konfiguracija kolačića za autentifikaciju
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

var app = builder.Build();

// Provera da li je aplikacija u razvojnom okruženju
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

// Inicijalizacija podataka
await SeedDataAsync(app);

// Definisanje ruta
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

// Funkcija za inicijalizaciju podataka
async Task SeedDataAsync(WebApplication app)
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<User>>();
        var logger = services.GetRequiredService<ILogger<Program>>();

        try
        {
            // Kreiraj uloge ako ne postoje
            string[] roleNames = { "Admin", "User" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    var role = new IdentityRole(roleName);
                    var result = await roleManager.CreateAsync(role);
                    if (result.Succeeded)
                    {
                        logger.LogInformation($"Uloga '{roleName}' je uspešno kreirana.");
                    }
                    else
                    {
                        logger.LogError($"Greška pri kreiranju role '{roleName}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
                else
                {
                    logger.LogInformation($"Uloga '{roleName}' već postoji.");
                }
            }

            // Kreiraj admin korisnika ako ne postoji
            var adminEmail = "ivangrgic0301@gmail.com";
            var adminPassword = "Jedrenjaci1!";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new User
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "Ivan",  // Postavite ime
                    LastName = "Grgić"   // Postavite prezime
                };

                var createResult = await userManager.CreateAsync(adminUser, adminPassword);
                if (createResult.Succeeded)
                {
                    logger.LogInformation("Admin korisnik je kreiran.");
                }
                else
                {
                    logger.LogError($"Greška pri kreiranju admin korisnika: {string.Join(", ", createResult.Errors.Select(e => e.Description))}");
                }
            }
            else
            {
                logger.LogInformation("Admin korisnik već postoji.");
            }

            // Dodeli Admin rolu korisniku ako je potrebno
            if (adminUser != null && !await userManager.IsInRoleAsync(adminUser, "Admin"))
            {
                var addRoleResult = await userManager.AddToRoleAsync(adminUser, "Admin");
                if (addRoleResult.Succeeded)
                {
                    logger.LogInformation("Admin rola je dodeljena.");
                }
                else
                {
                    logger.LogError($"Greška prilikom dodeljivanja role Admin korisniku: {string.Join(", ", addRoleResult.Errors.Select(e => e.Description))}");
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError($"Greška pri inicijalizaciji podataka: {ex.Message}");
            if (ex.InnerException != null)
            {
                logger.LogError($"Inner exception: {ex.InnerException.Message}");
            }
        }
    }
}
