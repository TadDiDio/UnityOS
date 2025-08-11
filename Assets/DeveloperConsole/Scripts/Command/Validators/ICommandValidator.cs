namespace DeveloperConsole.Command
{
	public interface ICommandValidator
	{
        /// <summary>
        /// Hook to execute arbitrary validation logic in a command after parsing.
        /// </summary>
        /// <param name="target">The target command being validated.</param>
        /// <param name="errorMessage">An error message if there is one.</param>
        /// <returns>True if successful validation.</returns>
		public bool Validate(ICommand target, out string errorMessage);
	}
}
