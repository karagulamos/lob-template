using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq.Expressions;

namespace Business.Persistence.Repository.Common.Extensions
{
    internal static class EntityDataExtensions
    {
        public static PrimitivePropertyConfiguration IsUnique<TSource, TType>(this PrimitivePropertyConfiguration configuration, Expression<Func<TSource, TType>> field, int columnOrder = 0)
        {
            var propertyName = GetName(field);
            return configuration.HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute($"IX_{propertyName}", columnOrder) { IsUnique = true }));
        }

        public static string GetName<TSource, TField>(Expression<Func<TSource, TField>> field)
        {
            return (field.Body as MemberExpression ?? (MemberExpression)((UnaryExpression)field.Body).Operand).Member.Name;
        }


        public static DbModificationClause UpdateIfMatch(this DbModificationClause clause, string property, DbExpression value)
        {
            return clause.IsFor(property) ? DbExpressionBuilder.SetClause(clause.Property(), value) : clause;
        }

        public static bool IsFor(this DbModificationClause clause, string property)
        {
            return clause.HasPropertyExpression() && clause.Property().Property.Name == property;
        }

        public static DbPropertyExpression Property(this DbModificationClause clause)
        {
            if (clause.HasPropertyExpression())
            {
                var setClause = (DbSetClause)clause;
                return (DbPropertyExpression)setClause.Property;
            }

            var message =
                "clause does not contain property expression. " +
                "Use HasPropertyExpression method to check if it has property expression.";

            throw new Exception(message);
        }

        public static bool HasPropertyExpression(this DbModificationClause modificationClause)
        {
            var setClause = modificationClause as DbSetClause;
            return setClause?.Property is DbPropertyExpression;
        }
    }
}
