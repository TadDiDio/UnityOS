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

        public static bool TryBuild(Attribute attr, out AttributeData data)
        {
            if (_providers.TryGetValue(attr.GetType(), out var provider))
            {
                data = provider.Build(attr);
                return true;
            }

            data = null;
            return false;
        }
    }

    public interface IAttributeDataBuilder
    {
        AttributeData Build(object attributeInstance);
    }

    public abstract class AttributeDataBuilder<T> : IAttributeDataBuilder where T : Attribute
    {
        protected abstract AttributeData Build(T attribute);
        public AttributeData Build(object attributeInstance)
        {
            if (attributeInstance is not Attribute attribute)
            {
                Log.Error("The injected object was not an attribute.");
                return default;
            }

            return Build((T)attribute);
        }
    }
    
    public class AttributeData
    {
        public object[] Arguments;
        public ConstructorInfo ConstructorInfo;

        public AttributeData(ConstructorInfo constructorInfo, object[] arguments)
        {
            Arguments = arguments;
            ConstructorInfo = constructorInfo;
        }
    }
}