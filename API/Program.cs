using Business;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;

var builder = WebApplication.CreateBuilder(args);

// Serilog Logger Yap�land�rmas�
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\emrelog.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

// Veritaban� ba�lant� dizesi
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Entity Framework ve DBHelper i�in yap�land�rma
builder.Services.AddSingleton(new DBHelper(connectionString));
builder.Services.AddDbContext<HelparkDbContext>(options =>
    options.UseSqlServer(connectionString));

// API Controller'lar� ekleyin
builder.Services.AddControllers();

// Swagger'� yap�land�r�n
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// �� katman� servisleri
builder.Services.AddScoped<DBHelper>(provider => new DBHelper(connectionString));
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<OtpService>();

var app = builder.Build();

// Swagger UI yap�land�rmas�
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// HTTPS y�nlendirme
app.UseHttpsRedirection();

// Y�nlendirme ve Yetkilendirme
app.UseRouting();
app.UseAuthorization();

// API Controllers'� mapleyin
app.MapControllers();

app.Run();
