using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartExternalEntity.RequestEntity
{
    public class LoginRequestEntity
    {
        public string LoginUserName { get; set; }
        public string LoginPwd { get; set; }
        public bool IsRemeberPwd { get; set; }
        public string ReturnUrl { get; set; }
    }
}
