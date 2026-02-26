using Microsoft.EntityFrameworkCore;
using WebAppCooking.Data;

var builder = WebApplication.CreateBuilder(args);

// Äîáàâëÿåì ñåğâèñû
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<UserAppContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ÍÀÑÒĞÎÉÊÀ ÑÅÑÑÈÉ - İÒÎ ÂÀÆÍÎ!
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline
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

// ÂÀÆÍÎ: UseSession ÄÎËÆÅÍ áûòü çäåñü!
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();