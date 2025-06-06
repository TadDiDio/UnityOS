using System;
using System.Reflection;
using System.Threading.Tasks;

namespace DeveloperConsole
{
    public abstract class ValidatedArgumentAsync : Attribute
    {
        public abstract Task<bool> ValidateAsync(AttributeValidationData data);
        public abstract string ErrorMessage();
    }

    public abstract class ValidatedArgument : ValidatedArgumentAsync
    {
        public override async Task<bool> ValidateAsync(AttributeValidationData data)
        {
            return await Task.FromResult(Validate(data));
        }
        
        public abstract bool Validate(AttributeValidationData data);
    }
    public class AttributeValidationData
    {
        public FieldInfo FieldInfo;
        public object Object;
        public bool WasSet;
    }
}