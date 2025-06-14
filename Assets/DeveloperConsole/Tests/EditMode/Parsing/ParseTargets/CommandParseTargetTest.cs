using System;
using System.Linq;
using DeveloperConsole.Command;
using DeveloperConsole.Parsing;
using NUnit.Framework;

namespace DeveloperConsole.Tests.EditMode.Parsing.ParseTargets
{
    public class CommandParseTargetTest
    {
        [Test]
        public void Constructor_CreatesCommandInstance()
        {
            var builder = new CommandBuilder()
                .WithSwitch("flag", typeof(bool), "f");
            Type commandType = builder.BuildType();

            var argSpecs = commandType.GetFields()
                .Select(f => new ArgumentSpecification(f))
                .ToHashSet();

            var schema = new CommandSchema
            {
                CommandType = commandType,
                ArgumentSpecifications = argSpecs
            };

            var target = new CommandParseTarget(schema);

            Assert.IsNotNull(target.Command);
            Assert.AreEqual(commandType, target.Command.GetType());
        }

        [Test]
        public void GetArguments_ReturnsSchemaArguments()
        {
            var builder = new CommandBuilder()
                .WithSwitch("flag", typeof(bool), "f");
            Type commandType = builder.BuildType();

            var argSpecs = commandType.GetFields()
                .Select(f => new ArgumentSpecification(f))
                .ToHashSet();

            var schema = new CommandSchema
            {
                CommandType = commandType,
                ArgumentSpecifications = argSpecs
            };

            var target = new CommandParseTarget(schema);
            var arguments = target.GetArguments();

            Assert.AreEqual(argSpecs.Count, arguments.Count);
            CollectionAssert.AreEquivalent(argSpecs, arguments);
        }

        [Test]
        public void SetArgument_SetsValueOnCommand()
        {
            var builder = new CommandBuilder()
                .WithSwitch("flag", typeof(bool), "f");
            Type commandType = builder.BuildType();

            var argSpecs = commandType.GetFields()
                .Select(f => new ArgumentSpecification(f))
                .ToHashSet();

            var schema = new CommandSchema
            {
                CommandType = commandType,
                ArgumentSpecifications = argSpecs
            };

            var target = new CommandParseTarget(schema);
            var argSpec = argSpecs.First();

            target.SetArgument(argSpec, true);

            var value = argSpec.FieldInfo.GetValue(target.Command);
            Assert.AreEqual(true, value);
        }
    }
}