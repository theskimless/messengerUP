using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationTest1.Models
{
    public class MessageViewModel
    {
        public int Type { get; set; }
        public string Data { get; set; }
        public string Date { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
    }
}
