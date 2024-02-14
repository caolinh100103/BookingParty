using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Entity
{
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime SentTime { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
    }
}
