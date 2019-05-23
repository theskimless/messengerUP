using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationTest1
{
    public enum PermissionItem
    {
        User,
        Guest
    }

    public enum PermissionAction
    {
        Access
    }

    public class AuthorizeAttribute : TypeFilterAttribute
    {
        public AuthorizeAttribute(PermissionItem item, PermissionAction action) : base(typeof(AuthenticationAttr))
        {
            Arguments = new object[] { item, action };
        }
    }

    public class AuthenticationAttr : IAuthorizationFilter
    {
        private readonly PermissionItem _item;
        private readonly PermissionAction _action;
        private IAuthService authService;
        public AuthenticationAttr(PermissionItem item, PermissionAction action, IAuthService authService)
        {
            _item = item;
            _action = action;
            this.authService = authService;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            bool IsAuthenticated = authService.IsAuthenticated(context.HttpContext.Request.Cookies);
            //IsAuthenticated = false;

            if (_item == PermissionItem.User && !IsAuthenticated)
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
            }

            if(_item == PermissionItem.Guest && IsAuthenticated)
            {
                context.Result = new RedirectToActionResult("Index", "Home", null);
            }
        }
    }
}
