using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Entity
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Email { get; set; }
        public string Status { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        
        public int RoleId { get; set; }
        public virtual Role Role { get; set; }

        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual ICollection<Service> Services{ get; set; }
        public virtual ICollection<Booking> Bookings { get; set; }
        public virtual ICollection<TransactionHistory> TransactionHistories { get; set; }
        public virtual ICollection<Room> Rooms{ get; set; }
        public virtual ICollection<Feedback> Feedbacks{ get; set; }
    }
}

