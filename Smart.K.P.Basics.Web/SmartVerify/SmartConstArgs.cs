using System.Configuration;

namespace SmartVerify
{
    public class SmartConstArgs
    {
        public const string Token = "Token";
        public const string SmartUser = "SmartUser";
        public const string UserToken = "UserToken";
       
        public static string SmartUrl
        {
            get { return ConfigurationManager.AppSettings["SmartUrl"] ?? " / "; }
        }
    }
}
