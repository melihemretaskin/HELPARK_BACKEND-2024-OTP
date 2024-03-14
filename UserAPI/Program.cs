using Business;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;




var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\emrelog.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddSingleton(new DBHelper(connectionString));
builder.Services.AddDbContext<HelparkDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddControllersWithViews();


builder.Services.AddScoped<DBHelper>(provider => new DBHelper(connectionString));
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<OtpService>();

var app = builder.Build();

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
