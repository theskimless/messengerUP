using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AuthenticationTest1.Models;
using Microsoft.AspNetCore.Authorization;
using AuthenticationTest1.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using AuthenticationTest1.Services;
using System.IO;
using Newtonsoft.Json;
using AuthenticationTest1.ViewModels;

namespace AuthenticationTest1.Controllers
{
    public class HomeController : Controller
    {
        ApplicationDbContext dbContext;
        IAuthService _authService;
        IFileWriter _fileWriter;
        public HomeController(ApplicationDbContext context, IAuthService authService, IFileWriter fileWriter)
        {
            dbContext = context;
            _authService = authService;
            _fileWriter = fileWriter;
        }

        [Authorize(PermissionItem.User, PermissionAction.Access)]
        public string LoadAudio(IFormFile data, string fname)
        {
            if (data != null)
            {
                var result = _fileWriter.WriteFile(data, Guid.NewGuid().ToString() + fname);
                if (result.Result.success)
                {
                    return result.Result.fileName;
                }
                else
                {
                    //HANDLE ERROR
                }
            }
            Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            return "";
        }

        [Authorize(PermissionItem.User, PermissionAction.Access)]
        public string LoadFile(IFormFile chat_formfile)
        {
            if(chat_formfile != null)
            {
                var result = _fileWriter.WriteFile(chat_formfile);
                if(result.Result.success)
                {
                    return result.Result.fileName;
                }
                else
                {
                    //HANDLE ERROR
                }
            }
            return "";
        }

        [Authorize(PermissionItem.User, PermissionAction.Access)]
        public IActionResult Index()
        {
            string id = _authService.GetUserId(HttpContext.Request.Cookies);
            ViewBag.login = _authService.GetUserById(id).Login;

            //var group = dbContext.GroupUsers.FirstOrDefault(p => p.GroupId == 24);
            //dbContext.GroupUsers.Remove(group);

            //var g = dbContext.Groups.FirstOrDefault(p => p.Id == 24);
            //dbContext.Groups.Remove(g);

            //dbContext.SaveChanges();

            return View();
        }

        [Authorize(PermissionItem.User, PermissionAction.Access)]
        [HttpGet]
        public IActionResult DeleteGroup(int id)
        {
            User user = _authService.GetUser(HttpContext.Request.Cookies);
            if(user != null)
            {
                Group group = dbContext.GroupUsers.Include(p => p.Group).FirstOrDefault(p => p.GroupId == id && p.User == user)?.Group;
                if(group != null)
                {
                    dbContext.Groups.Remove(group);
                    dbContext.SaveChanges();
                }
            }

            return RedirectToAction("Index");
        }

        [Authorize(PermissionItem.User, PermissionAction.Access)]
        [HttpGet]
        public string FindUser(int groupType, string name)
        {
            List<UserViewModel> users = new List<UserViewModel>();
            if(groupType == 0)
            {
                var user = dbContext.Users.FirstOrDefault(p => p.Login == name || p.Email == name);
                if (user != null)
                {
                    users.Add(new UserViewModel { Login = user.Login, Email = user.Email, Avatar = user.Avatar });
                    return JsonConvert.SerializeObject(users);
                }
            }
            else
            {
                var names = name.Split(',');

                for(int i = 0; i < names.Length; i++)
                {
                    var user = dbContext.Users.FirstOrDefault(p => p.Login == names[i].Trim() || p.Email == names[i].Trim());

                    if (user == null) return "";
                    else users.Add(new UserViewModel { Login = user.Login, Email = user.Email, Avatar = user.Avatar });
                }
                if(users.Count > 0) return JsonConvert.SerializeObject(users);
            }
            return "";
        }

        [Authorize(PermissionItem.User, PermissionAction.Access)]
        [HttpGet]
        public IActionResult CreateGroup()
        {
            string id = _authService.GetUserId(HttpContext.Request.Cookies);
            ViewBag.id = id;

            return View();
        }

        [Authorize(PermissionItem.User, PermissionAction.Access)]
        [HttpPost]
        public IActionResult CreateGroup(string name, IFormFile file, string def_avatar, string users, int group_type)
        {
            User user = null;

            if (group_type == 1)
            {
                if ((name == null || name.Length < 1))
                    ModelState.AddModelError("", "Введите название группы");

                if (file != null && def_avatar != null)
                    ModelState.AddModelError("", "Ошибка");

                if (file == null && def_avatar == null)
                    ModelState.AddModelError("", "Выберите картинку");
            }
            else if(group_type == 0)
            {
                if (users == null)
                {
                    ModelState.AddModelError("", "Добавьте пользователя");
                }
                else
                {
                    if (users.Split(',').Count() >= 2 && users.Split(',')[1] != "")
                    {
                        ModelState.AddModelError("", "Чтобы добавить больше пользователей создайте группу");
                    }

                    user = _authService.GetUser(HttpContext.Request.Cookies);
                    if (user.Login == users.Split(',')[0] || user.Email == users.Split(',')[0])
                    {
                        ModelState.AddModelError("", "Вы не можете добавить сами себя");
                    }
                }
            }

            if (ModelState.ErrorCount > 0)
                return View();

            //AVATAR
            string avatar = def_avatar;
            if(group_type == 1 && file != null)
            {
                var imgWriteResult = _fileWriter.WriteFile(file);
                if (imgWriteResult.Result.success)
                {
                    avatar = imgWriteResult.Result.fileName;
                }
                else ModelState.AddModelError("", "Ошибка");
            }

            if (ModelState.ErrorCount > 0)
                return View();

            Group group = new Group { Name = name, Type = group_type, Avatar = avatar };
            dbContext.Groups.Add(group);

            if(user == null) user = _authService.GetUser(HttpContext.Request.Cookies);
            if (user != null)
            {
                dbContext.GroupUsers.Add(new GroupUser { Group = group, User = user });
            }

            if (users != null)
            {
                foreach(var userName in users.Split(','))
                {
                    if((user = dbContext.Users.FirstOrDefault(p => p.Login == userName.Trim() || p.Email == userName.Trim())) != null && userName != "")
                    {
                        dbContext.GroupUsers.Add(new GroupUser { Group = group, User = user });
                    }
                }
            }

            dbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
