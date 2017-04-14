﻿using Chloe.Core;
using Chloe.DbExpressions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Chloe.Oracle
{
    partial class SqlGenerator : DbExpressionVisitor<DbExpression>
    {
        void LeftBracket()
        {
            this._sqlBuilder.Append("(");
        }
        void RightBracket()
        {
            this._sqlBuilder.Append(")");
        }

        static string GenParameterName(int ordinal)
        {
            if (ordinal < CacheParameterNames.Count)
            {
                return CacheParameterNames[ordinal];
            }

            return ParameterPrefix + ordinal.ToString();
        }
        static string GenRowNumberName(List<DbColumnSegment> columns)
        {
            int ROW_NUMBER_INDEX = 1;
            string row_numberName = "ROW_NUMBER_0";
            while (columns.Any(a => string.Equals(a.Alias, row_numberName, StringComparison.OrdinalIgnoreCase)))
            {
                row_numberName = "ROW_NUMBER_" + ROW_NUMBER_INDEX.ToString();
                ROW_NUMBER_INDEX++;
            }

            return row_numberName;
        }

        static DbExpression EnsureDbExpressionReturnCSharpBoolean(DbExpression exp)
        {
            if (exp.Type != UtilConstants.TypeOfBoolean && exp.Type != UtilConstants.TypeOfBoolean_Nullable)
                return exp;

            if (SafeDbExpressionTypes.Contains(exp.NodeType))
            {
                return exp;
            }

            //将且认为不符合上述条件的都是诸如 a.Id>1,a.Name=="name" 等不能作为 bool 返回值的表达式
            //构建 case when 
            return ConstructReturnCSharpBooleanCaseWhenExpression(exp);
        }
        public static DbCaseWhenExpression ConstructReturnCSharpBooleanCaseWhenExpression(DbExpression exp)
        {
            // case when 1>0 then 1 when not (1>0) then 0 else Null end
            DbCaseWhenExpression.WhenThenExpressionPair whenThenPair = new DbCaseWhenExpression.WhenThenExpressionPair(exp, DbConstantExpression.True);
            DbCaseWhenExpression.WhenThenExpressionPair whenThenPair1 = new DbCaseWhenExpression.WhenThenExpressionPair(DbExpression.Not(exp), DbConstantExpression.False);
            List<DbCaseWhenExpression.WhenThenExpressionPair> whenThenExps = new List<DbCaseWhenExpression.WhenThenExpressionPair>(2);
            whenThenExps.Add(whenThenPair);
            whenThenExps.Add(whenThenPair1);
            DbCaseWhenExpression caseWhenExpression = DbExpression.CaseWhen(whenThenExps, DbConstantExpression.Null, UtilConstants.TypeOfBoolean);

            return caseWhenExpression;
        }

        static Stack<DbExpression> GatherBinaryExpressionOperand(DbBinaryExpression exp)
        {
            DbExpressionType nodeType = exp.NodeType;

            Stack<DbExpression> items = new Stack<DbExpression>();
            items.Push(exp.Right);

            DbExpression left = exp.Left;
            while (left.NodeType == nodeType)
            {
                exp = (DbBinaryExpression)left;
                items.Push(exp.Right);
                left = exp.Left;
            }

            items.Push(left);
            return items;
        }
        static void EnsureMethodDeclaringType(DbMethodCallExpression exp, Type ensureType)
        {
            if (exp.Method.DeclaringType != ensureType)
                throw UtilExceptions.NotSupportedMethod(exp.Method);
        }
        static void EnsureMethod(DbMethodCallExpression exp, MethodInfo methodInfo)
        {
            if (exp.Method != methodInfo)
                throw UtilExceptions.NotSupportedMethod(exp.Method);
        }


        static void EnsureTrimCharArgumentIsSpaces(DbExpression exp)
        {
            var m = exp as DbMemberExpression;
            if (m == null)
                throw new NotSupportedException();

            DbParameterExpression p;
            if (!DbExpressionExtensions.TryParseToParameterExpression(m, out p))
            {
                throw new NotSupportedException();
            }

            var arg = p.Value;

            if (arg == null)
                throw new NotSupportedException();

            var chars = arg as char[];
            if (chars.Length != 1 || chars[0] != ' ')
            {
                throw new NotSupportedException();
            }
        }
        static bool TryGetCastTargetDbTypeString(Type sourceType, Type targetType, out string dbTypeString, bool throwNotSupportedException = true)
        {
            dbTypeString = null;

            sourceType = Utils.GetUnderlyingType(sourceType);
            targetType = Utils.GetUnderlyingType(targetType);

            if (sourceType == targetType)
                return false;

            if (CastTypeMap.TryGetValue(targetType, out dbTypeString))
            {
                return true;
            }

            if (throwNotSupportedException)
                throw new NotSupportedException(AppendNotSupportedCastErrorMsg(sourceType, targetType));
            else
                return false;
        }
        static string AppendNotSupportedCastErrorMsg(Type sourceType, Type targetType)
        {
            return string.Format("Does not support the type '{0}' converted to type '{1}'.", sourceType.FullName, targetType.FullName);
        }

        static void DbFunction_DATEADD(SqlGenerator generator, string interval, DbMethodCallExpression exp)
        {
            /*
             * Just support hour/minute/second
             * systimestamp + numtodsinterval(1,'HOUR')
             * sysdate + numtodsinterval(50,'MINUTE')
             * sysdate + numtodsinterval(45,'SECOND')
             */
            generator._sqlBuilder.Append("(");
            exp.Object.Accept(generator);
            generator._sqlBuilder.Append(" + ");
            generator._sqlBuilder.Append("NUMTODSINTERVAL(");
            exp.Arguments[0].Accept(generator);
            generator._sqlBuilder.Append(",'");
            generator._sqlBuilder.Append(interval);
            generator._sqlBuilder.Append("')");
            generator._sqlBuilder.Append(")");
        }
        static void DbFunction_DATEPART(SqlGenerator generator, string interval, DbExpression exp, bool castToTimestamp = false)
        {
            /* cast(to_char(sysdate,'yyyy') as number) */
            generator._sqlBuilder.Append("CAST(TO_CHAR(");
            if (castToTimestamp)
            {
                generator.BuildCastState(exp, "TIMESTAMP");
            }
            else
                exp.Accept(generator);
            generator._sqlBuilder.Append(",'");
            generator._sqlBuilder.Append(interval);
            generator._sqlBuilder.Append("') AS NUMBER)");
        }
        static void DbFunction_DATEDIFF(SqlGenerator generator, string interval, DbExpression startDateTimeExp, DbExpression endDateTimeExp)
        {
            throw new NotSupportedException("DATEDIFF is not supported.");
        }

        #region AggregateFunction
        static void Aggregate_Count(SqlGenerator generator)
        {
            generator._sqlBuilder.Append("COUNT(1)");
        }
        static void Aggregate_LongCount(SqlGenerator generator)
        {
            generator._sqlBuilder.Append("COUNT(1)");
        }
        static void Aggregate_Max(SqlGenerator generator, DbExpression exp, Type retType)
        {
            AppendAggregateFunction(generator, exp, retType, "MAX", false);
        }
        static void Aggregate_Min(SqlGenerator generator, DbExpression exp, Type retType)
        {
            AppendAggregateFunction(generator, exp, retType, "MIN", false);
        }
        static void Aggregate_Sum(SqlGenerator generator, DbExpression exp, Type retType)
        {
            AppendAggregateFunction(generator, exp, retType, "SUM", false);
        }
        static void Aggregate_Average(SqlGenerator generator, DbExpression exp, Type retType)
        {
            AppendAggregateFunction(generator, exp, retType, "AVG", false);
        }

        static void AppendAggregateFunction(SqlGenerator generator, DbExpression exp, Type retType, string functionName, bool withCast)
        {
            string dbTypeString = null;
            if (withCast == true)
            {
                Type unType = Utils.GetUnderlyingType(retType);
                if (CastTypeMap.TryGetValue(unType, out dbTypeString))
                {
                    generator._sqlBuilder.Append("CAST(");
                }
            }

            generator._sqlBuilder.Append(functionName, "(");
            exp.Accept(generator);
            generator._sqlBuilder.Append(")");

            if (dbTypeString != null)
            {
                generator._sqlBuilder.Append(" AS ", dbTypeString, ")");
            }
        }
        #endregion


        static void CopyColumnSegments(List<DbColumnSegment> sourceList, List<DbColumnSegment> destinationList, DbTable newTable)
        {
            for (int i = 0; i < sourceList.Count; i++)
            {
                DbColumnSegment newColumnSeg = CloneColumnSegment(sourceList[i], newTable);
                destinationList.Add(newColumnSeg);
            }
        }
        static DbColumnSegment CloneColumnSegment(DbColumnSegment rawColumnSeg, DbTable newBelongTable)
        {
            DbColumnAccessExpression columnAccessExp = new DbColumnAccessExpression(rawColumnSeg.Body.Type, newBelongTable, rawColumnSeg.Alias);
            DbColumnSegment newColumnSeg = new DbColumnSegment(columnAccessExp, rawColumnSeg.Alias);

            return newColumnSeg;
        }
        static void AppendLimitCondition(DbSqlQueryExpression sqlQuery, int limitCount)
        {
            DbLessThanExpression lessThanExp = DbExpression.LessThan(OracleSemantics.DbMemberExpression_ROWNUM, DbExpression.Constant(limitCount + 1));

            DbExpression condition = lessThanExp;
            if (sqlQuery.Condition != null)
                condition = DbExpression.And(sqlQuery.Condition, condition);

            sqlQuery.Condition = condition;
        }
        static DbSqlQueryExpression WrapSqlQuery(DbSqlQueryExpression sqlQuery, DbTable table, List<DbColumnSegment> columnSegments = null)
        {
            DbSubQueryExpression subQuery = new DbSubQueryExpression(sqlQuery);

            DbSqlQueryExpression newSqlQuery = new DbSqlQueryExpression();

            DbTableSegment tableSeg = new DbTableSegment(subQuery, table.Name);
            DbFromTableExpression fromTableExp = new DbFromTableExpression(tableSeg);

            newSqlQuery.Table = fromTableExp;

            CopyColumnSegments(columnSegments ?? subQuery.SqlQuery.ColumnSegments, newSqlQuery.ColumnSegments, table);

            return newSqlQuery;
        }
        static DbSqlQueryExpression CloneWithoutLimitInfo(DbSqlQueryExpression sqlQuery, string wraperTableName = "T")
        {
            DbSqlQueryExpression newSqlQuery = new DbSqlQueryExpression();
            newSqlQuery.Table = sqlQuery.Table;
            newSqlQuery.ColumnSegments.AddRange(sqlQuery.ColumnSegments);
            newSqlQuery.Condition = sqlQuery.Condition;

            newSqlQuery.GroupSegments.AddRange(sqlQuery.GroupSegments);
            newSqlQuery.HavingCondition = sqlQuery.HavingCondition;

            newSqlQuery.Orderings.AddRange(sqlQuery.Orderings);

            if (sqlQuery.Orderings.Count > 0 || sqlQuery.GroupSegments.Count > 0)
            {
                newSqlQuery = WrapSqlQuery(newSqlQuery, new DbTable(wraperTableName));
            }

            return newSqlQuery;
        }
    }
}
