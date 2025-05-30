using System;

namespace DeveloperConsole
{
    public class CommandAttribute : Attribute
    {
        public string Name { get; }
        public string Description { get; }
        public bool IsSubcommand { get; }

        public CommandAttribute(string name, string description, bool isSubcommand = false)
        {
            Name = name;
            Description = description;
            IsSubcommand = isSubcommand;
        }
    }
}