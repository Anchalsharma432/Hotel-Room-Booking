
using HospitalityProject.Models;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.EntityFrameworkCore;
using MySql.EntityFrameworkCore.Extensions;
using Org.BouncyCastle.Crypto.Tls;
using static System.Formats.Asn1.AsnWriter;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//builder.Services.AddDbContext<HotelDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("HotelConnection")));

var connectionString = builder.Configuration.GetConnectionString("HospitalityDbConnection");

builder.Services.AddDbContext<HotelDbContext>(options =>
    // This line MUST be active and correctly spelled
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 35)))
);

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<HotelDbContext>();
    dbContext.Database.EnsureCreated();  // Creates DB and tables with mock data
}


// Configure middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
