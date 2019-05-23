using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationTest1.Models
{
    public class User
    {
        public string Id { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string AuthKey { get; set; }
        public string Avatar { get; set; }

        public ICollection<Message> Messages { get; set; }
        public ICollection<GroupUser> GroupUsers { get; set; }

        public User()
        {
            Messages = new List<Message>();
            GroupUsers = new List<GroupUser>();
        }
    }
}
