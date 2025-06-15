using System;
using DeveloperConsole.Command;

namespace DeveloperConsole.Tests
{
    public class TestRequiredAttributeBuilder : AttributeDataBuilder<RequiredAttribute>
    {
        protected override AttributeData GetBuildData(RequiredAttribute attribute)
        {
            return new AttributeData
            (  
                attribute.GetType().GetConstructor(Type.EmptyTypes),
                null
            );
        }
    }
}