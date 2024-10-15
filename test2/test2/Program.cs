using test2.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;

var builder = WebApplication.CreateBuilder(args);

// ��������� ������� ��� ��������� �������� ����� API � ������������ Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "My API",
        Version = "v1"
    });
});

// ��������� ������� � ���������
builder.Services.AddControllersWithViews();

// ��������� �������� ���� ������ � �������������� SQL Server � ���������� ��������� ������� ��� �����
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
     sqlServerOptions => sqlServerOptions.EnableRetryOnFailure(
            maxRetryCount: 5, // ���������� �������
            maxRetryDelay: TimeSpan.FromSeconds(10), // ������������ �������� ����� ���������
            errorNumbersToAdd: null))); // �������������� ������ ������, ��� ������� ������ ��������� �������

// ������������ ExcelParserService ��� ������������� � �������� ������������
builder.Services.AddScoped<ExcelParserService>();

var app = builder.Build();

// ����������� �������������, ����������� ����� � �����������
app.UseRouting();
app.UseStaticFiles();
app.UseAuthorization();

// ��������� ��������� ��������� HTTP-�������� ��� ������ ����������
if (app.Environment.IsDevelopment())
{
    // ��������� ��������� Swagger ��� ������������ API
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API v1");
        c.RoutePrefix = string.Empty; // ����� ������� Swagger UI �� ������� ��������
    });
    app.UseExceptionHandler("/Home/Error"); // ���������� ���������� ��� ������ ����������
    app.UseHsts(); // ���������� HSTS ��� ��������� ������������
}

// ����������� �������� ����� ��� ������������� ������������
app.UseEndpoints(endpoints =>
{
    _ = endpoints.MapControllerRoute(
        name: "default",
       pattern: "{controller=Account}/{action=Index}/{excelFileId?}");
});

// ��������� ��������� ������������
app.MapControllers();

// ��������� ����������
app.Run();