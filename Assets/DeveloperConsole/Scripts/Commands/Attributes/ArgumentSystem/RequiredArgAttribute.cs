using System;

namespace DeveloperConsole
{
    [AttributeUsage(AttributeTargets.Field)]
    public class RequiredArgAttribute : ArgumentValidator
    {
        public override bool Validate(AttributeValidationData data)
        {
            return data.WasSet;
        }

        public override string ErrorMessage() => "This argument must be explicitly included.";
    }
}