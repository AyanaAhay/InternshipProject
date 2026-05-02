using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using StudentApi.Data;
using StudentApi.Services;


AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true); 

var builder = WebApplication.CreateBuilder(args);

// ========== Настройка контроллеров ==========
// Enum будет сериализоваться как строка в JSON (вместо числа)
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ========== Кэширование справочников ==========
builder.Services.AddMemoryCache();

// ========== Логирование ==========
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// ========== Подключение к БД ==========
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// ========== Регистрация сервисов ==========
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ApplicationService>();
builder.Services.AddScoped<ManagerDataService>();
builder.Services.AddScoped<QuestionnaireService>();
builder.Services.AddScoped<ManagerInterviewService>();

// ========== HTTP клиент для Manager API ==========
var managerApiBaseUrl = builder.Configuration["Services:ManagerApi"] ?? "http://localhost:5001";
Console.WriteLine($"[CONFIG] Manager API URL: {managerApiBaseUrl}");

builder.Services.AddHttpClient<ManagerApiClient>(client =>
{
    client.BaseAddress = new Uri(managerApiBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.DefaultRequestHeaders.Add("User-Agent", "StudentApi/1.0");
});

// HTTP клиент для Supervisor API
var supervisorApiBaseUrl = builder.Configuration["Services:SupervisorApi"] ?? "http://localhost:5002";
Console.WriteLine($"[CONFIG] Supervisor API URL: {supervisorApiBaseUrl}");

builder.Services.AddHttpClient<SupervisorApiClient>(client =>
{
    client.BaseAddress = new Uri(supervisorApiBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.DefaultRequestHeaders.Add("User-Agent", "StudentApi/1.0");
});

// Сервис 
builder.Services.AddScoped<InterviewSlotService>();
builder.Services.AddScoped<DocumentService>();
builder.Services.AddScoped<PracticeReviewService>();
builder.Services.AddScoped<StudentSupervisorLinkService>();

// ========== CORS ==========
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:5174", "http://localhost:5175", "http://localhost:5176")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// ========== Swagger ==========
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Student API V1");
    c.RoutePrefix = "swagger";
});

// ========== Применение миграций при старте ==========
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.MigrateAsync();

    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Student API started. DB migrated.");
}

app.UseCors("AllowVueApp");
app.UseAuthorization();
app.MapControllers();
app.Run();