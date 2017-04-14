using Chloe.Core;
using Chloe.DbExpressions;
using System;
using System.Reflection;

namespace Chloe.Descriptors
{
    public abstract class MappingMemberDescriptor : MemberDescriptor
    {
        protected MappingMemberDescriptor(TypeDescriptor declaringEntityDescriptor)
            : base(declaringEntityDescriptor)
        {
        }

        public bool IsPrimaryKey { get; set; }
        public bool IsAutoIncrement { get; set; }

        public abstract DbColumn Column { get; }
        public abstract object GetValue(object instance);
        public abstract void SetValue(object instance, object value);
    }
}
