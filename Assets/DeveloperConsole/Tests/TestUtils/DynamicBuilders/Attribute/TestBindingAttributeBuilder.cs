using DeveloperConsole.Command;

namespace DeveloperConsole.Tests
{
    public class TestBindingAttributeBuilder : AttributeDataBuilder<BindAttribute>
    {
        protected override AttributeData GetBuildData(BindAttribute attribute)
        {
            return new AttributeData
            (
                attribute.GetType().GetConstructor(new[] { typeof(string), typeof(string) }),
                new object[] { attribute.Name, attribute.Tag }
            );
        }
    }
}