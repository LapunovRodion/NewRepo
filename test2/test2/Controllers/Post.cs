using Microsoft.AspNetCore.Mvc;
using test2.Models;

namespace test2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Post : Controller
    {
        // Сервис для обработки файлов Excel
        private readonly ExcelParserService _excelParserService;

        // Конструктор, принимающий сервис для обработки Excel файлов в качестве параметра
        public Post(ExcelParserService excelParserService)
        {
            _excelParserService = excelParserService;
        }

        // Метод для обработки файла по указанному пути
        [HttpPost("process")]
        public IActionResult ProcessFile([FromBody] string filePath)
        {
            // Проверяем, что путь к файлу не пустой
            if (string.IsNullOrEmpty(filePath))
            {
                return BadRequest("File path cannot be empty.");
            }

            try
            {
                // Импортируем данные из Excel файла с помощью сервиса
                _excelParserService.ImportDataFromExcel(filePath);
                return Ok("File processed successfully.");
            }
            catch (Exception ex)
            {
                // Обрабатываем исключения и возвращаем сообщение об ошибке
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}