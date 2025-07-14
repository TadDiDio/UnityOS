using System;
using DeveloperConsole.Parsing;

namespace DeveloperConsole.Command
{
    /// <summary>
    /// Specifies to inject an object of this field's type for use in the command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class BindingAttribute : ArgumentAttribute, IValidatedAttribute
    {
        public readonly string Name;
        public readonly string Tag;

        private Type _type;

        private ICommandParseTarget _commandParseTarget;

        /// <summary>
        /// Injects an object of this type if one is or can be bound.
        /// </summary>
        /// <param name="name">Prefer an object with this name.</param>
        /// <param name="tag">Prefer an object with this tag.</param>
        public BindingAttribute(string name = "", string tag = "")
        {
            Tag = tag;
            Name = name;
        }


        public void Record(RecordingContext context)
        {
            _commandParseTarget = context.CommandParseTarget;
        }


        public bool Validate(ArgumentSpecification spec)
        {
            _type = spec.FieldInfo.FieldType;
            bool success = ConsoleAPI.Bindings.TryGetBinding(_type, Name, Tag, out var obj);

            if (!success) return false;

            _commandParseTarget.SetArgument(spec, obj);
            return true;
        }


        public string ErrorMessage()
        {
            return $"There is no binding of type '{_type}' and one could not be found in the loaded scenes. " +
                   $"Try using the 'bind' command to set a binding.";
        }
    }
}
