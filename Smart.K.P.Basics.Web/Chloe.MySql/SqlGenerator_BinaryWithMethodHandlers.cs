﻿using Chloe.DbExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Chloe.MySql
{
    partial class SqlGenerator : DbExpressionVisitor<DbExpression>
    {
        static Dictionary<MethodInfo, Action<DbBinaryExpression, SqlGenerator>> InitBinaryWithMethodHandlers()
        {
            var binaryWithMethodHandlers = new Dictionary<MethodInfo, Action<DbBinaryExpression, SqlGenerator>>();
            binaryWithMethodHandlers.Add(UtilConstants.MethodInfo_String_Concat_String_String, StringConcat);
            binaryWithMethodHandlers.Add(UtilConstants.MethodInfo_String_Concat_Object_Object, StringConcat);

            var ret = Utils.Clone(binaryWithMethodHandlers);
            return ret;
        }

        static void StringConcat(DbBinaryExpression exp, SqlGenerator generator)
        {
            List<DbExpression> operands = new List<DbExpression>();
            operands.Add(exp.Right);

            DbExpression left = exp.Left;
            DbAddExpression e = null;
            while ((e = (left as DbAddExpression)) != null && (e.Method == UtilConstants.MethodInfo_String_Concat_String_String || e.Method == UtilConstants.MethodInfo_String_Concat_Object_Object))
            {
                operands.Add(e.Right);
                left = e.Left;
            }

            operands.Add(left);

            DbExpression whenExp = null;
            List<DbExpression> operandExps = new List<DbExpression>(operands.Count);
            for (int i = operands.Count - 1; i >= 0; i--)
            {
                DbExpression operand = operands[i];
                DbExpression opBody = operand;
                if (opBody.Type != UtilConstants.TypeOfString)
                {
                    // 需要 cast type
                    opBody = DbExpression.Convert(opBody, UtilConstants.TypeOfString);
                }

                DbExpression equalNullExp = DbExpression.Equal(opBody, UtilConstants.DbConstant_Null_String);

                if (whenExp == null)
                    whenExp = equalNullExp;
                else
                    whenExp = DbExpression.And(whenExp, equalNullExp);

                DbExpression thenExp = DbConstantExpression.StringEmpty;
                DbCaseWhenExpression.WhenThenExpressionPair whenThenPair = new DbCaseWhenExpression.WhenThenExpressionPair(equalNullExp, thenExp);

                List<DbCaseWhenExpression.WhenThenExpressionPair> whenThenExps = new List<DbCaseWhenExpression.WhenThenExpressionPair>(1);
                whenThenExps.Add(whenThenPair);

                DbExpression elseExp = opBody;

                DbCaseWhenExpression caseWhenExpression = DbExpression.CaseWhen(whenThenExps, elseExp, UtilConstants.TypeOfString);

                operandExps.Add(caseWhenExpression);
            }

            generator._sqlBuilder.Append("CASE", " WHEN ");
            whenExp.Accept(generator);
            generator._sqlBuilder.Append(" THEN ");
            DbConstantExpression.Null.Accept(generator);
            generator._sqlBuilder.Append(" ELSE ");

            generator._sqlBuilder.Append("CONCAT(");

            for (int i = 0; i < operandExps.Count; i++)
            {
                if (i > 0)
                    generator._sqlBuilder.Append(",");

                operandExps[i].Accept(generator);
            }

            generator._sqlBuilder.Append(")");

            generator._sqlBuilder.Append(" END");
        }
    }
}
