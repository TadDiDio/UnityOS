using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace DeveloperConsole.Tests
{
    public class CommandRegistryTest
    {
        #region TEST TYPES
        [ExcludeFromCmdRegistry]
        [Command("test1", "Test command 1", false)]
        private class TestCommand1 : SimpleCommand
        {
            [Subcommand] private Subcommand subcommand;
            protected override CommandResult Execute(CommandContext context)
            {
                throw new NotImplementedException();
            }
        }

        [ExcludeFromCmdRegistry]
        [Command("sub", "Subcommand of test1", true)]
        private class Subcommand : SimpleCommand
        {
            [Subcommand] private SubSubcommand subcommand = new();
            protected override CommandResult Execute(CommandContext context)
            {
                throw new NotImplementedException();
            }
        }
        
        [ExcludeFromCmdRegistry]
        [Command("subsub", "subcommand of subcommand", true)]
        private class SubSubcommand : SimpleCommand
        {
            protected override CommandResult Execute(CommandContext context)
            {
                throw new NotImplementedException();
            }
        }
        #endregion

        private CommandRegistry _commandRegistry;
        
        [SetUp]
        public void SetUp()
        {
            Dictionary<string, Type> commands = new()
            {
                { "test1", typeof(TestCommand1) },
                { "test1.sub", typeof(Subcommand) },
                { "test1.sub.subsub", typeof(SubSubcommand) }
            };

            _commandRegistry = new CommandRegistry(commands);
        }

        [Test]
        public void CommandRegistry_GetBaseCommandNames()
        {
            var result = _commandRegistry.GetBaseCommandNames();
            
            Assert.AreEqual(result.Count, 1);
            Assert.AreEqual(result[0], "test1");
        }
        
        [Test]
        public void CommandRegistry_TryGetCommandSuccesses()
        {
            bool success = _commandRegistry.TryGetCommand("test1", out ICommand command);
            
            Assert.True(success);
            Assert.AreEqual(command.GetType(), typeof(TestCommand1));
            
            success = _commandRegistry.TryGetCommand("test1.sub", out command);
            
            Assert.True(success);
            Assert.AreEqual(command.GetType(), typeof(Subcommand));
            
            success = _commandRegistry.TryGetCommand("test1.sub.subsub", out command);
            
            Assert.True(success);
            Assert.AreEqual(command.GetType(), typeof(SubSubcommand));
        }   
        
        [Test]
        public void CommandRegistry_TryGetCommandFailures()
        {
            bool success = _commandRegistry.TryGetCommand("Test1", out ICommand command);
            
            Assert.False(success);
            Assert.Null(command);
            
            success = _commandRegistry.TryGetCommand("sub", out command);
            
            Assert.False(success);
            Assert.Null(command);

            success = _commandRegistry.TryGetCommand("sub.subsub", out command);
            
            Assert.False(success);
            Assert.Null(command);
        }
        
        [Test]
        public void CommandRegistry_GetDescription()
        {
            string description = _commandRegistry.GetDescription("test1");
            Assert.AreEqual(description, CommandMetaProcessor.Description("Test command 1", typeof(Subcommand)));
            
            description = _commandRegistry.GetDescription("test1.sub");
            Assert.AreEqual(description, CommandMetaProcessor.Description("Subcommand of test1", typeof(Subcommand)));
            
            description = _commandRegistry.GetDescription("test1.sub.subsub");
            Assert.AreEqual(description, CommandMetaProcessor.Description("subcommand of subcommand", typeof(SubSubcommand)));
            
            description = _commandRegistry.GetDescription("test2");
            Assert.AreEqual(description, "");
        }
    }
}