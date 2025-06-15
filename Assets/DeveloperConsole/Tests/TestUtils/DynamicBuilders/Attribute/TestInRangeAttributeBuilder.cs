using System;
using DeveloperConsole.Command;

namespace DeveloperConsole.Tests
{
    public class TestInRangeAttributeBuilder : AttributeDataBuilder<InRangeAttribute>
    {
        protected override AttributeData GetBuildData(InRangeAttribute attribute)
        {
            return new AttributeData
            (
                attribute.GetType().GetConstructor(new Type[] { typeof(float), typeof(float) }),
                new object[] { attribute.Min, attribute.Max}
            );
        }
    }
}