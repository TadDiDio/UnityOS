using DeveloperConsole.Command;

namespace DeveloperConsole.Tests
{
    public class TestBindingAttributeBuilder : AttributeDataBuilder<BindingAttribute>
    {
        protected override AttributeData GetBuildData(BindingAttribute attribute)
        {
            return new AttributeData
            (
                attribute.GetType().GetConstructor(new[] { typeof(string), typeof(string) }),
                new object[] { attribute.Name, attribute.Tag }
            );
        }
    }
}