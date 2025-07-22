using System;
using System.Threading;
using System.Threading.Tasks;

namespace DeveloperConsole.Command
{
    [AttributeUsage(AttributeTargets.Class)]
    public abstract class PreExecutionValidatorAttribute : Attribute
    {
        /// <summary>
        /// Runs a validation check before the command executes.
        /// </summary>
        /// <param name="context">The command's execution context.</param>
        /// <param name="cancellationToken">A cancellation token to cancel execution.</param>
        /// <returns>True if the validation passed.</returns>
        public abstract Task<bool> Validate(CommandContext context, CancellationToken cancellationToken);


        /// <summary>
        /// The message to show if the validation fails.
        /// </summary>
        /// <returns>The error message.</returns>
        public abstract string OnValidationFailedMessage();
    }
}
