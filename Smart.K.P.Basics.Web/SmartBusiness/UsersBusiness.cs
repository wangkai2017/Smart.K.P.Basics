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
using Chloe.MySql;

namespace SmartBusiness
{
    public class UsersBusiness : IUsersBusiness
    {
        public T_Users AddUser(T_Users user)
        {
            using (var context = new MySqlContext(new MySqlConnectionFactory(DbHelper.MySqlConnStr, DatabaseNameConst.MySqlTest)))
            {
                return context.Insert<T_Users>(user);
            }
        }

        public int DelUser(T_Users user)
        {
            using (var context = new MySqlContext(new MySqlConnectionFactory(DbHelper.MySqlConnStr, DatabaseNameConst.MySqlTest)))
            {
                return context.Delete<T_Users>(user);
            }
        }

        public int DelUser(int id)
        {
            using (var context = new MySqlContext(new MySqlConnectionFactory(DbHelper.MySqlConnStr, DatabaseNameConst.MySqlTest)))
            {
                return context.Delete<T_Users>(e => e.Id == id);
            }
        }

        public T_Users GetUserById(int id)
        {
            using (var context = new MySqlContext(new MySqlConnectionFactory(DbHelper.MySqlConnStr, DatabaseNameConst.MySqlTest)))
            {
                return context.Query<T_Users>().Where(e => e.Id == id).FirstOrDefault();
            }
        }

        public T_Users GetUserByPwd(string loginName, string pwd)
        {
            using (var context = new MySqlContext(new MySqlConnectionFactory(DbHelper.MySqlConnStr, DatabaseNameConst.MySqlTest)))
            {
                var entity = context.Query<T_Users>();
                return entity.Where(e => e.LoginName == loginName && e.LoginPwd == pwd).FirstOrDefault();
            }
        }

        public List<T_Users> GetUserList()
        {
            using (var context = new MySqlContext(new MySqlConnectionFactory(DbHelper.MySqlConnStr, DatabaseNameConst.MySqlTest)))
            {
                var entity = context.Query<T_Users>();
                return entity.Where(e => e.Id > 0).ToList();
            }
        }

        public int UpdateUser(T_Users user)
        {
            using (var context = new MySqlContext(new MySqlConnectionFactory(DbHelper.MySqlConnStr, DatabaseNameConst.MySqlTest)))
            {
                return context.Update<T_Users>(user);   
            }
        }
    }
}
