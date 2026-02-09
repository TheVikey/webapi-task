using Microsoft.EntityFrameworkCore;
using webapi_task.Applications;
using webapi_task.Applications.Services;
using webapi_task.Infrastructure;
using webapi_task.Infrastructure.Implementations;
using webapi_task.Infrastructure.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Регистрация репозиториев и Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IValueRepository, ValueRepository>();
builder.Services.AddScoped<IResultRepository, ResultRepository>();

// Регистрация сервисов
builder.Services.AddScoped<ICsvParserService, CsvParserService>();
builder.Services.AddScoped<IFileProcessingService, FileProcessingService>();
builder.Services.AddScoped<IFileQueryService, FileQueryService>();
builder.Services.AddScoped<IFileValidator, FileValidator>();
builder.Services.AddScoped<IStatisticsCalculator, StatisticsCalculator>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
