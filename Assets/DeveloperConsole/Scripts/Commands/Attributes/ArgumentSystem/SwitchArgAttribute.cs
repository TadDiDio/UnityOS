using System;

namespace DeveloperConsole
{
    [AttributeUsage(AttributeTargets.Field)]
    public class SwitchArgAttribute : Attribute
    {
        public string Name { get; }
        public string ShortName { get; }
        public SwitchArgAttribute(string name, char shortName)
        {
            Name = $"--{name.TrimStart('-')}";
            ShortName = $"-{shortName}";
        }
    }
}