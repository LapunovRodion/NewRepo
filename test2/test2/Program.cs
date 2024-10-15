using test2.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;

var builder = WebApplication.CreateBuilder(args);

// Добавляем сервисы для генерации конечных точек API и документации Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "My API",
        Version = "v1"
    });
});

// Добавляем сервисы в контейнер
builder.Services.AddControllersWithViews();

// Добавляем контекст базы данных с использованием SQL Server и настройкой повторных попыток при сбоях
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
     sqlServerOptions => sqlServerOptions.EnableRetryOnFailure(
            maxRetryCount: 5, // Количество попыток
            maxRetryDelay: TimeSpan.FromSeconds(10), // Максимальная задержка между попытками
            errorNumbersToAdd: null))); // Дополнительные номера ошибок, для которых делать повторные попытки

// Регистрируем ExcelParserService для использования в качестве зависимостей
builder.Services.AddScoped<ExcelParserService>();

var app = builder.Build();

// Настраиваем маршрутизацию, статические файлы и авторизацию
app.UseRouting();
app.UseStaticFiles();
app.UseAuthorization();

// Настройка конвейера обработки HTTP-запросов для режима разработки
if (app.Environment.IsDevelopment())
{
    // Добавляем поддержку Swagger для документации API
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API v1");
        c.RoutePrefix = string.Empty; // чтобы открыть Swagger UI на главной странице
    });
    app.UseExceptionHandler("/Home/Error"); // Обработчик исключений для режима разработки
    app.UseHsts(); // Используем HSTS для повышения безопасности
}

// Настраиваем конечные точки для маршрутизации контроллеров
app.UseEndpoints(endpoints =>
{
    _ = endpoints.MapControllerRoute(
        name: "default",
       pattern: "{controller=Account}/{action=Index}/{excelFileId?}");
});

// Добавляем поддержку контроллеров
app.MapControllers();

// Запускаем приложение
app.Run();