using System;

namespace DeveloperConsole
{
    [AttributeUsage(AttributeTargets.Field)]
    public class RequiredArgAttribute : ValidatedAttribute
    {
        public override bool Validate(AttributeValidationData data)
        {
            return data.WasSet;
        }
    }
}