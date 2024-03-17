using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Entity
{
    public class Service
    {
        [Key]
        public int ServiceId { get; set; }
        public string ServiceTitle { get; set; }
        public string ServiceName { get; set; }
        public  decimal Price { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
        public virtual ICollection<Feedback> Feedbacks  { get; set; }
        public virtual ICollection<Promotion> Promotions { get; set; }
        public virtual ICollection<Image> Images { get; set; }
        public virtual ICollection<BookingDetail> BookingDetails{ get; set; }
        public virtual ICollection<ServiceAvailableInDay> ServiceAvailableInDays { get; set; }
    }
}
