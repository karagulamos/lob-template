using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;

namespace Business.Persistence.Repository.Interceptors
{
    public class SoftDeleteInterceptor : IDbCommandTreeInterceptor
    {
        public const string IsDeletedColumnName = "IsDeleted";

        public void TreeCreated(DbCommandTreeInterceptionContext interceptionContext)
        {
            var dataSpace = interceptionContext.OriginalResult.DataSpace;
            if (dataSpace != DataSpace.SSpace)
            {
                return;
            }

            if (interceptionContext.Result is DbQueryCommandTree queryCommand)
            {
                interceptionContext.Result = HandleQueryCommand(queryCommand);
            }
        }

        private static DbCommandTree HandleQueryCommand(DbQueryCommandTree queryCommand)
        {
            var newQuery = queryCommand.Query.Accept(new SoftDeleteQueryVisitor());

            return new DbQueryCommandTree(
                queryCommand.MetadataWorkspace,
                queryCommand.DataSpace,
                newQuery
            );
        }

        public class SoftDeleteQueryVisitor : DefaultExpressionVisitor
        {
            public override DbExpression Visit(DbScanExpression expression)
            {
                var table = (EntityType)expression.Target.ElementType;

                if (table.Properties.All(p => p.Name != IsDeletedColumnName))
                {
                    return base.Visit(expression);
                }

                var binding = expression.Bind();

                return binding.Filter(
                    binding.VariableType
                           .Variable(binding.VariableName)
                           .Property(IsDeletedColumnName)
                           .NotEqual(DbExpression.FromBoolean(true))
                );
            }
        }
    }
}
