using System.Reflection;

namespace Chloe.Descriptors
{
    public class NavigationPropertyDescriptor : NavigationMemberDescriptor
    {
        PropertyInfo propertyInfo;
        public NavigationPropertyDescriptor(PropertyInfo propertyInfo, TypeDescriptor declaringEntityDescriptor, string thisKey, string associatingKey)
            : base(propertyInfo, propertyInfo.PropertyType, declaringEntityDescriptor, thisKey, associatingKey)
        {
            this.propertyInfo = propertyInfo;
        }
    }
}
