using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace Chloe.Descriptors
{
    public abstract class MemberDescriptor
    {
        Dictionary<Type, Attribute> _customAttributes = new Dictionary<Type, Attribute>();
        protected MemberDescriptor(TypeDescriptor declaringEntityDescriptor)
        {
            this.DeclaringEntityDescriptor = declaringEntityDescriptor;
        }

        public TypeDescriptor DeclaringEntityDescriptor { get; set; }
        public abstract MemberInfo MemberInfo { get; }
        public abstract Type MemberInfoType { get; }
        public abstract MemberTypes MemberType { get; }

        public virtual Attribute GetCustomAttribute(Type attributeType)
        {
            Attribute val;
            if (!this._customAttributes.TryGetValue(attributeType, out val))
            {
                val = this.MemberInfo.GetCustomAttributes(attributeType, false).FirstOrDefault() as Attribute;
                lock (this._customAttributes)
                {
                    this._customAttributes[attributeType] = val;
                }
            }

            return val;
        }
        public bool IsDefined(Type attributeType)
        {
            return this.MemberInfo.IsDefined(attributeType, false);
        }
    }
}
