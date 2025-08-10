using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;
using DeveloperConsole.Command;

namespace DeveloperConsole.Tests
{
    public class CommandBuilder
    {
        private string _name;
        private Type _parentType;
        private Type _baseType = typeof(AsyncCommand);
        private List<FieldInfo> fields = new();
        private List<PreExecutionValidatorAttribute> prevalidators = new();


        /// <summary>
        /// Adds a name for the command via the attribute. Names for multiple
        /// command don't need to be unique for the sake of avoiding assembly collisions.
        /// </summary>
        /// <param name="name">The command name.</param>
        /// <returns>The builder.</returns>
        public CommandBuilder WithName(string name)
        {
            _name = name;
            return this;
        }


        /// <summary>
        /// Makes it a subcommand.
        /// </summary>
        /// <param name="parentType">The parent type.</param>
        /// <returns>The builder.</returns>
        public CommandBuilder AsSubcommand(Type parentType)
        {
            _parentType = parentType;
            return this;
        }


        /// <summary>
        /// Adds a pre-execution validator.
        /// </summary>
        /// <param name="attribute">The validator.</param>
        /// <returns>The builder.</returns>
        public CommandBuilder WithPrevalidator(PreExecutionValidatorAttribute attribute)
        {
            prevalidators.Add(attribute);
            return this;
        }


        /// <summary>
        /// Adds a switch arg to the command.
        /// </summary>
        /// <param name="name">The field name.</param>
        /// <param name="type">The field type.</param>
        /// <param name="alias">The alias, defaulted to the first letter of name.</param>
        /// <returns>The builder.</returns>
        public CommandBuilder WithSwitch(string name, Type type, char alias = '\0')
        {
            char workingAlias = alias == '\0' ? name[0] : alias;
            var field = new FieldBuilder()
                .WithName(name)
                .WithType(type)
                .WithAttribute(new SwitchAttribute(workingAlias, "desc"))
                .Build();

            fields.Add(field);
            return this;
        }


        /// <summary>
        /// Adds a positional arg to the command.
        /// </summary>
        /// <param name="name">The field name.</param>
        /// <param name="type">The field type.</param>
        /// <param name="index">The positional index.</param>
        /// <returns>The builder.</returns>
        public CommandBuilder WithPositional(string name, Type type, int index = 0)
        {
            var field = new FieldBuilder()
                .WithName(name)
                .WithType(type)
                .WithAttribute(new PositionalAttribute(index, "desc"))
                .Build();

            fields.Add(field);
            return this;
        }


        /// <summary>
        /// Adds variadic args to the command.
        /// </summary>
        /// <param name="name">The field name.</param>
        /// <param name="elementType">The field type.</param>
        /// <returns>The builder.</returns>
        public CommandBuilder WithVariadic(string name, Type elementType)
        {
            var listType = typeof(List<>).MakeGenericType(elementType);

            var field = new FieldBuilder()
                .WithName(name)
                .WithType(listType)
                .WithAttribute(new VariadicAttribute("desc"))
                .Build();

            fields.Add(field);
            return this;
        }


        /// <summary>
        /// Defaults to unique name, simple command base, not a subcommand, no fields or prevalidators.
        /// </summary>
        /// <returns>The command type.</returns>
        public Type BuildType()
        {
            var typeBuilder = DynamicAssemblyCache.ModuleBuilder.DefineType($"TestDynamicClass_{DynamicAssemblyCache.NextId}", TypeAttributes.Public, _baseType);

            foreach (var field in fields)
            {
                var fieldBuilder = typeBuilder.DefineField(field.Name, field.FieldType, FieldAttributes.Public);

                foreach (var attribute in field.GetCustomAttributes<ArgumentAttribute>())
                {
                    if (!AttributeBuilderRegistry.TryGet(attribute, out var data))
                    {
                        Log.Error($"Attribute {attribute} could not be built during test.");
                        return null;
                    }
                    fieldBuilder.SetCustomAttribute(new CustomAttributeBuilder(data.ConstructorInfo, data.Arguments));
                }
            }

            foreach (var prevalidator in prevalidators)
            {
                if (!AttributeBuilderRegistry.TryGet(prevalidator, out var data))
                {
                    Log.Error($"Pre-execution validator {prevalidator} could not be built during test.");
                    return null;
                }
                typeBuilder.SetCustomAttribute(new CustomAttributeBuilder(data.ConstructorInfo, data.Arguments));
            }

            string name = _name ?? $"test_{DynamicAssemblyCache.NextId}";
            CommandAttribute commandAttribute = _parentType == null ? new CommandAttribute(name, "desc") :
                new CommandAttribute(name, "desc", _parentType);

            if (!AttributeBuilderRegistry.TryGet(commandAttribute, out var commandData))
            {
                Log.Error($"Could not build a {commandAttribute.Name} during testing.");
                return null;
            }

            GenerateFakeExecute(typeBuilder);

            typeBuilder.SetCustomAttribute(new CustomAttributeBuilder(commandData.ConstructorInfo, commandData.Arguments));
            return typeBuilder.CreateType();
        }


        private void GenerateFakeExecute(TypeBuilder typeBuilder)
        {
            var methodToOverride = _baseType.GetMethod("ExecuteAsync");

            // Create a method override for ExecuteAsync
            var methodBuilder = typeBuilder.DefineMethod(
                methodToOverride!.Name,
                MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig,
                methodToOverride.ReturnType,
                new [] { typeof(CommandContext) });

            // Generate IL for the method (return Task.FromResult(default(CommandOutput)))
            var il = methodBuilder.GetILGenerator();

            // Prepare to call Task.FromResult(CommandOutput)
            var commandOutputType = typeof(CommandOutput);
            var fromResultMethod = typeof(Task).GetMethod(nameof(Task.FromResult))!.MakeGenericMethod(commandOutputType);

            // Load default(CommandOutput) onto stack
            var local = il.DeclareLocal(commandOutputType);
            il.Emit(OpCodes.Ldloca_S, local);
            il.Emit(OpCodes.Initobj, commandOutputType);  // initialize default value

            // Load the local (default CommandOutput)
            il.Emit(OpCodes.Ldloc_0);

            // Call Task.FromResult(default(CommandOutput))
            il.Emit(OpCodes.Call, fromResultMethod);

            // Return the Task<CommandOutput>
            il.Emit(OpCodes.Ret);

            // Mark this method as override of the abstract base method
            typeBuilder.DefineMethodOverride(methodBuilder, methodToOverride);
        }

        /// <summary>
        /// Defaults to unique name, simple command base, not a subcommand, no fields or prevalidators.
        /// </summary>
        /// <returns>The command.</returns>
        public ICommand Build()
        {
            return (ICommand)Activator.CreateInstance(BuildType());
        }
    }
}
