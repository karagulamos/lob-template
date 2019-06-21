using System.ComponentModel.DataAnnotations;

namespace Business.Core.Common.Extensions
{
    public class RequiredIfAttribute : RequiredAttribute
    {
        private string PropertyName { get; set; }
        private object DesiredValue { get; set; }

        public RequiredIfAttribute(string propertyName, object desiredvalue)
        {
            PropertyName = propertyName;
            DesiredValue = desiredvalue;
        }

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            var instance = context.ObjectInstance;
            var type = instance.GetType();
            var proprtyvalue = type.GetProperty(PropertyName)?.GetValue(instance, null);

            if (proprtyvalue?.ToString() != DesiredValue.ToString())
                return ValidationResult.Success;

            var result = base.IsValid(value, context);

            return result;
        }
    }
}
