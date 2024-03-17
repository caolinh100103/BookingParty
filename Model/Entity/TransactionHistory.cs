using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Model.Entity
{
    public class TransactionHistory
    {
        public int TransactionHistoryId { get; set; }
        public string? BankCode { get; set; }
        public string PaymentMethod { get; set; }
        public int? Txn_ref { get; set; }
        public string TransactionDate { get; set; }
        public decimal Amount { get; set; }
        public decimal PlatformFee { get; set; }
        public string Status { get; set; }
        public int DepositId { get; set; }
        public virtual Deposit Deposit { get; set; }
    }
}
