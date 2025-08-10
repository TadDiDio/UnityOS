using System;
using System.Collections.Generic;
using System.Linq;
using DeveloperConsole.Command;

namespace DeveloperConsole.Parsing
{
    /// <summary>
    /// A target for parses to parse into.
    /// </summary>
    public class CommandParseTarget
    {
        /// <summary>
        /// The command being constructed.
        /// </summary>
        public ICommand Command { get; }

        /// <summary>
        /// The schema representing this command.
        /// </summary>
        public CommandSchema Schema { get; }

        private Dictionary<ArgumentSpecification, List<IAttributeValidator>> _validators = new();

        private List<ICommandValidator> _commandValidators;


        /// <summary>
        /// Creates a new command parse target.
        /// </summary>
        /// <param name="schema">The schema to build.</param>
        public CommandParseTarget(CommandSchema schema)
        {
            Schema = schema;
            Command = Activator.CreateInstance(schema.CommandType) as ICommand;

            // TODO: Move these registrations to a registry that ConsoleAPI can see.
            _commandValidators = new List<ICommandValidator>
            {
                new InstantiateVariadic(),
                new BindingProcessor()
            };

            foreach (var arg in schema.ArgumentSpecifications)
            {
                List<IAttributeValidator> validators = new();

                foreach (var factory in arg.Attributes.OfType<IAttributeValidatorFactory>())
                {
                    validators.Add(factory.CreateValidatorInstance());
                }

                _validators.Add(arg, validators);
            }
        }

        /// <summary>
        /// All arguments that this target can receive.
        /// </summary>
        /// <returns>A set of arguments.</returns>
        public HashSet<ArgumentSpecification> GetArguments() => Schema.ArgumentSpecifications;


        /// <summary>
        /// Validates that this target is properly constructed.
        /// </summary>
        /// <param name="errorMessage">The error message if failed.</param>
        /// <returns>True if validation passes.</returns>
        public bool Validate(out string errorMessage)
        {
            errorMessage = null;

            foreach (var kvp in _validators)
            {
                foreach (var validator in kvp.Value)
                {
                    if (validator.Validate(kvp.Key)) continue;

                    errorMessage = validator.ErrorMessage();
                    return false;
                }
            }

            foreach (var cmdValidator in  _commandValidators)
            {
                if (cmdValidator.Validate(this, out var error)) continue;

                errorMessage = error;
                return false;
            }

            return true;
        }


        /// <summary>
        /// Sets an argument to a given value.
        /// </summary>
        /// <param name="argument">The argument to set.</param>
        /// <param name="argValue">The value to set it to.</param>
        public void SetArgument(ArgumentSpecification argument, object argValue)
        {
            argument.FieldInfo.SetValue(Command, argValue);
            foreach (var validator in _validators[argument])
            {
                validator.Record(new RecordingContext
                {
                    ArgumentSpecification = argument,
                    ArgumentValue = argValue,
                    CommandParseTarget = this
                });
            }
        }
    }
}
