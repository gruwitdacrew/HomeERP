using HomeERP.Services;
using Logistics.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDBContext>(options => options.UseNpgsql("Server=db;Port=5432;Database=HomeERP;Username=postgres;Password=admin"));
builder.Services.AddScoped<EntityService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDBContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Entity}/{action=Explorer}"
);

app.MapControllerRoute(
    name: "EntityCreate",
    pattern: "{controller=Entity}/{action=Create}");


app.Run();
