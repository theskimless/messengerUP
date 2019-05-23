using AuthenticationTest1.Data;
using AuthenticationTest1.Models;
using AuthenticationTest1.ViewModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationTest1
{
    public class AuthService : IAuthService
    {
        private const int AuthCookieDays = 5;

        private ApplicationDbContext dbContext;
        public AuthService(ApplicationDbContext context)
        {
            dbContext = context;
        }

        public bool IsAuthenticated(IRequestCookieCollection Cookies)
        {
            if (GetUser(Cookies) != null) return true;
            return false;
        }

        public User GetUserByLoginOrEmail(string login)
        {
            return dbContext.Users.FirstOrDefault(p => p.Login == login || p.Email == login);
        }
        public string GetUserId(IRequestCookieCollection Cookies)
        {
            return GetUser(Cookies)?.Id;
        }
        public User GetUserById(string UserId)
        {
            return dbContext.Users.FirstOrDefault(p => p.Id == UserId);
        }
        public User GetUser(IRequestCookieCollection Cookies)
        {
            if (Cookies.ContainsKey("AuthKey"))
            {
                return GetUser(Cookies["AuthKey"]);
            }
            else return null;
        }
        public User GetUser(string AuthKey)
        {
            if(AuthKey != null)
            {
                return dbContext.Users.FirstOrDefault(p => p.AuthKey == AuthKey);
            }
            else return null;
        }

        public static string GenerateSHA256(string data)
        {
            using (SHA256 sha265hash = SHA256.Create())
            {
                byte[] bytes = sha265hash.ComputeHash(Encoding.UTF8.GetBytes(data));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
        
        public bool SignIn(string login, string password, HttpResponse Response)
        {
            User user = dbContext.Users.FirstOrDefault( p => (p.Login == login || p.Email == login) && p.Password == GenerateSHA256(password) );
            if (user != null)
            {
                var expirationDate = DateTime.Now.AddDays(AuthCookieDays).ToString("r");
                Response.Headers.Add("Set-Cookie", $"AuthKey={user.AuthKey};Expires={expirationDate};Path=/");
                return true;
            }
            else return false;
        }

        public void RegisterUser(string login, string email, string password, HttpResponse Response)
        {
            string authKey = Guid.NewGuid().ToString();
            string UserId = Guid.NewGuid().ToString();
            dbContext.Users.Add(new User { Id = UserId, Login = login, Email = email, Password = GenerateSHA256(password), AuthKey = authKey });
            dbContext.SaveChanges();

            var expirationDate = DateTime.Now.AddDays(AuthCookieDays).ToString("r");
            Response.Headers.Add("Set-Cookie", $"AuthKey={authKey};Expires={expirationDate};Path=/");
        }

        public void LogOut(IRequestCookieCollection RequestCookies, HttpResponse Response)
        {
            if (RequestCookies.ContainsKey("AuthKey"))
            {
                var expirationDate = DateTime.MinValue.ToString("r");
                Response.Headers.Add("Set-Cookie", $"AuthKey=;Expires={expirationDate};Path=/");
            }
        }
    }
}
