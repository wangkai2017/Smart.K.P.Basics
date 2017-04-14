using SmartBusiness;
using SmartCache;
using SmartCommon.ConvertHelper;
using SmartCommon.LogHelper;
using SmartEntity;
using SmartExternalEntity.RequestEntity;
using SmartIBusiness;
using SmartVerify;
using System;
using System.Web.Mvc;

namespace Smart.K.P.Basics.Web.Controllers
{
    public class LoginController : Controller
    {
        [AllowAnonymous]
        // GET: Login
        public ActionResult Index(string returnUrl)
        {
            try
            {
                var user = UserHelper.Instance.GetUser();
                if (user != null)
                {
                    if (string.IsNullOrEmpty(returnUrl) == false)
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.WriteErrorLog(ex, extContent: "获取用户失败");
            }
            return View();
        }


        [HttpPost]
        public ActionResult LoginIn(LoginRequestEntity entity)
        {
            IUsersBusiness userBusiness = new UsersBusiness();
            var user = new T_Users();
            var loginName = ConvertHelper.ClearInputText(entity.LoginUserName);
            var loginPwd = ConvertHelper.ClearInputText(entity.LoginPwd);

            user = userBusiness.GetUserByPwd(loginName, EncryptionAndDecryptHelper.GetMD5(loginPwd));

            if (user == null || user.Id <= 0)
            {
                return RedirectToAction("Index",entity);
            }

            //生成token
            var token = EncryptionAndDecryptHelper.EncryptString(user.Id + "_" + ConvertHelper.ToISO8601DateString(DateTime.Now));
            var userInfo = new UserInfo()
            {
                UserId = user.Id,
                UserName = user.UserName,
                Token = token
            };

            UserHelper.Instance.CacheUser(token, userInfo, entity.IsRemeberPwd);

            if (string.IsNullOrEmpty(entity.ReturnUrl) == false)
            {
                return Redirect(entity.ReturnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }

        [HttpPost]
        public ActionResult LoginOut()
        {
            UserHelper.Instance.SignOut();
            return View();
        }

    }
}