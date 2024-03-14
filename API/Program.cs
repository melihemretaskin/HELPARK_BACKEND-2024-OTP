using Business;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;

var builder = WebApplication.CreateBuilder(args);

// Serilog Logger Yapýlandýrmasý
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\emrelog.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

// Veritabaný baðlantý dizesi
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Entity Framework ve DBHelper için yapýlandýrma
builder.Services.AddSingleton(new DBHelper(connectionString));
builder.Services.AddDbContext<HelparkDbContext>(options =>
    options.UseSqlServer(connectionString));

// API Controller'larý ekleyin
builder.Services.AddControllers();

// Swagger'ý yapýlandýrýn
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Ýþ katmaný servisleri
builder.Services.AddScoped<DBHelper>(provider => new DBHelper(connectionString));
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<OtpService>();

var app = builder.Build();

// Swagger UI yapýlandýrmasý
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// HTTPS yönlendirme
app.UseHttpsRedirection();

// Yönlendirme ve Yetkilendirme
app.UseRouting();
app.UseAuthorization();

// API Controllers'ý mapleyin
app.MapControllers();

app.Run();
