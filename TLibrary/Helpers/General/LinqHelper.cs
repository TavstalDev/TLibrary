using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Tavstal.TLibrary.Helpers.General
{
    /// <summary>
    /// Provides helper methods for working with LINQ expressions.
    /// </summary>
    public static class LinqHelper
    {
        /// <summary>
        /// Pulls out all single expressions from a binary expression tree.
        /// If a part of the expression is also a binary expression, this method calls itself to get the nested ones.
        /// </summary>
        /// <param name="binaryExpression">The binary expression to unpack.</param>
        /// <returns>A list of all expressions found inside the binary expression.</returns>
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

        /// <summary>
        /// Gets the name of a member from a member expression.
        /// </summary>
        /// <param name="expression">The expression to get the member name from. Must be a <see cref="MemberExpression"/>.</param>
        /// <returns>The name of the member.</returns>
        /// <exception cref="ArgumentException">Thrown when the expression is not a <see cref="MemberExpression"/>.</exception>
        public static string GetMemberName(Expression expression)
        {
            if (expression is MemberExpression memberExpression)
                return memberExpression.Member.Name;

            throw new ArgumentException("Invalid expression type. Expected MemberExpression.");
        }
    }
}
