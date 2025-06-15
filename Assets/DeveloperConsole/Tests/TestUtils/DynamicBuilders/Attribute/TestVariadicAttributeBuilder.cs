using DeveloperConsole.Command;

namespace DeveloperConsole.Tests
{
    public class TestVariadicAttributeBuilder : AttributeDataBuilder<VariadicAttribute>
    {
        protected override AttributeData GetBuildData(VariadicAttribute attribute)
        {
            return new AttributeData
            (
                attribute.GetType().GetConstructor(new[] { typeof(string), typeof(string) }),
                new object[] { attribute.Description, attribute.Name }
            );
        }
    }
}