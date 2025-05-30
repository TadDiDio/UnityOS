using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace DeveloperConsole.Tests
{
    public class ConsoleParserTest
    {
        #region TEST TYPES
        [ExcludeFromCmdRegistry]
        [Command("test1", "Test command 1")]
        private class TestCommand1 : SimpleCommand
        {
            [Subcommand] private Subcommand subcommand;
            protected override CommandResult Execute(CommandArgs args)
            {
                throw new NotImplementedException();
            }
        }

        [ExcludeFromCmdRegistry]
        [Command("sub", "Subcommand of test1", true)]
        private class Subcommand : SimpleCommand
        {
            [Subcommand] private SubSubcommand subcommand = new();
            protected override CommandResult Execute(CommandArgs args)
            {
                throw new NotImplementedException();
            }
        }
        
        [ExcludeFromCmdRegistry]
        [Command("subsub", "Subcommand of subcommand", true)]
        private class SubSubcommand : SimpleCommand
        {
            protected override CommandResult Execute(CommandArgs args)
            {
                throw new NotImplementedException();
            }
        }
        #endregion

        private IConsoleParser _consoleParser;
        private ICommandRegistryProvider _commandRegistry;
        
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
            _consoleParser = new ConsoleParser(_commandRegistry);
        }

        [Test]
        public void ConsoleParser_Passes()
        {
            List<string> tokens = new() { "test1" };
            var result = _consoleParser.Parse(new TokenStream(tokens));
            
            Assert.AreEqual(result.Error, ParseError.None);
            Assert.AreEqual(result.Command.GetType(), typeof(TestCommand1));
            Assert.AreEqual(result.CommandName, "test1");
            
            tokens = new() { "test1", "sub" };
            result = _consoleParser.Parse(new TokenStream(tokens));
            
            Assert.AreEqual(result.Error, ParseError.None);
            Assert.AreEqual(result.Command.GetType(), typeof(Subcommand));
            Assert.AreEqual(result.CommandName, "test1.sub");
            
            tokens = new() { "test1", "sub", "subsub"};
            result = _consoleParser.Parse(new TokenStream(tokens));
            
            Assert.AreEqual(result.Error, ParseError.None);
            Assert.AreEqual(result.Command.GetType(), typeof(SubSubcommand));
            Assert.AreEqual(result.CommandName, "test1.sub.subsub");
        }
        
        [Test]
        public void ConsoleParser_InvalidCommandName()
        {
            List<string> tokens = new() { "test2" };
            var result = _consoleParser.Parse(new TokenStream(tokens));
            
            Assert.AreEqual(result.Error, ParseError.InvalidCommandName);
        }
        
        [Test]
        public void ConsoleParser_ArgumentError()
        {
            List<string> tokens = new() { "test1", "test2" };
            var result = _consoleParser.Parse(new TokenStream(tokens));
            
            Assert.AreEqual(result.Error, ParseError.ArgumentError);
        }
    }
}