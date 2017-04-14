using SmartCache;
using SmartCommon.SingleTonHelper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace SmartVerify
{
    public class UserHelper: SingleTon<UserHelper>
    {
        public UserInfo GetUser()
        {
            var token = GetUserToken();
            if (!string.IsNullOrEmpty(token))
            {
                var user = ValidateToken(token);
                if (user != null && user.UserId > 0)
                {
                    CookieHelper.SetCookie(SmartConstArgs.UserToken, token, DateTime.Now.AddMonths(1));
                    return user;
                }
            }
            return null;
        }

        public void SignOut()
        {
            FormsAuthentication.SignOut();
            //var token = GetUserToken();
            CookieHelper.ClearCookie(SmartConstArgs.UserToken);
            //重定向到登录页面   
            HttpContext.Current.Response.Redirect(SmartConstArgs.SmartUrl + "Login/Index");
        }

        private UserInfo ValidateToken(string token)
        {
            if (SmartConstArgs.SmartUrl != "")
            {
                var session = HttpContext.Current.Session;
                var user = session[token] as UserInfo;
                if (user != null && user.UserId>0)
                {
                    return user;
                }
                //通过token获取用户信息
                string userJson = string.Empty;
                
                if (!string.IsNullOrEmpty(userJson))
                {
                    user = new JavaScriptSerializer().Deserialize<UserInfo>(userJson);
                    if (user != null && user.UserId > 0)
                    {
                        session.Add(token, user);
                        session.Timeout = 60 * 12;
                        return user;
                    }
                }
            }
            return null;
        }

        public static string GetUserToken()
        {
            var token = CookieHelper.GetCookieValue(SmartConstArgs.UserToken);
            if (!string.IsNullOrEmpty(token))
            {
                return token;
            }

            token = HttpContext.Current.Request[SmartConstArgs.Token];
            return token;
        }
        public void CacheUser(string token,UserInfo userInfo,bool isRemeber)
        {
            CookieHelper.SetCookie(SmartConstArgs.UserToken,token);
            var session = HttpContext.Current.Session;
            session.Remove(token);

            session.Add(token, userInfo);
            if (isRemeber == true)
            {
                session.Timeout = 60 * 24 * 7;
             }
            
        }
    }
}
