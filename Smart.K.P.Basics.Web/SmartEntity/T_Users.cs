using Chloe.Entity;
using Chloe.Oracle;
using System;

namespace SmartEntity
{
    [Serializable]
    [TableAttribute("Users")]
    public class T_Users
    {
        /// <summary>
        /// Id
        /// </summary>
        [Column(IsPrimaryKey = true)]
        [AutoIncrement]
        [Sequence("USERS_AUTOID")]//For oracle
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string LoginName { get; set; }
        public string LoginPwd { get; set; }
    }
}
