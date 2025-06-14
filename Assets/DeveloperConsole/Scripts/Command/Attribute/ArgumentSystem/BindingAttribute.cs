using System;
using DeveloperConsole.Parsing;
using Object = UnityEngine.Object;

namespace DeveloperConsole.Command
{
    /// <summary>
    /// Specifies to inject an object of this field's type for use in the command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class BindingAttribute : ValidatedAttribute
    {
        private Type _type;
        private string _tag;
        private string _name;
        
        
        /// <summary>
        /// Injects an object of this type if one is or can be bound.
        /// </summary>
        /// <param name="description">The arg's description.</param>
        /// <param name="name">Prefer an object with this name.</param>
        /// <param name="tag">Prefer an object with this tag.</param>
        /// <param name="overrideName">Overrides the field name.</param>
        public BindingAttribute(string description, string name = "", string tag = "", string overrideName = null)
            : base(description, overrideName)
        {
            _tag = tag;
            _name = name;
        }

        public override bool Validate(ParseContext context)
        {
            var result = context.Target.GetFirstArgumentMatchingAttribute(this);
            if (!result.HasValue) return false;

            var (spec, _) = result.Value;
            _type = spec.FieldInfo.FieldType;
            return ConsoleAPI.TryGetBinding(_type, _name, _tag, out var obj) && 
                   BindObject(spec, context.Target, obj);
        }

        private bool BindObject(ArgumentSpecification spec, object parseTarget, Object target)
        {
            try
            {
                spec.FieldInfo.SetValue(parseTarget, target);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public override string ErrorMessage()
        {
            return $"There is no binding of type '{_type}' and one could not be found in the loaded scenes. " +
                   $"Try using the 'bind' command to set a binding.";
        }
    }
}