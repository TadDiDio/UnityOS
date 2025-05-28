using System;

namespace DeveloperConsole
{
    [AttributeUsage(AttributeTargets.Field)]
    public class PositionalArgAttribute : Attribute
    {
        public int Index { get; }
        public PositionalArgAttribute(int index) => Index = index;
    }
}