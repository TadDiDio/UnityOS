using System;

namespace DeveloperConsole.Command
{
    /// <summary>
    /// Specifies to inject an object of this field's type for use in the command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class BindingAttribute : ArgumentAttribute, IAttributeValidatorFactory
    {
        public readonly string Name;
        public readonly string Tag;

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

        public IAttributeValidator CreateValidatorInstance()
        {
            return new BindingAttributeValidator(Name, Tag);
        }
    }
}
