using System.ComponentModel.DataAnnotations.Schema;

namespace test2.Models
{
    public class Account
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal OpeningBalanceActive { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal OpeningBalancePassive { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal ClosingBalanceActive { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal ClosingBalancePassive { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Debet { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Credit { get; set; }
        public int BankId { get; set; }
        public int ClassOfTransactionNumber { get; set; }
        public int ExcelFileId { get; set; }
    }
}