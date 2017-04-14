using SmartEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartIBusiness
{
    public interface IUsersBusiness
    {
        int AddUser(T_Users user);
        int DelUser(int id);
        int DelUser(T_Users user);
        int UpdateUser(T_Users user);
        List<T_Users> GetUserList();
        T_Users GetUserByPwd(string loginName, string pwd);
        T_Users GetUserById(int id);

    }
}
