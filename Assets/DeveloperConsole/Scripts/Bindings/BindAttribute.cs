using System;

namespace DeveloperConsole.Command
{
    /// <summary>
    /// Specifies to inject an object of this field's type for use in the command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class BindAttribute : Attribute
    {
        public readonly string Name;
        public readonly string Tag;

        /// <summary>
        /// Injects an object of this type automatically. If the object is a GameObject
        /// </summary>
        /// <param name="name">Prefer an object with this name.</param>
        /// <param name="tag">Prefer an object with this tag.</param>
        public BindAttribute(string name = "", string tag = "")
        {
            Tag = tag;
            Name = name;
        }
    }
}
