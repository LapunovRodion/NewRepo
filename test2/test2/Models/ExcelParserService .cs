using ClosedXML.Excel;
using DocumentFormat.OpenXml.Math;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text.RegularExpressions;

// "F:\dev\ОСВ для тренинга (2).xlsx"
namespace test2.Models
{
    public class ExcelParserService
    {
        // Контекст базы данных, используемый для сохранения данных, извлеченных из Excel файла
        private readonly ApplicationDbContext _context;

        // Конструктор, принимающий контекст базы данных в качестве параметра
        public ExcelParserService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Метод для импорта данных из Excel файла по указанному пути
        public void ImportDataFromExcel(string filePath)
        {
            // Проверяем, существует ли файл по указанному пути
            if (!System.IO.File.Exists(filePath))
            {
                throw new FileNotFoundException("File not found.", filePath);
            }

            using (var workbook = new XLWorkbook(filePath))
            {
                string fileName = Path.GetFileName(filePath);
                var worksheet = workbook.Worksheet(1);
                var Bank = new Bank();
                var ExcelFile = new ExcelFile();
                var firstRow = worksheet.Row(1);

                // Устанавливаем имя Excel файла и сохраняем в базе данных
                ExcelFile.Name = fileName;
                _context.ExcelFile.Add(ExcelFile);
                _context.SaveChanges();

                // Извлекаем период начала и окончания из третьей строки Excel файла
                var thirdRow = worksheet.Row(3);
                string input = thirdRow.Cell(1).GetValue<string>();
                string pattern = @"(\d{2}\.\d{2}\.\d{4})";
                MatchCollection matches = Regex.Matches(input, pattern);
                var PeriodStart = DateTime.Parse(matches[0].Value);
                var PeriodEnd = DateTime.Parse(matches[1].Value);

                // Устанавливаем данные банка и сохраняем в базе данных
                Bank.ExcelFileId = ExcelFile.Id;
                Bank.Name = firstRow.Cell(1).GetValue<string>();
                Bank.Period = $" {PeriodStart:dd.MM.yyyy} по {PeriodEnd:dd.MM.yyyy}";
                _context.Bank.Add(Bank);
                _context.SaveChanges();

                // Проходимся по строкам Excel файла, начиная с 8-ой строки
                foreach (var row in worksheet.RowsUsed().Skip(7))
                {
                    // Проверяем, содержит ли строка информацию о классе транзакции
                    if (row.Cell(1).GetValue<string>().Contains("КЛАСС "))
                    {
                        var name = row.Cell(1).GetValue<string>();

                        // Проверяем наличие номера класса транзакции и добавляем в базу данных, если его еще нет
                        string CheckPattern = @"\d+";
                        Match Checkmatch = Regex.Match(name, CheckPattern);
                        if (Checkmatch.Success)
                        {
                            int number = int.Parse(Checkmatch.Value);

                            var existingTransaction = _context.ClassOfTransaction
                                .FirstOrDefault(ct => ct.Number == number);
                            if (existingTransaction == null)
                            {
                                var ClassOfTransaction = new ClassOfTransaction
                                {
                                    Name = row.Cell(1).GetValue<string>(),
                                    Number = int.Parse(Checkmatch.Value)
                                };
                                _context.ClassOfTransaction.Add(ClassOfTransaction);
                                _context.SaveChanges();
                            }
                        }
                    }

                    // Если строка содержит данные об аккаунте 
                    if (row.Cell(1).GetValue<string>().Length == 4)
                    {
                        var account = new Account
                        {
                            AccountId = int.Parse(row.Cell(1).GetValue<string>()),
                            OpeningBalanceActive = decimal.Parse(row.Cell(2).GetValue<string>()),
                            OpeningBalancePassive = decimal.Parse(row.Cell(3).GetValue<string>()),
                            Debet = decimal.Parse(row.Cell(4).GetValue<string>()),
                            Credit = decimal.Parse(row.Cell(5).GetValue<string>()),
                            ClosingBalanceActive = decimal.Parse(row.Cell(6).GetValue<string>()),
                            ClosingBalancePassive = decimal.Parse(row.Cell(7).GetValue<string>()),
                            BankId = Bank.Id,
                            ClassOfTransactionNumber = int.Parse(row.Cell(1).GetValue<string>().FirstOrDefault().ToString()),
                            ExcelFileId = ExcelFile.Id
                        };
                        _context.Account.Add(account);
                    }
                    _context.SaveChanges(); // Сохраняем данные в базе данных
                }
            }
        }
    }
}