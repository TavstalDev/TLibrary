﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Tavstal.TLibrary.Compatibility;
using UnityEngine;

namespace Tavstal.TLibrary.Helpers
{
    public static class LinqHelper
    {
        public static List<Expression> GetExpressions(BinaryExpression binaryExpression)
        {
            var expressions = new List<Expression>();

            if (binaryExpression.Left is BinaryExpression nestedBinaryExpression)
                expressions.AddRange(GetExpressions(nestedBinaryExpression));
            else
                expressions.Add(binaryExpression.Left);

            if (binaryExpression.Right is BinaryExpression nestedBinaryExpression2)
                expressions.AddRange(GetExpressions(nestedBinaryExpression2));
            else
                expressions.Add(binaryExpression.Right);

            return expressions;
        }

        public static string GetMemberName(Expression expression)
        {
            if (expression is MemberExpression memberExpression)
                return memberExpression.Member.Name;

            throw new ArgumentException("Invalid expression type. Expected MemberExpression.");
        }
    }
}
