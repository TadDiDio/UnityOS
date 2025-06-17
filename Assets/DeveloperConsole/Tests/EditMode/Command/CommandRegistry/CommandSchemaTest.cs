using System.Collections.Generic;
using DeveloperConsole.Command;
using NUnit.Framework;

namespace DeveloperConsole.Tests.Command
{
    public class CommandSchemaTest
    {
        [Test]
        public void CommandSchema_CanBeCreated_WithBasicInfo()
        {
            var schema = new CommandSchema
            {
                Name = "test",
                Description = "This is a test command.",
                CommandType = typeof(string),
                Subcommands = new HashSet<CommandSchema>(),
                ArgumentSpecifications = new HashSet<ArgumentSpecification>()
            };

            Assert.AreEqual("test", schema.Name);
            Assert.AreEqual("This is a test command.", schema.Description);
            Assert.AreEqual(typeof(string), schema.CommandType);
            Assert.IsNotNull(schema.Subcommands);
            Assert.IsNotNull(schema.ArgumentSpecifications);
            Assert.IsNull(schema.ParentSchema);
        }

        [Test]
        public void CommandSchema_CanReferenceParentCommand()
        {
            var parent = new CommandSchema { Name = "parent", Subcommands = new HashSet<CommandSchema>(), ArgumentSpecifications = new HashSet<ArgumentSpecification>() };
            var child = new CommandSchema { Name = "child", ParentSchema = parent, Subcommands = new HashSet<CommandSchema>(), ArgumentSpecifications = new HashSet<ArgumentSpecification>() };

            parent.Subcommands.Add(child);

            Assert.AreEqual(parent, child.ParentSchema);
            Assert.Contains(child, new List<CommandSchema>(parent.Subcommands));
        }

        [Test]
        public void CommandSchema_CanContainMultipleArguments()
        {
            var schema = new CommandSchema
            {
                Name = "args",
                Subcommands = new HashSet<CommandSchema>(),
                ArgumentSpecifications = new HashSet<ArgumentSpecification>()
            };

            var field1 = new FieldBuilder()
                .WithName("test1")
                .WithType(typeof(string))
                .Build();
            
            var field2 = new FieldBuilder()
                .WithName("test2")
                .WithType(typeof(int))
                .Build();
            
            var arg1 = new ArgumentSpecification(field1);
            var arg2 = new ArgumentSpecification(field2);

            schema.ArgumentSpecifications.Add(arg1);
            schema.ArgumentSpecifications.Add(arg2);

            Assert.Contains(arg1, new List<ArgumentSpecification>(schema.ArgumentSpecifications));
            Assert.Contains(arg2, new List<ArgumentSpecification>(schema.ArgumentSpecifications));
        }

        [Test]
        public void CommandSchema_Subcommands_HaveCorrectParentReference()
        {
            var parent = new CommandSchema
            {
                Name = "root",
                Subcommands = new HashSet<CommandSchema>(),
                ArgumentSpecifications = new HashSet<ArgumentSpecification>()
            };

            var sub = new CommandSchema
            {
                Name = "child",
                ParentSchema = parent,
                Subcommands = new HashSet<CommandSchema>(),
                ArgumentSpecifications = new HashSet<ArgumentSpecification>()
            };

            parent.Subcommands.Add(sub);

            Assert.AreEqual(parent, sub.ParentSchema);
        }
    }
}