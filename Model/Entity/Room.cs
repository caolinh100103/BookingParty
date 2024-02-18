using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Entity
{
    public class Room
    {
        [Key]
        public int RoomId{ get; set; }
        public string RoomName { get; set; }
        public string Description { get; set; }
        public int Capacity { get; set; }
        public string Address { get; set; }
        public decimal Price { get; set; }
        public int Status { get; set; }
        public virtual ICollection<BookingDetail> BookingDetails { get; set; }

        public int UserId { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
        public virtual ICollection<Promotion> Promotions { get; set; }
        public virtual ICollection<Image> Images { get; set; }
    }
}
