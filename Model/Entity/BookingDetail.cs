using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Entity
{
    public class BookingDetail
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTIme { get; set; }

        public int ServiceId { get; set; }
        public virtual Service Service{ get; set; }
        public int BookingId { get; set; }
        public virtual Booking Booking{ get; set; }

        public int RoomId { get; set; }
        public virtual Room Room { get; set; }
    }
}
