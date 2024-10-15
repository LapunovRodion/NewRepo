using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace test2.Models
{
    // Контекст базы данных, используемый для взаимодействия с таблицами базы данных
    public class ApplicationDbContext : DbContext
    {
        // Таблица для хранения данных об аккаунтах
        public DbSet<Account> Account { get; set; }

        // Таблица для хранения данных о классах транзакций
        public DbSet<ClassOfTransaction> ClassOfTransaction { get; set; }

        // Таблица для хранения данных о банках
        public DbSet<Bank> Bank { get; set; }

        // Таблица для хранения данных об Excel файлах
        public DbSet<ExcelFile> ExcelFile { get; set; }

        // Конструктор, принимающий параметры конфигурации контекста базы данных
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}