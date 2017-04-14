using Chloe.DbExpressions;
using Chloe.Query;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Chloe.Utility
{
    static class Utils
    {
        static List<Type> MapTypes;
        public static readonly Dictionary<Type, DbType> _typeDbTypeMap;

        static Utils()
        {
            var mapTypes = new List<Type>();

            mapTypes.Add(typeof(string));
            mapTypes.Add(typeof(int));
            mapTypes.Add(typeof(long));
            mapTypes.Add(typeof(decimal));
            mapTypes.Add(typeof(double));
            mapTypes.Add(typeof(float));
            mapTypes.Add(typeof(bool));
            mapTypes.Add(typeof(DateTime));
            mapTypes.Add(typeof(short));
            mapTypes.Add(typeof(Guid));
            mapTypes.Add(typeof(byte));
            mapTypes.Add(typeof(char));

            mapTypes.Add(typeof(ulong));
            mapTypes.Add(typeof(uint));
            mapTypes.Add(typeof(ushort));
            mapTypes.Add(typeof(sbyte));

            mapTypes.Add(typeof(Object));

            mapTypes.Add(typeof(byte[]));

            mapTypes.TrimExcess();
            MapTypes = mapTypes;


            var typeDbTypeMap = new Dictionary<Type, DbType>();

            typeDbTypeMap[typeof(byte)] = DbType.Byte;
            typeDbTypeMap[typeof(sbyte)] = DbType.SByte;
            typeDbTypeMap[typeof(short)] = DbType.Int16;
            typeDbTypeMap[typeof(ushort)] = DbType.UInt16;
            typeDbTypeMap[typeof(int)] = DbType.Int32;
            typeDbTypeMap[typeof(uint)] = DbType.UInt32;
            typeDbTypeMap[typeof(long)] = DbType.Int64;
            typeDbTypeMap[typeof(ulong)] = DbType.UInt64;
            typeDbTypeMap[typeof(float)] = DbType.Single;
            typeDbTypeMap[typeof(double)] = DbType.Double;
            typeDbTypeMap[typeof(decimal)] = DbType.Decimal;
            typeDbTypeMap[typeof(bool)] = DbType.Boolean;
            typeDbTypeMap[typeof(string)] = DbType.String;
            typeDbTypeMap[typeof(char)] = DbType.StringFixedLength;
            typeDbTypeMap[typeof(Guid)] = DbType.Guid;
            typeDbTypeMap[typeof(DateTime)] = DbType.DateTime;
            typeDbTypeMap[typeof(DateTimeOffset)] = DbType.DateTimeOffset;
            typeDbTypeMap[typeof(TimeSpan)] = DbType.Time;
            typeDbTypeMap[typeof(byte[])] = DbType.Binary;
            typeDbTypeMap[typeof(Object)] = DbType.Object;

            _typeDbTypeMap = Utils.Clone(typeDbTypeMap);
        }

        public static DbType? TryGetDbType(Type type)
        {
            if (type == null)
                return null;

            Type unType;
            if (!Utils.IsNullable(type, out unType))
                unType = type;

            if (unType.IsEnum)
                unType = UtilConstants.TypeOfInt32;

            DbType ret;
            if (_typeDbTypeMap.TryGetValue(unType, out ret))
                return ret;

            return null;
        }
        public static void CheckNull(object obj, string paramName = null)
        {
            if (obj == null)
                throw new ArgumentNullException(paramName);
        }
        public static bool IsNullable(Type type)
        {
            Type unType;
            return IsNullable(type, out unType);
        }
        public static bool IsNullable(Type type, out Type unType)
        {
            unType = Nullable.GetUnderlyingType(type);
            return unType != null;
        }
        public static bool AreEqual(object obj1, object obj2)
        {
            if (obj1 == null && obj2 == null)
                return true;

            if (obj1 != null)
            {
                return obj1.Equals(obj2);
            }

            if (obj2 != null)
            {
                return obj2.Equals(obj1);
            }

            return object.Equals(obj1, obj2);
        }


        public static bool IsMapType(Type type)
        {
            Type unType;
            if (!Utils.IsNullable(type, out unType))
                unType = type;

            if (unType.IsEnum)
                return true;

            return MapTypes.Contains(unType);
        }
        public static bool IsAnonymousType(Type type)
        {
            string typeName = type.Name;
            return typeName.Contains("<>") && typeName.Contains("__") && typeName.Contains("AnonymousType");
        }

        public static string GenerateUniqueColumnAlias(DbSqlQueryExpression sqlQuery, string defaultAlias = UtilConstants.DefaultColumnAlias)
        {
            string alias = defaultAlias;
            int i = 0;
            while (sqlQuery.ColumnSegments.Any(a => string.Equals(a.Alias, alias, StringComparison.OrdinalIgnoreCase)))
            {
                alias = defaultAlias + i.ToString();
                i++;
            }

            return alias;
        }

        public static Dictionary<TKey, TValue> Clone<TKey, TValue>(Dictionary<TKey, TValue> source)
        {
            Dictionary<TKey, TValue> ret = new Dictionary<TKey, TValue>(source.Count);

            foreach (var kv in source)
            {
                ret.Add(kv.Key, kv.Value);
            }

            return ret;
        }
    }

}
