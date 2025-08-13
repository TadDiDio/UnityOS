using System;

namespace DeveloperConsole.Command
{
    [AttributeUsage(AttributeTargets.Field)]
    public class RequiredAttribute : ArgumentAttribute, IAttributeValidatorFactory
    {
        public IAttributeValidator CreateValidatorInstance()
        {
            return new RequiredAttributeValidator();
        }
    }
}
