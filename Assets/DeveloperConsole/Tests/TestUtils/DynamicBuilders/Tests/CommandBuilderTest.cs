using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DeveloperConsole.Command;
using NUnit.Framework;
using Unity.Hierarchy;
using UnityEngine;

namespace DeveloperConsole.Tests.TestUtils
{
    public class CommandBuilderTest
    {
        [Test]
        public void CommandBuilder_DefaultType()
        {
            var builder = new CommandBuilder();
            Type commandType = builder.BuildType();

            Assert.NotNull(commandType);
            Assert.IsTrue(typeof(AsyncCommand).IsAssignableFrom(commandType));

            var commandAttr = commandType.GetCustomAttribute<CommandAttribute>();
            Assert.NotNull(commandAttr);
            Assert.That(commandAttr.Name, Does.StartWith("test_"));  // default generated name
        }

        [Test]
        public void CommandBuilder_WithName()
        {
            var builder = new CommandBuilder().WithName("MyCommand");
            var type = builder.BuildType();

            var cmdAttr = type.GetCustomAttribute<CommandAttribute>();
            Assert.NotNull(cmdAttr);
            Assert.AreEqual(CommandMetaProcessor.Name("MyCommand"), cmdAttr.Name);
        }

        [Test]
        public void CommandBuilder_Subcommand()
        {
            Type parentType = typeof(SimpleCommand);  // just use base class as parent

            var builder = new CommandBuilder()
                .WithName("child")
                .AsSubcommand(parentType);

            Type type = builder.BuildType();
            var subAttr = type.GetCustomAttribute<SubcommandAttribute>();
            Assert.NotNull(subAttr);
            Assert.AreEqual(CommandMetaProcessor.Name("child"), subAttr.Name);
            Assert.AreEqual(parentType, subAttr.ParentCommandType);
        }

        [Test]
        public void CommandBuilder_WithSwitch()
        {
            var builder = new CommandBuilder()
                .WithSwitch("verbose", typeof(bool));

            var type = builder.BuildType();
            var field = type.GetField("verbose");
            Assert.NotNull(field);

            var switchAttr = field.GetCustomAttribute<SwitchAttribute>();
            Assert.NotNull(switchAttr);
            Assert.AreEqual('v', switchAttr.Alias);  // default alias is first char
        }

        [Test]
        public void CommandBuilder_WithPositional()
        {
            var builder = new CommandBuilder()
                .WithPositional("count", typeof(int), 2);

            var type = builder.BuildType();
            var field = type.GetField("count");
            Assert.NotNull(field);

            var positionalAttr = field.GetCustomAttribute<PositionalAttribute>();
            Assert.NotNull(positionalAttr);
            Assert.AreEqual(2, positionalAttr.Index);
        }

        [Test]
        public void CommandBuilder_WithVariadic()
        {
            var builder = new CommandBuilder()
                .WithVariadic("files", typeof(string));

            var type = builder.BuildType();
            var field = type.GetField("files");
            Assert.NotNull(field);

            Assert.That(field.FieldType.IsGenericType);
            Assert.AreEqual(typeof(List<>), field.FieldType.GetGenericTypeDefinition());

            var variadicAttr = field.GetCustomAttribute<VariadicAttribute>();
            Assert.NotNull(variadicAttr);
        }

        [Test]
        public void CommandBuilder_WithBadPreExecutionValidator()
        {
            using SilentLogCapture log = new();

            var validator = new DummyPreExecutionValidatorAttribute();
            var builder = new CommandBuilder()
                .WithPrevalidator(validator);

            var type = builder.BuildType();

            Assert.Null(type);
            Assert.AreEqual(log.Count(LogType.Error), 1);
            Assert.True(log.HasLog(LogType.Error, "Pre-execution validator"));
        }

        [Test]
        public void CommandBuilder_WithInstantiation()
        {
            var builder = new CommandBuilder()
                .WithName("TestCmd")
                .WithSwitch("flag", typeof(bool));

            ICommand command = builder.Build();

            Assert.NotNull(command);
            Assert.IsInstanceOf<ICommand>(command);
            Assert.AreEqual(CommandMetaProcessor.Name("TestCmd"), command.GetType().GetCustomAttribute<CommandAttribute>().Name);
        }
    }
    public class DummyPreExecutionValidatorAttribute : PreExecutionValidatorAttribute
    {
        public override Task<bool> Validate(CommandContext context, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
