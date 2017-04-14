using Smart.K.P.Basics.Web.Base;
using SmartBusiness;
using SmartCommon.SerializerHelper;
using SmartIBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Smart.K.P.Basics.Web.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            IUsersBusiness userBusiness = new UsersBusiness();

            var list = userBusiness.GetUserList();
            var str = SerializerHelper.ObjectToJson(list);


            //var obj = LocalCacheHelper.GetCache("key");
            //LocalCacheHelper.SetCache("key", "value", TimeSpan.FromMinutes(10));

            ViewBag.Str = str;
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}