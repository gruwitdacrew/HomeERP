using HomeERP.Domain.Common.Contexts;
using HomeERP.Domain.Common.Repositories;
using HomeERP.Logic;
using HomeERP.Logic.Utils;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDBContext>(options => 
{
    options.UseNpgsql(Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ?? "Server=localhost;Port=5432;Database=HomeERP;Username=postgres;Password=admin");
});
builder.Services.AddScoped<FileStorageContext>();

builder.Services.AddTransient(typeof(BaseEntityRepository<>));
builder.Services.AddTransient(typeof(GenericRepository<>));
builder.Services.AddTransient(typeof(FileRepository));

builder.Services.AddScoped<EAVService>();
builder.Services.AddScoped<FileOverviewService>();
builder.Services.AddScoped<ChoreService>();
builder.Services.AddScoped<ProductService>();

builder.Services.AddSingleton<TelegramBot>();
builder.Services.AddSingleton<IHostedService>(p => p.GetRequiredService<TelegramBot>());
builder.Services.AddHostedService<Notifier>();


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
