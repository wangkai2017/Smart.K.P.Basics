using System.Reflection;
using System;
using Chloe.DbExpressions;
using System.Threading;
using Chloe.Core;
using Chloe.Core.Emit;

namespace Chloe.Descriptors
{
    public class FieldDescriptor : MappingMemberDescriptor
    {
        FieldInfo _fieldInfo;
        DbColumn _column;

        Func<object, object> _valueGetter = null;
        Action<object, object> _valueSetter = null;
        public FieldDescriptor(FieldInfo fieldInfo, TypeDescriptor declaringEntityDescriptor, string columnName)
            : base(declaringEntityDescriptor)
        {
            this._fieldInfo = fieldInfo;
            this._column = new DbColumn(columnName, fieldInfo.FieldType);
        }


        public override MemberInfo MemberInfo
        {
            get { return this._fieldInfo; }
        }
        public override Type MemberInfoType
        {
            get { return this._fieldInfo.FieldType; }
        }
        public override MemberTypes MemberType
        {
            get { return MemberTypes.Field; }
        }
        public override DbColumn Column
        {
            get { return this._column; }
        }

        public override object GetValue(object instance)
        {
            if (null == this._valueGetter)
            {
                if (Monitor.TryEnter(this))
                {
                    try
                    {
                        if (null == this._valueGetter)
                        {
                            this._valueGetter = DelegateGenerator.CreateValueGetter(this._fieldInfo);
                        }
                    }
                    finally
                    {
                        Monitor.Exit(this);
                    }
                }
                else
                {
                    return this._fieldInfo.GetValue(instance);
                }
            }

            return this._valueGetter(instance);
        }
        public override void SetValue(object instance, object value)
        {
            if (null == this._valueSetter)
            {
                if (Monitor.TryEnter(this))
                {
                    try
                    {
                        if (null == this._valueSetter)
                        {
                            this._valueSetter = DelegateGenerator.CreateValueSetter(this._fieldInfo);
                        }
                    }
                    finally
                    {
                        Monitor.Exit(this);
                    }
                }
                else
                {
                    this._fieldInfo.SetValue(instance, value);
                    return;
                }
            }

            this._valueSetter(instance, value);
        }
    }
}
