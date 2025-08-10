using DeveloperConsole.Parsing;

namespace DeveloperConsole.Command
{
	public interface ICommandValidator
	{
        /// <summary>
        /// Hook to execute arbitrary validation logic in a commad after parsing.
        /// </summary>
        /// <param name="target">The target being validated.</param>
        /// <param name="errorMessage">An error message if there is one.</param>
        /// <returns>True if successful validation.</returns>
		public bool Validate(CommandParseTarget target, out string errorMessage);
	}
}
