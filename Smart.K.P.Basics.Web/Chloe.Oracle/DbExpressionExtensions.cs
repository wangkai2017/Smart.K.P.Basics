using Chloe.DbExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Chloe.Oracle
{
    static class DbExpressionExtensions
    {
        /// <summary>
        /// 尝试将 exp 转换成 DbParameterExpression。
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static bool TryParseToParameterExpression(this DbMemberExpression exp, out DbParameterExpression val)
        {
            val = null;
            if (!exp.CanEvaluate())
                return false;

            //求值
            val = exp.ParseToParameterExpression();
            return true;
        }
        /// <summary>
        /// 尝试将 exp 转换成 DbParameterExpression。
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static DbExpression ParseDbExpression(this DbExpression exp)
        {
            DbExpression stripedExp = DbExpressionHelper.StripInvalidConvert(exp);

            DbExpression tempExp = stripedExp;

            List<DbConvertExpression> cList = null;
            while (tempExp.NodeType == DbExpressionType.Convert)
            {
                if (cList == null)
                    cList = new List<DbConvertExpression>();

                DbConvertExpression c = (DbConvertExpression)tempExp;
                cList.Add(c);
                tempExp = c.Operand;
            }

            if (tempExp.NodeType == DbExpressionType.Constant || tempExp.NodeType == DbExpressionType.Parameter)
                return stripedExp;

            if (tempExp.NodeType == DbExpressionType.MemberAccess)
            {
                DbMemberExpression dbMemberExp = (DbMemberExpression)tempExp;

                if (ExistDateTime_NowOrDateTime_UtcNow(dbMemberExp))
                    return stripedExp;

                DbParameterExpression val;
                if (DbExpressionExtensions.TryParseToParameterExpression(dbMemberExp, out val))
                {
                    if (cList != null)
                    {
                        if (val.Value == DBNull.Value)//如果是 null，则不需要 Convert 了，在数据库里没意义
                            return val;

                        DbConvertExpression c = null;
                        for (int i = cList.Count - 1; i > -1; i--)
                        {
                            DbConvertExpression item = cList[i];
                            c = new DbConvertExpression(item.Type, val);
                        }

                        return c;
                    }

                    return val;
                }
            }

            return stripedExp;
        }

        public static bool CanEvaluate(this DbMemberExpression memberExpression)
        {
            if (memberExpression == null)
                throw new ArgumentNullException("memberExpression");

            do
            {
                DbExpression prevExp = memberExpression.Expression;

                // prevExp == null 表示是静态成员
                if (prevExp == null || prevExp is DbConstantExpression)
                    return true;

                DbMemberExpression memberExp = prevExp as DbMemberExpression;
                if (memberExp == null)
                    return false;
                else
                    memberExpression = memberExp;

            } while (true);
        }

        /// <summary>
        /// 对 memberExpression 进行求值
        /// </summary>
        /// <param name="exp"></param>
        /// <returns>返回 DbParameterExpression</returns>
        public static DbParameterExpression ParseToParameterExpression(this DbMemberExpression memberExpression)
        {
            DbParameterExpression ret = null;
            //求值
            object val = DbExpressionExtensions.GetExpressionValue(memberExpression);

            ret = DbExpression.Parameter(val, memberExpression.Type);

            return ret;
        }

        /// <summary>
        /// 判定 exp 返回值肯定是 null
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static bool AffirmExpressionRetValueIsNull(this DbExpression exp)
        {
            exp = DbExpressionHelper.StripConvert(exp);

            if (exp.NodeType == DbExpressionType.Constant)
            {
                var c = (DbConstantExpression)exp;
                return c.Value == null || c.Value == DBNull.Value;
            }


            if (exp.NodeType == DbExpressionType.Parameter)
            {
                var p = (DbParameterExpression)exp;
                return p.Value == null || p.Value == DBNull.Value;
            }

            return false;
        }
        /// <summary>
        /// 判定 exp 返回值肯定不是 null
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static bool AffirmExpressionRetValueIsNotNull(this DbExpression exp)
        {
            exp = DbExpressionHelper.StripConvert(exp);

            if (exp.NodeType == DbExpressionType.Constant)
            {
                var c = (DbConstantExpression)exp;
                return c.Value != null && c.Value != DBNull.Value;
            }

            if (exp.NodeType == DbExpressionType.Parameter)
            {
                var p = (DbParameterExpression)exp;
                return p.Value != null && p.Value != DBNull.Value;
            }

            return false;
        }

        public static object GetMemberAccessExpressionValue(this DbMemberExpression exp, object instance)
        {
            if (exp.Member.MemberType
              == MemberTypes.Field)
            {
                return ((FieldInfo)exp.Member).GetValue(instance);
            }
            else if (exp.Member.MemberType
                     == MemberTypes.Property)
            {
                return ((PropertyInfo)exp.Member).GetValue(instance, null);
            }

            throw new NotSupportedException();
        }
        public static object GetExpressionValue(this DbExpression exp)
        {
            if (exp.NodeType == DbExpressionType.Constant)
                return ((DbConstantExpression)exp).Value;

            if (exp.NodeType == DbExpressionType.MemberAccess)
            {
                DbMemberExpression m = (DbMemberExpression)exp;
                object instance = null;
                if (m.Expression != null)
                {
                    instance = DbExpressionExtensions.GetExpressionValue(m.Expression);

                    /* 非静态成员，需要检查是否为空引用。Nullable<T>.HasValue 的情况比较特俗，暂不考虑 */
                    Type declaringType = m.Member.DeclaringType;
                    if (declaringType.IsClass || declaringType.IsInterface)
                    {
                        if (instance == null)
                            throw new NullReferenceException(string.Format("There is an object reference not set to an instance of an object in expression tree.The type of null object is '{0}'.", declaringType.FullName));
                    }
                }

                return GetMemberAccessExpressionValue(m, instance);
            }

            throw new NotSupportedException();
        }
        public static bool ExistDateTime_NowOrDateTime_UtcNow(this DbMemberExpression exp)
        {
            while (exp != null)
            {
                if (exp.Member == UtilConstants.PropertyInfo_DateTime_Now || exp.Member == UtilConstants.PropertyInfo_DateTime_UtcNow)
                {
                    return true;
                }

                exp = exp.Expression as DbMemberExpression;
            }

            return false;
        }
    }
}
