using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationTest1.Models
{
    public class GroupViewModel
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public int GroupType { get; set; }
        public string Avatar { get; set; }
        public List<MessageViewModel> LastMessages { get; set; }
    }
}
