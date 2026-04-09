using Microsoft.EntityFrameworkCore;
using StudentApi.Data;
using StudentApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Добавляем контроллеры в приложение
builder.Services.AddControllers();

// НАСТРОЙКА SWAGGER (добавлен правильный пакет)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();  // Теперь эта строка работает

// НАСТРОЙКА ПОДКЛЮЧЕНИЯ К БАЗЕ ДАННЫХ POSTGRESQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Добавляем DbContext в приложение
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));  // Используем PostgreSQL

// Регистрируем сервисы
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ApplicationService>();

// Регистрация HTTP клиента для ManagerApi
builder.Services.AddHttpClient<ManagerApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ManagerApi:BaseUrl"] ?? "http://localhost:5228");
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddScoped<ManagerDataService>();

// Разрешаем CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// НАСТРОЙКА SWAGGER (теперь работает)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// АВТОМАТИЧЕСКОЕ СОЗДАНИЕ БАЗЫ ДАННЫХ
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
}

app.UseCors("AllowVueApp");
app.UseAuthorization();
app.MapControllers();

app.Run();