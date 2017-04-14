using SmartExternalEntity.ResponseEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SmartVerify
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class VerifyUserAuthorize : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase context)
        {
            //base.AuthorizeCore(context);

            //验证用户            
            var user = UserHelper.Instance.GetUser();

            if (user != null && user.UserId > 0)
            {                
                return true;
            }
            else
            {
                context.Response.StatusCode = 403;
                return false;
            }
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            base.HandleUnauthorizedRequest(filterContext);


            if (filterContext.HttpContext.Response.StatusCode == 403)
            {
                if (filterContext.HttpContext.Request.Url != null)
                {
                    if (filterContext.HttpContext.Request.IsAjaxRequest())
                    {

                        var result = new JsonResult { Data = new ResponseBase { IsSuccess = false, ErrorCode = 403, ErrorMessage = "未登录" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                        filterContext.Result = result;
                    }
                    else
                    {
                        if (filterContext.HttpContext.Request.Url != null)
                            filterContext.Result = new RedirectResult(SmartConstArgs.SmartUrl + "Login/Index?url=" + filterContext.HttpContext.Server.UrlEncode(filterContext.HttpContext.Request.Url.ToString()));
                    }

                }

            }

        }


    }
}
