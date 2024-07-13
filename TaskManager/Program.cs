using Microsoft.EntityFrameworkCore;
using TaskManager.Data;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var server = Environment.GetEnvironmentVariable("DB_SERVER") ?? "mssqlserver";
var port = Environment.GetEnvironmentVariable("DB_PORT") ?? "1433";
var user = Environment.GetEnvironmentVariable("DB_USER") ?? "sa";
var password = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "Password123!";
var database = Environment.GetEnvironmentVariable("DB_DATABASE") ?? "TaskManagerDB";

builder.Services.AddDbContext<TaskManagerDBContext>(options =>
        options.UseSqlServer($"Server={server},{port};Initial Catalog={database};User ID={user};Password={password};TrustServerCertificate=True;Connection Timeout=30;"));

var app = builder.Build();

//Apply any pending migrations
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<TaskManagerDBContext>();
    context.Database.Migrate();
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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
