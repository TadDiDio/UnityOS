using DeveloperConsole.Command;

namespace DeveloperConsole.Tests
{
    public class TestSwitchAttributeBuilder : AttributeDataBuilder<SwitchAttribute>
    {
        protected override AttributeData Build(SwitchAttribute attribute)
        {
            return new AttributeData
            (
                attribute.GetType().GetConstructor(new[] { typeof(string), typeof(string), typeof(string) }),
                new object[] { attribute.ShortName, attribute.Description, attribute.Name }
            );
        }
    }
}