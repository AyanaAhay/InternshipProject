using Microsoft.EntityFrameworkCore;
using StudentApi.Data;
using StudentApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Добавляем контроллеры
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Настройка логирования
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Настройка подключения к БД
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Регистрация сервисов
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ApplicationService>();
builder.Services.AddScoped<ManagerDataService>();

// Настройка HTTP клиента для Manager API
// ИЗМЕНЕНО: читаем из секции "Services"
var managerApiBaseUrl = builder.Configuration["Services:ManagerApi"] ?? "http://localhost:5001";
var timeoutSeconds = 30;

Console.WriteLine($"[CONFIG] Manager API URL: {managerApiBaseUrl}");

builder.Services.AddHttpClient<ManagerApiClient>(client =>
{
    client.BaseAddress = new Uri(managerApiBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.DefaultRequestHeaders.Add("User-Agent", "StudentApi/1.0");
});

// Настройка CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:5174")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Включаем Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Student API V1");
    c.RoutePrefix = "swagger";
});

// Автоматическое создание БД
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.EnsureCreatedAsync();

    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    logger.LogInformation($"Student API started. Manager API URL: {managerApiBaseUrl}");
}

app.UseCors("AllowVueApp");
app.UseAuthorization();
app.MapControllers();

app.Run();