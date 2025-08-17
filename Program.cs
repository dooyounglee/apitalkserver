using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using rest1.Controllers;
using rest1.Repositories;
using rest1.Services;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// PostgreSQL 연결 문자열
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Postgres
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention());

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSingleton<DbHelper>(); // 의존성 주입
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IChatRepository, ChatRepository>();
builder.Services.AddScoped<IFileRepository, FileRepository>();

var app = builder.Build();
var logger = app.Logger;

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.Use(async (context, next) =>
{
    // Console.WriteLine($"Request: {context.Request.Method} {context.Request.Path}");
    Debug.WriteLine($"Request: {context.Request.Method} {context.Request.Path}");
    // logger.LogInformation($"Request: {context.Request.Method} {context.Request.Path}");
    await next.Invoke();
});

app.UseAuthorization();

app.MapControllers();

app.Run();
