using System;
using DeveloperConsole.Command;

namespace DeveloperConsole.Tests
{
    public class TestCommandAttributeBuilder : AttributeDataBuilder<CommandAttribute>
    {
        protected override AttributeData GetBuildData(CommandAttribute attribute)
        {
            return new AttributeData
            (
                attribute.GetType().GetConstructor(new[] { typeof(string), typeof(string) }),
                new object[] { attribute.Name, attribute.Description }
            );
        }
    }
}