using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DeveloperConsole.Command;

namespace DeveloperConsole.Tests
{
    public class FieldBuilder
    {
        private string _name = "test";
        private Type _type = typeof(int);
        
        private List<AttributeData> _attributes = new();

        public FieldBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        public FieldBuilder WithType(Type type)
        {
            _type = type;
            return this;
        }

        public FieldBuilder WithAttribute(ArgumentAttribute argumentAttribute)
        {
            if (!AttributeBuilderRegistry.TryBuild(argumentAttribute, out var data))
            {
                Log.Error($"Argument attribute '{argumentAttribute.GetType().Name}' could not be built in a test.");
                return null;
            }
            
            _attributes.Add(data);
            return this;
        }
        
        
        public Type BuildType()
        {
            // Define a public class named "DynamicClass_<num>" in the assembly.
            var typeBuilder = DynamicAssemblyCache.ModuleBuilder.DefineType($"TestDynamicClass_{DynamicAssemblyCache.NextId}", TypeAttributes.Public);
            var fieldBuilder = typeBuilder.DefineField(_name, _type, FieldAttributes.Public);

            foreach (var data in _attributes)
            {
                var builder = new CustomAttributeBuilder(data.ConstructorInfo, data.Arguments);
                fieldBuilder.SetCustomAttribute(builder);
            }
            
            return typeBuilder.CreateType();
        }
        
        public FieldInfo Build()
        {
            return Activator.CreateInstance(BuildType()).GetType().GetField(_name);
        }
    }
}