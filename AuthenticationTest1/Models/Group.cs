using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationTest1.Models
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }

        public ICollection<Message> Messages { get; set; }
        public ICollection<GroupUser> GroupUsers { get; set; }

        public int Type { get; set; }

        public Group()
        {
            Messages = new List<Message>();
            GroupUsers = new List<GroupUser>();
        }
    }
}
