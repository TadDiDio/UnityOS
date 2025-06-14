using DeveloperConsole.Parsing;

namespace DeveloperConsole.Command
{
    /// <summary>
    /// Base class for all synchronous argument attributes which must be validated.
    /// </summary>
    public abstract class ValidatedAttribute : ArgumentAttribute
    {
        /// <summary>
        /// Creates a new synchronously validated argument.
        /// </summary>
        /// <param name="description">The arg's description.</param>
        /// <param name="overrideName">Overrides the field name.</param>
        protected ValidatedAttribute(string description, string overrideName = null) : base(description, overrideName) { }

        /// <summary>
        /// Runs the synchronous validation.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>True if validation passed.</returns>
        public abstract bool Validate(ParseContext context);

        public abstract string ErrorMessage();
    }
}