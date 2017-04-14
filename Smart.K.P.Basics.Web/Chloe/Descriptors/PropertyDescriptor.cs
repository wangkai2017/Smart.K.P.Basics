﻿using Chloe.DbExpressions;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Threading;
using Chloe.Core;
using Chloe.Core.Emit;

namespace Chloe.Descriptors
{
    public class PropertyDescriptor : MappingMemberDescriptor
    {
        PropertyInfo _propertyInfo;
        DbColumn _column;

        Func<object, object> _valueGetter = null;
        Action<object, object> _valueSetter = null;
        public PropertyDescriptor(PropertyInfo propertyInfo, TypeDescriptor declaringEntityDescriptor, string columnName)
            : base(declaringEntityDescriptor)
        {
            this._propertyInfo = propertyInfo;
            this._column = new DbColumn(columnName, propertyInfo.PropertyType);
        }

        public override MemberInfo MemberInfo
        {
            get { return this._propertyInfo; }
        }
        public override Type MemberInfoType
        {
            get { return this._propertyInfo.PropertyType; }
        }
        public override MemberTypes MemberType
        {
            get { return MemberTypes.Property; }
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
                            this._valueGetter = DelegateGenerator.CreateValueGetter(this._propertyInfo);
                        }
                    }
                    finally
                    {
                        Monitor.Exit(this);
                    }
                }
                else
                {
                    return this._propertyInfo.GetValue(instance, null);
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
                            this._valueSetter = DelegateGenerator.CreateValueSetter(this._propertyInfo);
                        }
                    }
                    finally
                    {
                        Monitor.Exit(this);
                    }
                }
                else
                {
                    this._propertyInfo.SetValue(instance, value, null);
                    return;
                }
            }

            this._valueSetter(instance, value);
        }
    }

}
