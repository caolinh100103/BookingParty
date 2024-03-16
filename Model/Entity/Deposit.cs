using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Entity
{
    public class Deposit
    {
        [Key]
        public int DepositId { get; set; }
        public string Content { get; set; }
        public string Title { get; set; }
        public float Percentage { get; set; }
        public int BookingId { get; set; }
        public virtual Booking Booking { get; set; }
        public virtual TransactionHistory TransactionHistory { get; set; }
    }
}
