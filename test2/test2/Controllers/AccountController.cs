using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using test2.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
// http://localhost:5289/Account/Index

namespace test2.Controllers
{
    public class AccountController : Controller
    {
        // Контекст базы данных, используемый для взаимодействия с таблицей Account
        private readonly ApplicationDbContext _context;

        // Конструктор, принимающий контекст базы данных в качестве параметра
        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Метод для получения списка аккаунтов по ExcelFileId и отображения их на представлении
        public async Task<IActionResult> Index(int? excelFileId)
        {
            // Если идентификатор Excel файла не указан, возвращаем пустой список аккаунтов
            if (excelFileId == null)
            {
                return View(new List<Account>());
            }

            // Сохраняем ExcelFileId в ViewBag для дальнейшего использования на представлении
            ViewBag.ExcelFileId = excelFileId;

            // Извлекаем список аккаунтов, которые соответствуют указанному ExcelFileId
            List<Account> accounts = await _context.Account
                .Where(a => a.ExcelFileId == excelFileId)
                .ToListAsync();

            // Возвращаем представление со списком аккаунтов
            return View(accounts);
        }

        // Метод для скачивания таблицы аккаунтов в текстовом формате (txt)
        [HttpPost]
        public async Task<IActionResult> DownloadTxt(int? excelFileId)
        {
            // Если идентификатор Excel файла не указан, перенаправляем на действие Index
            if (excelFileId == null)
            {
                return RedirectToAction("Index");
            }

            // Извлекаем список аккаунтов, которые соответствуют указанному ExcelFileId
            List<Account> accounts = await _context.Account
                .Where(a => a.ExcelFileId == excelFileId)
                .ToListAsync();

            // Создаем объект StringBuilder для формирования содержимого текстового файла
            StringBuilder sb = new StringBuilder();

            // Добавляем заголовок таблицы в текстовый файл
            sb.AppendLine($"{"Id",-10}{"AccountId",-15}{"OpeningBalanceActive",-25}{"OpeningBalancePassive",-25}{"ClosingBalanceActive",-25}{"ClosingBalancePassive",-25}{"Debet",-20}{"Credit",-20}{"BankId",-10}{"ClassOfTransactionNumber",-25}{"ExcelFileId",-10}");

            // Добавляем данные каждого аккаунта в текстовый файл
            foreach (var account in accounts)
            {
                sb.AppendLine(string.Format("{0,-10}{1,-15}{2,-25:F2}{3,-25:F2}{4,-25:F2}{5,-25:F2}{6,-20:F2}{7,-20:F2}{8,-10}{9,-25}{10,-10}",
                    account.Id,
                    account.AccountId,
                    account.OpeningBalanceActive,
                    account.OpeningBalancePassive,
                    account.ClosingBalanceActive,
                    account.ClosingBalancePassive,
                    account.Debet,
                    account.Credit,
                    account.BankId,
                    account.ClassOfTransactionNumber,
                    account.ExcelFileId));
            }

            // Преобразуем строку в байтовый массив и возвращаем файл в формате txt
            byte[] fileBytes = Encoding.UTF8.GetBytes(sb.ToString());
            return File(fileBytes, "text/plain", "accounts.txt");
        }
    }
}