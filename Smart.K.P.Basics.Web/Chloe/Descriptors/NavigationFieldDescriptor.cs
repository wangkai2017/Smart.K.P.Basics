using System.Reflection;

namespace Chloe.Descriptors
{
    public class NavigationFieldDescriptor : NavigationMemberDescriptor
    {
        FieldInfo _fieldInfo;
        public NavigationFieldDescriptor(FieldInfo fieldInfo, TypeDescriptor declaringEntityDescriptor, string thisKey, string associatingKey)
            : base(fieldInfo, fieldInfo.FieldType, declaringEntityDescriptor, thisKey, associatingKey)
        {
            this._fieldInfo = fieldInfo;
        }
    }
}
