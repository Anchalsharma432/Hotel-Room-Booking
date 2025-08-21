using HospitalityProject.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("HospitalityDbConnection");
builder.Services.AddDbContext<HotelDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 35)))
);

// In Program.cs or Startup.cs
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("Smtp"));


builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

builder.Services.AddAuthorization();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<HotelDbContext>();
    dbContext.Database.EnsureCreated();

    // ✅ Ensure Admin user exists (hardcoded)
    if (!dbContext.Users.Any(u => u.Role == "Admin"))
    {
        var adminUser = new User
        {
            Username = "admin",
            Password = BCrypt.Net.BCrypt.HashPassword("admin123"),
            Role = "Admin"
        };

        dbContext.Users.Add(adminUser);
        dbContext.SaveChanges();
        Console.WriteLine("✅ Admin user created.");
    }

    // ✅ Optional: Load other users from JSON if DB empty
    var jsonFilePath = Path.Combine(AppContext.BaseDirectory, "data", "users_db.json");
    if (File.Exists(jsonFilePath) && !dbContext.Users.Any(u => u.Role != "Admin"))
    {
        var jsonString = File.ReadAllText(jsonFilePath);
        var usersFromJson = JsonSerializer.Deserialize<List<User>>(jsonString);

        if (usersFromJson != null)
        {
            foreach (var user in usersFromJson.Where(u => u.Role != "Admin"))
            {
                if (!user.Password.StartsWith("$2a$"))
                {
                    user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                }
            }
            dbContext.Users.AddRange(usersFromJson.Where(u => u.Role != "Admin"));
            dbContext.SaveChanges();
            Console.WriteLine("✅ Other users added from JSON.");
        }
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
