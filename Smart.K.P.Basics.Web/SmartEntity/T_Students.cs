using Chloe.Entity;
using Chloe.Oracle;
using System;

namespace SmartEntity
{
    [Serializable]
    [TableAttribute("Students")]
    public partial class T_Students
    {        
        /// <summary>
        /// Id
        /// </summary>
        [Column(IsPrimaryKey = true)]
        [AutoIncrement]
        [Sequence("USERS_AUTOID")]//For oracle
        public int Id { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }
 
    }
}
