using System;

namespace DeveloperConsole
{
    [AttributeUsage(AttributeTargets.Field)]
    public class SubcommandAttribute : Attribute
    {
        public string Name { get; }
        
        public SubcommandAttribute(string name) => Name = name;
    }
}