using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using test2.Models; 
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class FilesController : Controller
{
    // Контекст базы данных, используемый для взаимодействия с таблицей ExcelFile
    private readonly ApplicationDbContext _context;

    // Конструктор, принимающий контекст базы данных в качестве параметра
    public FilesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Метод для получения списка всех файлов из таблицы ExcelFile
    [HttpGet("list")]
    public IActionResult GetFiles()
    {
        try
        {
            // Получаем список всех файлов из таблицы ExcelFile
            var files = _context.ExcelFile.ToList();

            // Если файлов нет, возвращаем пустой список
            if (files == null || files.Count == 0)
            {
                return NotFound("No files found.");
            }

            // Возвращаем список файлов
            return Ok(files);
        }
        catch (Exception ex)
        {
            // Обработка ошибок, возвращаем статус ошибки сервера и сообщение об ошибке
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}