using System;
using System.Collections.Generic;
using System.Reflection;

namespace DeveloperConsole.Tests
{
    public static class AttributeBuilderRegistry
    {
        private static readonly Dictionary<Type, IAttributeDataBuilder> _providers = new();

        static AttributeBuilderRegistry()
        {
            Register(new TestCommandAttributeBuilder());
            Register(new TestSubcommandAttributeBuilder());
            Register(new TestSwitchAttributeBuilder());
            Register(new TestPositionalAttributeBuilder());
            Register(new TestVariadicAttributeBuilder());
        }
        
        public static void Register<T>(AttributeDataBuilder<T> dataBuilder) where T : Attribute
        {
            _providers[typeof(T)] = dataBuilder;
        }

        public static bool TryGet(Attribute attr, out AttributeData data)
        {
            if (_providers.TryGetValue(attr.GetType(), out var provider))
            {
                data = provider.GetBuildData(attr);
                return true;
            }

            data = null;
            return false;
        }
    }

    public interface IAttributeDataBuilder
    {
        AttributeData GetBuildData(object attributeInstance);
    }

    public abstract class AttributeDataBuilder<T> : IAttributeDataBuilder where T : Attribute
    {
        protected abstract AttributeData GetBuildData(T attribute);
        public AttributeData GetBuildData(object attributeInstance)
        {
            if (attributeInstance is Attribute attribute) return GetBuildData((T)attribute);
            
            Log.Error("The injected object was not an attribute.");
            return null;
        }
    }
    
    public class AttributeData
    {
        public readonly object[] Arguments;
        public readonly ConstructorInfo ConstructorInfo;

        public AttributeData(ConstructorInfo constructorInfo, object[] arguments)
        {
            Arguments = arguments;
            ConstructorInfo = constructorInfo;
        }
    }
}