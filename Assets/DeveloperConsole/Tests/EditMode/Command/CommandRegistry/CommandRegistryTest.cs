using System;
using System.Collections.Generic;
using System.Linq;
using DeveloperConsole.Command;

using NUnit.Framework;

namespace DeveloperConsole.Tests.Command
{
    public class CommandRegistryTest : ConsoleTest
    {
        private class MockDiscovery : ICommandDiscoveryStrategy
        {
            private readonly List<Type> _types;
            public MockDiscovery(params Type[] types) => _types = types.ToList();
            public List<Type> GetAllCommandTypes() => _types;
        }

        [Test]
        public void RegistersSingleRootCommand()
        {
            var rootType = new CommandBuilder()
                .WithName("root")
                .BuildType();

            var registry = new CommandRegistry(new MockDiscovery(rootType));

            Assert.That(registry.AllCommandNames(), Contains.Item("root"));
            Assert.True(registry.TryResolveCommandSchema("root", out var schema));
            Assert.AreEqual("root", schema.Name);
            Assert.AreEqual(rootType, schema.CommandType);
            Assert.IsNull(schema.ParentSchema);
        }

        [Test]
        public void RegistersRootAndSubcommandHierarchy()
        {
            var root = new CommandBuilder().WithName("root").BuildType();
            var child = new CommandBuilder().WithName("child").AsSubcommand(root).BuildType();
            var grandchild = new CommandBuilder().WithName("grandchild").AsSubcommand(child).BuildType();

            var registry = new CommandRegistry(new MockDiscovery(root, child, grandchild));

            Assert.That(registry.AllCommandNames(), Contains.Item("root.child"));
            Assert.That(registry.AllCommandNames(), Contains.Item("root.child.grandchild"));

            Assert.True(registry.TryResolveCommandSchema(new List<string> { "root", "child", "grandchild" }, out var schema));
            Assert.AreEqual("grandchild", schema.Name);
            Assert.AreEqual(grandchild, schema.CommandType);
        }

        [Test]
        public void GetFullyQualifiedCommandName_ReturnsExpectedValue()
        {
            var root = new CommandBuilder().WithName("root").BuildType();
            var child = new CommandBuilder().WithName("child").AsSubcommand(root).BuildType();

            var registry = new CommandRegistry(new MockDiscovery(root, child));
            var fqName = registry.GetFullyQualifiedCommandName(child);

            Assert.AreEqual("root.child", fqName);
        }

        [Test]
        public void TryResolveCommandSchema_PartialPath_ReturnsTrueHalfWay()
        {
            var root = new CommandBuilder().WithName("root").BuildType();

            var registry = new CommandRegistry(new MockDiscovery(root));
            Assert.True(registry.TryResolveCommandSchema(new List<string> { "root", "nonexistent" }, out var schema));
            Assert.AreEqual("root", schema.Name);
        }

        [Test]
        public void RegisterCommand_DynamicallyAddsCommand()
        {
            var root = new CommandBuilder().WithName("root").BuildType();
            var registry = new CommandRegistry(new MockDiscovery(root));

            var child = new CommandBuilder().WithName("child").AsSubcommand(root).BuildType();

            registry.RegisterCommand(child);

            Assert.That(registry.AllCommandNames(), Contains.Item("root.child"));
        }

        [Test]
        public void RegistersArgumentsProperly()
        {
            var command = new CommandBuilder()
                .WithName("args")
                .WithSwitch("flag", typeof(bool))
                .WithPositional("target", typeof(string), index: 0)
                .WithVariadic("values", typeof(int))
                .BuildType();

            var registry = new CommandRegistry(new MockDiscovery(command));

            Assert.True(registry.TryResolveCommandSchema("args", out var schema));
            var args = schema.ArgumentSpecifications;

            Assert.IsNotNull(args);
            Assert.AreEqual(3, args.Count);
            Assert.IsTrue(args.Any(arg => arg.Name == "flag"));
            Assert.IsTrue(args.Any(arg => arg.Name == "target"));
            Assert.IsTrue(args.Any(arg => arg.Name == "values"));
        }
    }
}