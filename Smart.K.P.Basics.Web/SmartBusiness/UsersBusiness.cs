using SmartIBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartEntity;
using SmartCommon.DbHelper;
using SmartCommon.ConstHelper;
using Chloe.SqlServer;

namespace SmartBusiness
{
    public class UsersBusiness : IUsersBusiness
    {
        public int AddUser(T_Users user)
        {
            throw new NotImplementedException();
        }

        public int DelUser(T_Users user)
        {
            throw new NotImplementedException();
        }

        public int DelUser(int id)
        {
            throw new NotImplementedException();
        }

        public T_Users GetUserById(int id)
        {
            throw new NotImplementedException();
        }

        public T_Users GetUserByPwd(string loginName, string pwd)
        {
            using (var context = new MsSqlContext(DbHelper.ConnectionString, DatabaseNameConst.DBTest))
            {
                var entity = context.Query<T_Users>();
                return entity.Where(e => e.LoginName == loginName && e.LoginPwd == pwd).FirstOrDefault();
            }
        }

        public List<T_Users> GetUserList()
        {
            using (var context = new MsSqlContext(DbHelper.ConnectionString, DatabaseNameConst.DBTest))
            {
                var entity = context.Query<T_Users>();
                return entity.Where(e => e.Id > 0).ToList();
            }
        }

        public int UpdateUser(T_Users user)
        {
            throw new NotImplementedException();
        }
    }
}
