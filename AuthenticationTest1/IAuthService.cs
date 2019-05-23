using AuthenticationTest1.Models;
using Microsoft.AspNetCore.Http;

namespace AuthenticationTest1
{
    public interface IAuthService
    {
        bool SignIn(string login, string password, HttpResponse Response);
        void RegisterUser(string login, string email, string password, HttpResponse Response);
        bool IsAuthenticated(IRequestCookieCollection Cookies);

        string GetUserId(IRequestCookieCollection Cookies);
        User GetUserByLoginOrEmail(string login);
        User GetUserById(string UserId);
        User GetUser(IRequestCookieCollection AuthKey);
        User GetUser(string AuthKey);

        void LogOut(IRequestCookieCollection RequestCookies, HttpResponse Response);
    }
}