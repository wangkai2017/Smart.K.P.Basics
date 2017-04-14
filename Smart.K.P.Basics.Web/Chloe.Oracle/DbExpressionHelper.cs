using Chloe.DbExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chloe.Oracle
{
    static class DbExpressionHelper
    {
        public static DbExpression StripConvert(DbExpression exp)
        {
            while (exp.NodeType == DbExpressionType.Convert)
            {
                exp = ((DbConvertExpression)exp).Operand;
            }

            return exp;
        }
        public static DbExpression StripInvalidConvert(DbExpression exp)
        {
            DbConvertExpression convertExpression = exp as DbConvertExpression;

            if (convertExpression == null)
                return exp;

            if (convertExpression.Type.IsEnum)
            {
                //(enumType)123
                if (typeof(int) == convertExpression.Operand.Type)
                    return StripInvalidConvert(convertExpression.Operand);

                DbConvertExpression newExp = new DbConvertExpression(typeof(int), convertExpression.Operand);
                return StripInvalidConvert(newExp);
            }

            Type unType;

            //(int?)123
            if (Utils.IsNullable(convertExpression.Type, out unType))//可空类型转换
            {
                if (unType == convertExpression.Operand.Type)
                    return StripInvalidConvert(convertExpression.Operand);

                DbConvertExpression newExp = new DbConvertExpression(unType, convertExpression.Operand);
                return StripInvalidConvert(newExp);
            }

            //(int)enumTypeValue
            if (exp.Type == typeof(int))
            {
                //(int)enumTypeValue
                if (convertExpression.Operand.Type.IsEnum)
                    return StripInvalidConvert(convertExpression.Operand);

                //(int)NullableEnumTypeValue
                if (Utils.IsNullable(convertExpression.Operand.Type, out unType) && unType.IsEnum)
                    return StripInvalidConvert(convertExpression.Operand);
            }

            //float long double and so on
            if (exp.Type.IsValueType)
            {
                //(long)NullableValue
                if (Utils.IsNullable(convertExpression.Operand.Type, out unType) && unType == exp.Type)
                    return StripInvalidConvert(convertExpression.Operand);
            }

            if (convertExpression.Type == convertExpression.Operand.Type)
            {
                return StripInvalidConvert(convertExpression.Operand);
            }

            //如果是子类向父类转换
            if (exp.Type.IsAssignableFrom(convertExpression.Operand.Type))
                return StripInvalidConvert(convertExpression.Operand);

            return convertExpression;
        }
    }
}
