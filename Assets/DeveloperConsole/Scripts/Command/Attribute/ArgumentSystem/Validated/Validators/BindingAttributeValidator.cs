using System;
using DeveloperConsole.Parsing;

namespace DeveloperConsole.Command
{
    public class BindingAttributeValidator : IAttributeValidator
    {
        private Type _type;
        private CommandParseTarget _commandParseTarget;

        private string _name;
        private string _tag;

        public BindingAttributeValidator(string name, string tag)
        {
            _name = name;
            _tag = tag;
        }

        public void Record(RecordingContext context)
        {
            _commandParseTarget = context.CommandParseTarget;
        }

        public bool Validate(ArgumentSpecification spec)
        {
            _type = spec.FieldInfo.FieldType;
            bool success = ConsoleAPI.Bindings.TryGetBinding(_type, _name, _tag, out var obj);

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
