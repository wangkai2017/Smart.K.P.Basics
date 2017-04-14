using Chloe.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chloe.DbExpressions
{
    [System.Diagnostics.DebuggerDisplay("Name = {Name}")]
    public class DbColumn
    {
        string _name;
        Type _type;
        public DbColumn(string name, Type type)
        {
            this._name = name;
            this._type = type;
        }

        public string Name { get { return this._name; } }
        public Type Type { get { return this._type; } }
    }

}
