using Microsoft.EntityFrameworkCore;
using WebAppCooking.Data;

var builder = WebApplication.CreateBuilder(args);

// Добавляем контекст базы данных
builder.Services.AddDbContext<UserAppContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString")));

// Добавляем сессии
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseSession(); // Важно: добавить UseSession

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();