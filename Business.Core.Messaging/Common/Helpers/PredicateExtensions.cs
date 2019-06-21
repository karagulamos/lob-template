using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq.Expressions;

namespace Business.Core.Messaging.Common.Helpers
{
    internal static class PredicateExtensions
    {
        public static Expression<Func<T, bool>> Begin<T>(bool value = false)
        {
            if (value)
                return parameter => true; // value cannot be used in place of true/false

            return parameter => false;
        }

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> left,
            Expression<Func<T, bool>> right)
        {
            return CombineLambdas(left, right, ExpressionType.AndAlso);
        }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            return CombineLambdas(left, right, ExpressionType.OrElse);
        }

        public static PrimitivePropertyConfiguration IsUnique<TSource, TType>(this PrimitivePropertyConfiguration configuration, Expression<Func<TSource, TType>> field, int columnOrder = 0)
        {
            var propertyName = GetName(field);
            return configuration.HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute($"IX_{propertyName}", columnOrder) { IsUnique = true }));
        }

        public static string GetName<TSource, TField>(Expression<Func<TSource, TField>> field)
        {
            return (field.Body as MemberExpression ?? (MemberExpression)((UnaryExpression)field.Body).Operand).Member.Name;
        }


        #region private

        private static Expression<Func<T, bool>> CombineLambdas<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right, ExpressionType expressionType)
        {
            //Remove expressions created with Begin<T>()
            if (IsExpressionBodyConstant(left))
                return (right);

            var p = left.Parameters[0];

            var visitor = new SubstituteParameterVisitor
            {
                Sub =
                {
                    [right.Parameters[0]] = p
                }
            };

            Expression body = Expression.MakeBinary(expressionType, left.Body, visitor.Visit(right.Body));
            return Expression.Lambda<Func<T, bool>>(body, p);
        }

        private static bool IsExpressionBodyConstant<T>(Expression<Func<T, bool>> left)
        {
            return left.Body.NodeType == ExpressionType.Constant;
        }

        internal class SubstituteParameterVisitor : ExpressionVisitor
        {
            public Dictionary<Expression, Expression> Sub = new Dictionary<Expression, Expression>();

            protected override Expression VisitParameter(ParameterExpression node)
            {
                Expression newValue;

                return Sub.TryGetValue(node, out newValue) ? newValue : node;
            }
        }

        #endregion
    }
}
