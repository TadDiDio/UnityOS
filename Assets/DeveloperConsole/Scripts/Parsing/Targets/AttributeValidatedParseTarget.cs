using System.Collections.Generic;
using System.Linq;
using DeveloperConsole.Command;

namespace DeveloperConsole.Parsing
{
    /// <summary>
    /// A parse target that validates ValidatedAttributes on arguments.
    /// </summary>
    public abstract class AttributeValidatedParseTarget : IParseTarget
    {
        public abstract HashSet<ArgumentSpecification> GetArguments();

        
        /// <summary>
        /// Sets the value of an argument on the target.
        /// </summary>
        /// <param name="argument">The arg to set.</param>
        /// <param name="argValue">The value.</param>
        protected abstract void SetArgumentValue(ArgumentSpecification argument, object argValue);
        
        
        public void SetArgument(ArgumentSpecification argument, object argValue)
        {
            SetArgumentValue(argument, argValue);
            foreach (var validator in argument.Attributes.OfType<IValidatedAttribute>())
            {
                validator.Record(new RecordingContext
                {
                    ArgumentSpecification = argument,
                    ArgumentValue = argValue,
                    ParseTarget = this
                });
            }
        }

        
        public bool Validate(out string errorMessage)
        {
            errorMessage = null;
            
            foreach (var arg in GetArguments())
            {
                var validated = arg.Attributes.OfType<IValidatedAttribute>();
                foreach (var attr in validated)
                {
                    if (attr.Validate(arg)) continue;
                    
                    errorMessage = attr.ErrorMessage();
                    return false;
                }
            }
            
            return true;
        }
    }
}