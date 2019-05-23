using AuthenticationTest1.Data;
using AuthenticationTest1.Models;
using AuthenticationTest1.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationTest1
{
    public class ChatHub : Hub
    {
        private enum GroupTypes { Chat = 0, Group = 1 };

        private ApplicationDbContext dbContext;
        private IAuthService authService;
        private User User;
        public ChatHub(ApplicationDbContext context, IAuthService authService)
        {
            dbContext = context;
            this.authService = authService;
        }

        //NUMBER OF MESSAGES ARE SENT TO USER WHEN CONNECTED
        private const int MessagesLoadedNumber = 16;
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();

            //Check if authenticated
            IRequestCookieCollection Cookies = Context.GetHttpContext().Request.Cookies;
            if ((User = authService.GetUser(Cookies)) == null)
            {
                await Clients.Caller.SendAsync("AuthenticationFailed");
            }
            //using(var fs = new System.IO.FileStream(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "log.txt"), System.IO.FileMode.Append))
            //{
            //    byte[] bytes = Encoding.UTF8.GetBytes("\nConnected: " + User.Login + ", Login: " + Context.ConnectionId);
            //    fs.Write(bytes, 0, bytes.Length);
            //}

            //Send list of groups
            try
            {
                string id = authService.GetUserId(Cookies);
                var groups = dbContext.GroupUsers.Where(p => p.UserId == id).Select(p => p.Group);

                List<GroupViewModel> groupsInfo = new List<GroupViewModel>();
                foreach (var group in groups.ToList())
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, group.Id.ToString());

                    string groupName;
                    string avatar = group.Avatar;

                    if (group.Type == (int)GroupTypes.Chat)
                    {
                        var gp = dbContext.GroupUsers.Include(p => p.User).FirstOrDefault(p => p.GroupId == group.Id && p.UserId != id)?.User;

                        avatar = gp?.Avatar;
                        groupName = gp?.Login;
                    }
                    else groupName = group.Name;

                    var messages = dbContext.Messages.Where(p => p.Group == group).Skip(Math.Max(0, dbContext.Messages.Where(p => p.Group == group).Count() - MessagesLoadedNumber - 2));
                    List<MessageViewModel> lastMessagesList = new List<MessageViewModel>();
                    foreach (var message in messages)
                    {
                        lastMessagesList.Add(new MessageViewModel
                        {
                            Type = message.Type,
                            Data = message.Data,
                            Date = message.Date.ToString("yyyy-MM-ddTHH:mm:ss"),
                            UserId = message.UserId,
                            UserName = message.User.Login
                        });
                    }


                    groupsInfo.Add(new GroupViewModel { GroupId = group.Id, GroupName = groupName ?? "", GroupType = group.Type, LastMessages = lastMessagesList, Avatar = avatar });
                }
                await Clients.Caller.SendAsync("LoadGroups", JsonConvert.SerializeObject(groupsInfo));
            }
            catch (Exception e)
            {

            }
        }

        //On SelectGroup
        public async Task SelectGroup(int groupId)
        {
            //List<Message> messagesList = dbContext.Groups.Include(p => p.Messages).FirstOrDefault(p => p.Id == groupId).Messages.ToList();
            List<Message> messagesList = dbContext.Messages.Where(p => p.Group.Id == groupId).Include(p => p.User).ToList();
            List<MessageViewModel> messagesVmList = new List<MessageViewModel>();
            if (messagesList != null)
            {
                for(int i = 0; i < messagesList.Count; i++)
                {
                    messagesVmList.Add(new MessageViewModel {
                        Type = messagesList[i].Type,
                        Data = messagesList[i].Data,
                        Date = messagesList[i].Date.ToString("yyyy-MM-ddTHH:mm:ss"),
                        UserId = messagesList[i].UserId,
                        UserName = messagesList[i].User.Login
                    });
                }
                await Clients.Caller.SendAsync("SelectGroup", JsonConvert.SerializeObject(messagesVmList));
            }
            else
            {
                await Clients.Caller.SendAsync("SelectGroup", "");

            }
        }

        private enum MessageType { Text = 0}
        //On SendMessage
        public async Task SendMessage(int groupId, int type, string message)
        {
            //Check if authenticated
            IRequestCookieCollection Cookies = Context.GetHttpContext().Request.Cookies;
            if ((User = authService.GetUser(Cookies)) == null)
            {
                await Clients.Caller.SendAsync("AuthenticationFailed");
            }

            var groupUser = dbContext.GroupUsers.Include(p => p.Group).FirstOrDefault(p => p.GroupId == groupId && p.UserId == User.Id);
            if (groupUser != null)
            {
                MessageViewModel messageObj = new MessageViewModel
                {
                    Type = type ,
                    Data = message,
                    Date = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),
                    UserId = User.Id,
                    UserName = User.Login
                };
                groupUser.Group.Messages.Add(new Message { User = User, Type = type, Data = message, Date = DateTime.Now });
                await dbContext.SaveChangesAsync();

                await Clients.Group(groupId.ToString()).SendAsync("SendMessage", groupId, JsonConvert.SerializeObject(messageObj));
            }
        }

        public async Task LoadMoreMessages(int groupId, int numberOfMessages)
        {
            //var messages = dbContext.Messages.Where(p => p.GroupId == groupId).Skip(1).SkipLast(1).ToList();

            List<Message> messagesList = dbContext.Messages.Where(p => p.Group.Id == groupId).Include(p => p.User).ToList().SkipLast(numberOfMessages).TakeLast(15).ToList();

            List<MessageViewModel> messagesVmList = new List<MessageViewModel>();
            if (messagesList != null)
            {
                for (int i = 0; i < messagesList.Count; i++)
                {
                    messagesVmList.Add(new MessageViewModel
                    {
                        Type = messagesList[i].Type,
                        Data = messagesList[i].Data,
                        Date = messagesList[i].Date.ToString("yyyy-MM-ddTHH:mm:ss"),
                        UserId = messagesList[i].UserId,
                        UserName = messagesList[i].User.Login
                    });
                }
                await Clients.Caller.SendAsync("LoadMoreMessages", JsonConvert.SerializeObject(messagesVmList));
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            try
            {
                var user = authService.GetUser(Context.GetHttpContext().Request.Cookies);
                using (var fs = new System.IO.FileStream(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "LOGGGG.txt"), System.IO.FileMode.Append))
                {
                    byte[] bytes = Encoding.UTF8.GetBytes("\nDis: " + user.Login + ", Login: " + Context.ConnectionId);
                    fs.Write(bytes, 0, bytes.Length);
                }
            }
            catch(Exception e)
            {

            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
