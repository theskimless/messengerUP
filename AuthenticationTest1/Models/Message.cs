using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationTest1.Models
{
    public class Message
    {
        public int Id { get; set; }
        public int Type { get; set; }
        public string Data { get; set; }

        public DateTime Date { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }

        public int GroupId { get; set; }
        public Group Group { get; set; }
    }
}
