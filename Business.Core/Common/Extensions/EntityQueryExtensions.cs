using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Business.Core.Common.Exceptions;

namespace Business.Core.Common.Extensions
{
    public static class EntityQueryExtensions
    {
        public static IQueryable<TEntity> Search<TEntity, TProperty>(this IQueryable<TEntity> query, TProperty term)
        {
            var param = Expression.Parameter(typeof(TEntity));

            var predicate = PredicateExtensions.Begin<TEntity>();

            var fieldType = typeof(string);
            const string containsMethod = "Contains";
            var containsMethodInfo = fieldType.GetMethod(containsMethod, new[] { fieldType });

            if (containsMethodInfo == null)
                throw new BusinessGenericException($"There's no method \"{containsMethod}\" in type {fieldType.Name}.");

            const string isNullOrEmptyMethod = "IsNullOrEmpty";

            foreach (var fieldName in GetEntityPropertiesToCompare<TEntity, TProperty>())
            {
                var property = Expression.Property(param, fieldName);

                var notNullCondition =
                Expression.Lambda<Func<TEntity, bool>>(
                    Expression.Not(
                        Expression.Call(fieldType, isNullOrEmptyMethod, null, property)
                    ),
                param);
                
                var predicateToAdd =
                    Expression.Lambda<Func<TEntity, bool>>(
                        Expression.Call(
                            property,
                            containsMethodInfo,
                            Expression.Constant(term)
                        ),
                        param);

                predicate = predicate.Or(notNullCondition.And(predicateToAdd));
            }

            return query.Where(predicate);
        }

        private static IEnumerable<string> GetEntityPropertiesToCompare<TEntity, TProperty>()
        {
            var propertyType = typeof(TProperty);

            var properties = typeof(TEntity).GetProperties()
                .Where(p => p.PropertyType == propertyType)
                .Select(p => p.Name);

            return properties;
        }
    }
}