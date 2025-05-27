using System;

namespace DeveloperConsole
{
    [AttributeUsage(AttributeTargets.Field)]
    public class PositionalArgAttribute : Attribute
    {
        public int index { get; }
        public PositionalArgAttribute(int index) => this.index = index;
    }
}