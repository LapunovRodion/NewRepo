using Microsoft.AspNetCore.Mvc;
using test2.Models;

namespace test2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExcelFilesPresentation : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExcelFilesPresentation(ApplicationDbContext context)
        {
            _context = context;
        }

        // Получение данных по Excel-файлу
        [HttpGet("GetFileData/{fileId}")]
        public IActionResult GetFileData(int fileId)
        {
            // Получаем данные по конкретному файлу Excel
            var accountData = _context.Account
                .Where(a => a.ExcelFileId == fileId)
                .ToList();

            if (accountData == null || !accountData.Any())
            {
                return NotFound("No data found for the selected file.");
            }

            return Ok(accountData);
        }
    }
}