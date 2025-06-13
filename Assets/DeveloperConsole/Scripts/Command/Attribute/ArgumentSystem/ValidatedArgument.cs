using System.Reflection;
using System.Threading.Tasks;

namespace DeveloperConsole.Command
{
    /// <summary>
    /// Base class for all argument attributes which must be validated.
    /// </summary>
    public abstract class ValidatedAsync : ArgumentAttribute
    {
        /// <summary>
        /// Creates a new asynchronously validated argument.
        /// </summary>
        /// <param name="description">The arg's description.</param>
        /// <param name="overrideName">Overrides the field name.</param>
        protected ValidatedAsync(string description, string overrideName = null) : base(description, overrideName) { }

        
        /// <summary>
        /// Validates an argument asynchronously.
        /// </summary>
        /// <param name="data">The input data.</param>
        /// <returns>True if validation passed.</returns>
        public abstract Task<bool> ValidateAsync(AttributeValidationData data);
        
        
        /// <summary>
        /// The message to get when validation fails.
        /// </summary>
        /// <returns>The error message.</returns>
        public abstract string ErrorMessage();
    }

    /// <summary>
    /// Base class for all syncrhonous argument attributes which must be validated.
    /// </summary>
    public abstract class Validated : ValidatedAsync
    {
        /// <summary>
        /// Creates a new synchronously validated argument.
        /// </summary>
        /// <param name="description">The arg's description.</param>
        /// <param name="overrideName">Overrides the field name.</param>
        protected Validated(string description, string overrideName = null) : base(description, overrideName) { }

        public override async Task<bool> ValidateAsync(AttributeValidationData data)
        {
            return await Task.FromResult(Validate(data));
        }

        
        /// <summary>
        /// Runs the synchronous validation.
        /// </summary>
        /// <param name="data">The input data.</param>
        /// <returns>True if validation passed.</returns>
        protected abstract bool Validate(AttributeValidationData data);
    }
    
    // TODO: Get rid of this bullshit that's impossible to extend
    /// <summary>
    /// The input data to all argument validators.
    /// </summary>
    public class AttributeValidationData
    {
        public FieldInfo FieldInfo;
        public object Object;
        public bool WasSet;
    }
}