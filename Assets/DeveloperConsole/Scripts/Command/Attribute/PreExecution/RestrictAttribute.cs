using System;
using System.Threading;
using System.Threading.Tasks;

namespace DeveloperConsole.Command
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RestrictAttribute : PreExecutionValidatorAttribute
    {
        public UnityEnvironment Environment;
        public RestrictAttribute(UnityEnvironment environment)
        {
            Environment = environment;
        }

        public override Task<bool> Validate(CommandContext context, CancellationToken cancellationToken)
        {
            return Task.FromResult(Environment.IsAvailable());
        }

        public override string OnValidationFailedMessage()
        {
            return $"Cannot run this command because it is only available in {Environment}";
        }
    }
}
