using HomeERP.Services;
using HomeERP.Services.Utils.FileService;
using Logistics.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDBContext>(options => options.UseNpgsql("Server=localhost;Port=5432;Database=HomeERP;Username=postgres;Password=admin"));
builder.Services.AddScoped<EAVService>();
builder.Services.AddScoped<FileService>();
builder.Services.AddScoped<FileOverviewService>();

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
    pattern: "{controller=EAV}/{action=Explorer}"
);


app.Run();
