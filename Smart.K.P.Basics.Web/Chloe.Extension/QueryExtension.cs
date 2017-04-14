using Chloe.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Chloe
{
    public static class QueryExtension
    {
        public static IQuery<TSource> WhereIfNotNullOrEmpty<TSource>(this IQuery<TSource> source, string value, Expression<Func<TSource, bool>> predicate)
        {
            return source.WhereIf(!string.IsNullOrEmpty(value), predicate);
        }

        public static IQuery<TSource> WhereIfNotNull<TSource, TValue>(this IQuery<TSource> source, Nullable<TValue> value, Expression<Func<TSource, bool>> predicate) where TValue : struct
        {
            return source.WhereIf(value != null, predicate);
        }

        public static IQuery<TSource> WhereIfNotNull<TSource>(this IQuery<TSource> source, object value, Expression<Func<TSource, bool>> predicate)
        {
            return source.WhereIf(value != null, predicate);
        }

        public static IQuery<TSource> WhereIf<TSource>(this IQuery<TSource> source, bool condition, Expression<Func<TSource, bool>> predicate)
        {
            if (condition)
            {
                return source.Where(predicate);
            }
            return source;
        }

        public static IQuery<TSource> WhereIfNotNull<TSource, V>(this IQuery<TSource> source, V val, Expression<Func<TSource, V, bool>> predicate)
        {
            if (val != null)
            {
                Expression<Func<TSource, bool>> newPredicate = (Expression<Func<TSource, bool>>)ParameterTwoExpressionReplacer.Replace(predicate, val);
                source = source.Where(newPredicate);
            }

            return source;
        }
        public static IQuery<TSource> WhereIfNotNullOrEmpty<TSource>(this IQuery<TSource> source, string val, Expression<Func<TSource, string, bool>> predicate)
        {
            return source.WhereIfNotNull(val == string.Empty ? null : val, predicate);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="q"></param>
        /// <param name="orderString">Id asc,Age desc...</param>
        /// <returns></returns>
        public static IOrderedQuery<T> OrderBy<T>(this IQuery<T> q, string orderString)
        {
            if (q == null)
                throw new ArgumentNullException("q");
            if (string.IsNullOrEmpty(orderString))
                throw new ArgumentNullException("orderString");

            List<Ordering> orderingList = SplitOrderingString(orderString);

            IOrderedQuery<T> orderedQuery = null;
            for (int i = 0; i < orderingList.Count; i++)
            {
                Ordering ordering = orderingList[i];
                if (orderedQuery == null)
                    orderedQuery = q.InnerOrderBy(ordering);
                else
                    orderedQuery = orderedQuery.InnerThenBy(ordering);
            }

            return orderedQuery;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="q"></param>
        /// <param name="orderString">Id asc,Age desc...</param>
        /// <returns></returns>
        public static IOrderedQuery<T> ThenBy<T>(this IOrderedQuery<T> q, string orderString)
        {
            if (q == null)
                throw new ArgumentNullException("q");
            if (string.IsNullOrEmpty(orderString))
                throw new ArgumentNullException("orderString");

            List<Ordering> orderingList = SplitOrderingString(orderString);

            IOrderedQuery<T> orderedQuery = q;
            for (int i = 0; i < orderingList.Count; i++)
            {
                Ordering ordering = orderingList[i];
                orderedQuery = orderedQuery.InnerThenBy(ordering);
            }

            return orderedQuery;
        }

        static IOrderedQuery<T> InnerOrderBy<T>(this IQuery<T> q, Ordering ordering)
        {
            LambdaExpression predicate = GetOrderPredicate<T>(ordering.MemberChain);

            MethodInfo orderMethod;
            if (ordering.OrderType == OrderType.Asc)
                orderMethod = typeof(IQuery<T>).GetMethod("OrderBy");
            else
                orderMethod = typeof(IQuery<T>).GetMethod("OrderByDesc");

            IOrderedQuery<T> orderedQuery = Invoke<T>(q, orderMethod, predicate);
            return orderedQuery;
        }
        static IOrderedQuery<T> InnerThenBy<T>(this IOrderedQuery<T> q, Ordering ordering)
        {
            LambdaExpression predicate = GetOrderPredicate<T>(ordering.MemberChain);

            MethodInfo orderMethod;
            if (ordering.OrderType == OrderType.Asc)
                orderMethod = typeof(IOrderedQuery<T>).GetMethod("ThenBy");
            else
                orderMethod = typeof(IOrderedQuery<T>).GetMethod("ThenByDesc");

            IOrderedQuery<T> orderedQuery = Invoke<T>(q, orderMethod, predicate);
            return orderedQuery;
        }
        static IOrderedQuery<T> Invoke<T>(object q, MethodInfo orderMethod, LambdaExpression predicate)
        {
            orderMethod = orderMethod.MakeGenericMethod(new Type[] { predicate.Body.Type });
            IOrderedQuery<T> orderedQuery = (IOrderedQuery<T>)orderMethod.Invoke(q, new object[] { predicate });
            return orderedQuery;
        }
        static List<Ordering> SplitOrderingString(string orderString)
        {
            string[] orderings = SplitWithRemoveEmptyEntries(orderString, ',');
            List<Ordering> orderingList = new List<Ordering>(orderings.Length);

            for (int i = 0; i < orderings.Length; i++)
            {
                orderingList.Add(Ordering.Create(orderings[i]));
            }

            return orderingList;
        }
        static LambdaExpression GetOrderPredicate<T>(string memberChain)
        {
            Type entityType = typeof(T);

            string[] memberNames = SplitWithRemoveEmptyEntries(memberChain, '.');

            Type currType = entityType;
            ParameterExpression parameterExp = Expression.Parameter(entityType, "a");
            Expression exp = parameterExp;
            for (int i = 0; i < memberNames.Length; i++)
            {
                var memberName = memberNames[i];

                MemberInfo memberIfo = currType.GetProperty(memberName);
                if (memberIfo == null)
                {
                    memberIfo = currType.GetField(memberName);

                    if (memberIfo == null)
                    {
                        memberIfo = currType.GetProperties().Where(a => a.Name.ToLower() == memberName).FirstOrDefault();

                        if (memberIfo == null)
                        {
                            memberIfo = currType.GetFields().Where(a => a.Name.ToLower() == memberName).FirstOrDefault();
                        }
                    }
                }

                if (memberIfo == null)
                    throw new ArgumentException(string.Format("The type '{0}' doesn't define property or field '{1}'", currType.FullName, memberName));

                exp = Expression.MakeMemberAccess(exp, memberIfo);
                currType = exp.Type;
            }

            if (exp == parameterExp)
                throw new Exception("Oh,god!You are so lucky!");

            Type delegateType = null;

            delegateType = typeof(Func<,>).MakeGenericType(new Type[] { typeof(T), exp.Type });

            LambdaExpression lambda = Expression.Lambda(delegateType, exp, parameterExp);

            return lambda;
        }

        static string[] SplitWithRemoveEmptyEntries(string str, char c)
        {
            string[] arr = str.Split(new char[] { c }, StringSplitOptions.RemoveEmptyEntries);
            return arr;
        }

        class Ordering
        {
            public string MemberChain { get; set; }
            public OrderType OrderType { get; set; }

            public static Ordering Create(string str)
            {
                string[] arr = SplitWithRemoveEmptyEntries(str, ' ');

                Ordering ordering = new Ordering();

                if (arr.Length == 1)
                {
                    ordering.OrderType = OrderType.Asc;
                    ordering.MemberChain = arr[0];
                }
                else if (arr.Length == 2)
                {
                    string orderTypeString = arr[1].ToLower();
                    if (orderTypeString == "asc")
                        ordering.OrderType = OrderType.Asc;
                    else if (orderTypeString == "desc")
                        ordering.OrderType = OrderType.Desc;
                    else
                        throw new NotSupportedException(string.Format("Invalid order type '{0}'", orderTypeString));

                    ordering.MemberChain = arr[0];
                }
                else
                    throw new ArgumentException(string.Format("Invalid order text '{0}'", str));

                return ordering;
            }
        }
        enum OrderType
        {
            Asc,
            Desc
        }
    }
}
