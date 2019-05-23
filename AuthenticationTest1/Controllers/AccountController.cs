using AuthenticationTest1.Data;
using AuthenticationTest1.Models;
using AuthenticationTest1.Services;
using AuthenticationTest1.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationTest1.Controllers
{
    public class AccountController : Controller
    {
        private ApplicationDbContext dbContext;
        private IAuthService authService;
        private IFileWriter _fileWriter;
        public AccountController(ApplicationDbContext context, IAuthService authService, IFileWriter fileWriter)
        {
            dbContext = context;
            this.authService = authService;
            _fileWriter = fileWriter;
        }

        //LOGIN
        [Authorize(PermissionItem.Guest, PermissionAction.Access)]
        public IActionResult Login()
        {
            return View();
        }

        [Authorize(PermissionItem.Guest, PermissionAction.Access)]
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (authService.SignIn(model.Login, model.Password, HttpContext.Response))
                {
                    return RedirectToAction("Index", "Home");
                }


                ModelState.AddModelError("", "Ошибка входа. Проверьте введенные данные");
                return View(model);
            }
            return View(model);
        }

        //REGISTER
        [Authorize(PermissionItem.Guest, PermissionAction.Access)]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [Authorize(PermissionItem.Guest, PermissionAction.Access)]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Password.Count() > 128)
                    ModelState.AddModelError("Password", "Максимальная длина пароля 128 символов");

                if(model.Password.Count() < 6)
                    ModelState.AddModelError("Password", "Минимальная длина пароля 6 символов");

                if(authService.GetUserByLoginOrEmail(model.Login) != null)
                    ModelState.AddModelError("Login", "Пользователь с таким логином уже существует");

                if (authService.GetUserByLoginOrEmail(model.Email) != null)
                    ModelState.AddModelError("Email", "Пользователь с таким E-mail уже существует");

                if(ModelState.ErrorCount > 0)
                {
                    return View(model);
                }

                authService.RegisterUser(model.Login, model.Email, model.Password, HttpContext.Response);

                return RedirectToAction("ManageProfile");
            }
            ModelState.AddModelError("", "Ошибка регистрации");
            return View(model);
        }

        [HttpGet]
        [Authorize(PermissionItem.User, PermissionAction.Access)]
        public IActionResult ManageProfile()
        {
            var user = authService.GetUser(HttpContext.Request.Cookies);
            if (user != null) return View("ManageProfile", new UserViewModel { Avatar = user.Avatar, Email = user.Email, Login = user.Login });

            return View();
        }

        [HttpPost]
        [Authorize(PermissionItem.User, PermissionAction.Access)]
        public IActionResult ManageProfile(IFormFile file, string def_avatar)
        {
            User user = authService.GetUser(HttpContext.Request.Cookies);
            if(user != null)
            {
                if (file != null)
                {
                    var result = _fileWriter.WriteFile(file);
                    if (result.Result.success)
                    {
                        user.Avatar = result.Result.fileName;
                        dbContext.SaveChanges();
                    }
                    else
                    {
                        //HANDLE ERROR
                    }
                }
                else
                {
                    user.Avatar = def_avatar;
                    dbContext.SaveChanges();
                }
                return View("ManageProfile", new UserViewModel { Avatar = user?.Avatar, Login = user?.Login, Email = user?.Email});
            }
            return View("ManageProfile", null);
        }

        public IActionResult LogOut()
        {
            authService.LogOut(HttpContext.Request.Cookies, HttpContext.Response);

            return RedirectToAction("Login", "Account");
        }
    }
}
