using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Entity
{
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }
        public DateTime BookingDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }

        public virtual Contract Contract { get; set; }

        public virtual ICollection<BookingDetail> BookingDetails { get; set; }
        public virtual ICollection<Deposit> Deposits{ get; set; }

    }
}
