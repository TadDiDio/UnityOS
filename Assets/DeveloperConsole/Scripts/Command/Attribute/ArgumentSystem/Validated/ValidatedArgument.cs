using DeveloperConsole.Parsing;

namespace DeveloperConsole.Command
{
    /// <summary>
    /// Synchronous argument attributes which must be validated.
    /// </summary>
    public interface IValidatedAttribute
    {
        /// <summary>
        /// Runs the validation for this attribute. Once this is called,
        /// it is safe to assume that arguments are well formed. Validators
        /// apply logic on top of casted values.
        /// </summary>
        /// <param name="argSpec">The arg that this validator is validating.</param>
        /// <returns>True if validation passed.</returns>
        public abstract bool Validate(ArgumentSpecification argSpec);


        /// <summary>
        /// Recording hook when the argument is set during parsing.
        /// </summary>
        /// <param name="context">The context of this recording.</param>
        public abstract void Record(RecordingContext context);


        /// <summary>
        /// Gets an error message when this validator fails.
        /// </summary>
        /// <returns>The message.</returns>
        public abstract string ErrorMessage();
    }

    public class RecordingContext
    {
        public object ArgumentValue;
        public ArgumentSpecification ArgumentSpecification;
        public ICommandParseTarget CommandParseTarget;
    }
}
