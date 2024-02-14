using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Entity
{
    public class Contract
    {
        [Key]
        public int ContractId { get; set; }
        public string LinkFile { get; set; }
        public int  Status { get; set; }
        public int BookingServiceId { get; set; }
        public virtual Booking BookingService{ get; set; }
    }
}
