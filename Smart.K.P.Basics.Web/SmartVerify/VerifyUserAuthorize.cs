using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SmartVerify
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class VerifyUserAuthorize : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext context)
        {
            base.OnAuthorization(context);

            //验证用户
            var token =UserHelper.GetUserToken();
            if (token != null && string.IsNullOrEmpty(token) == false)
            {
                return;
            }
            context.HttpContext.Response.Redirect(SmartConstArgs.SmartUrl + "Login/Index" + "?returnUrl=" + context.HttpContext.Request.Url.ToString());
            context.Result = new HttpUnauthorizedResult();
        }
    }
}
