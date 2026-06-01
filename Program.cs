using EventEase.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add MVC (Controllers + Views)
builder.Services.AddControllersWithViews();

// Connect to the database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration
        .GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Standard error handling
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// Set Search as the homepage
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Bookings}/{action=Search}/{id?}");

app.Run();