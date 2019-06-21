using System.Data.Entity.Validation;
using System.Text;

namespace Business.Core.Messaging.Common.Helpers
{
    public static class DbEntityExtensions
    {
        public static string ToLogString(this DbEntityValidationException dbex)
        {
            var sb = new StringBuilder();
            sb.AppendLine("-----------DbEntityValidationException encountered----------");
            sb.AppendLine("Message : " + dbex.Message + ";   ");
            if (dbex.InnerException != null)
            {
                sb.AppendLine("Inner Exception : " + dbex.InnerException.Message + ";   ");
            }
            sb.AppendLine("StackTrace : " + dbex.StackTrace + ";   ");
            foreach (var ev in dbex.EntityValidationErrors)
            {
                sb.AppendLine("***********" + ev.Entry.Entity.GetType().Name + " Validation Errors :*****************");
                sb.AppendLine("Entity Name : " + ev.Entry.Entity.GetType().Name + ";   ");
                foreach (var ve in ev.ValidationErrors)
                {
                    sb.AppendLine("Property Name : " + ve.PropertyName + ";   ");
                    sb.AppendLine("Error : " + ve.ErrorMessage + ";   ");
                }
            }
            return sb.ToString();
        }
    }
}
