using System;

namespace DeveloperConsole
{
    [AttributeUsage(AttributeTargets.Field)]
    public class DescriptionAttribute : Attribute
    {
        private string _rawDescription;
        public string Description => CommandMetaProcessor.Description(_rawDescription);
        public DescriptionAttribute(string description) => _rawDescription = description;
    }
}