using System;
using System.Threading.Tasks;

namespace DeveloperConsole
{
    [AttributeUsage(AttributeTargets.Class)]
    public abstract class PreExecutionValidatorAttribute : Attribute
    {
        public abstract Task<bool> Validate(CommandContext context);

        public virtual string OnValidationFailedMessage() => "";
        public virtual string OnValidationSucceededMessage() => "";
    }
}