using System;
using DeveloperConsole.Command;

namespace DeveloperConsole.Tests
{
    public class TestSubcommandAttributeBuilder : AttributeDataBuilder<SubcommandAttribute>
    {
        protected override AttributeData Build(SubcommandAttribute attribute)
        {
            return new AttributeData
            (
                attribute.GetType().GetConstructor(new[] { typeof(string), typeof(string), typeof(Type) }),
                new object[] { attribute.Name, attribute.Description, attribute.ParentCommandType }
            );
        }
    }
}