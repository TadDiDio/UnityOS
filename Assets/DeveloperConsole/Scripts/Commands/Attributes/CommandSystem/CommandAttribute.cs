using System;

namespace DeveloperConsole
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CommandAttribute : Attribute
    {
        public bool IsRoot { get; }

        private string _rawName;
        private string _rawDescription;
        
        public string Name => CommandMetaProcessor.Name(_rawName);
        public string Description => CommandMetaProcessor.Description(_rawDescription);

        public CommandAttribute(string name, string description, bool isRoot)
        {
            _rawName = name;
            _rawDescription = description;
            IsRoot = isRoot;
        }
    }

}