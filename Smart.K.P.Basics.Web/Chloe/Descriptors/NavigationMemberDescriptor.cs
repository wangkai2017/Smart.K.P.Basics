using System;
using System.Reflection;

namespace Chloe.Descriptors
{
    public class NavigationMemberDescriptor : MemberDescriptor
    {
        MemberInfo _memberInfo;

        /// <summary>
        /// 假设实体 Order 内有个导航属性 User， Order.UserId=User.Id，则 ThisKey 为 Order.UserId，AssociatingKey 为 User.Id
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <param name="memberType"></param>
        /// <param name="declaringEntityDescriptor"></param>
        /// <param name="thisKey">定义导航属性实体相关的属性或字段名称</param>
        /// <param name="associatingKey">关联导航属性实体类型的属性或字段名称</param>
        public NavigationMemberDescriptor(MemberInfo memberInfo, Type memberType, TypeDescriptor declaringEntityDescriptor, string thisKey, string associatingKey)
            : base(declaringEntityDescriptor)
        {
            this._memberInfo = memberInfo;
            this.ThisKey = thisKey;
            this.AssociatingKey = associatingKey;
        }

        public override MemberInfo MemberInfo
        {
            get { return this._memberInfo; }
        }
        public override Type MemberInfoType
        {
            get { throw new NotImplementedException(); }
        }
        public override MemberTypes MemberType
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// 假设实体 Order 内有个导航属性 User， Order.UserId=User.Id，则 ThisKey 为 Order.UserId，AssociatingKey 为 User.Id
        /// </summary>
        public string ThisKey { get; private set; }
        public string AssociatingKey { get; private set; }
    }
}
