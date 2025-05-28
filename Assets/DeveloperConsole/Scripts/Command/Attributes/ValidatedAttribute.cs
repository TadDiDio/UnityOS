using System;
using System.Reflection;

namespace DeveloperConsole
{
    public abstract class ValidatedAttribute : Attribute
    {
        public abstract bool Validate(AttributeValidationData data);
    }

    public class AttributeValidationData
    {
        public FieldInfo FieldInfo;
        public object Object;
        public bool WasSet;
    }
}