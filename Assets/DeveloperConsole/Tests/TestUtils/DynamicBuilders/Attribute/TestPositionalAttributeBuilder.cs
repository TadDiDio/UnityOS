using DeveloperConsole.Command;

namespace DeveloperConsole.Tests
{
    public class TestPositionalAttributeBuilder : AttributeDataBuilder<PositionalAttribute>
    {
        protected override AttributeData GetBuildData(PositionalAttribute attribute)
        {
            return new AttributeData
            (
                attribute.GetType().GetConstructor(new[] { typeof(int), typeof(string), typeof(string) }),
                new object[] { attribute.Index, attribute.Description, attribute.Name }
            );
        }
    }
}