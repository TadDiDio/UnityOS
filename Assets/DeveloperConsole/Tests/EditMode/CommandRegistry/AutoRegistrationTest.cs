using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace DeveloperConsole.Tests
{
    public class AutoRegistrationTest
    {
        #region TEST TYPES
        [ExcludeFromCmdRegistry]
        [Command("base1", "Base command 1", true)]
        private class Base1 : SimpleCommand
        {
            [Subcommand] private Subcommand subcommand;
            protected override CommandResult Execute(CommandContext context)
            {
                throw new NotImplementedException();
            }
        }

        [ExcludeFromCmdRegistry]
        [Command("base2", "Base command 2", true)]
        private class Base2 : SimpleCommand
        {
            [Subcommand] private SubSubcommand subsubcommand;
            protected override CommandResult Execute(CommandContext context)
            {
                throw new NotImplementedException();
            }
        }
        
        [ExcludeFromCmdRegistry]
        [Command("sub", "Subcommand of base1", false)]
        private class Subcommand : SimpleCommand
        {
            [Subcommand] private SubSubcommand subcommand = new();
            protected override CommandResult Execute(CommandContext context)
            {
                throw new NotImplementedException();
            }
        }
        
        [ExcludeFromCmdRegistry]
        [Command("subsub", "subcommand of subcommand", false)]
        private class SubSubcommand : SimpleCommand
        {
            protected override CommandResult Execute(CommandContext context)
            {
                throw new NotImplementedException();
            }
        }
        
        [ExcludeFromCmdRegistry]
        [Command("selfcircularbase", "self circular base command", true)]
        private class SelfCircularBaseCommand : SimpleCommand
        {
            [Subcommand] private SelfCircularBaseCommand subcommand;
            protected override CommandResult Execute(CommandContext context)
            {
                throw new NotImplementedException();
            }
        }
        
        [ExcludeFromCmdRegistry]
        [Command("seldcircularsub", "self circular sub command", false)]
        private class SelfCircularSubCommand : SimpleCommand
        {
            [Subcommand] private SelfCircularSubCommand subcommand;
            protected override CommandResult Execute(CommandContext context)
            {
                throw new NotImplementedException();
            }
        }
        
        [ExcludeFromCmdRegistry]
        [Command("circularbase", "circular base command", true)]
        private class CircularBaseCommand : SimpleCommand
        {
            [Subcommand] private CircularSubCommand subcommand;
            protected override CommandResult Execute(CommandContext context)
            {
                throw new NotImplementedException();
            }
        }
        
        [ExcludeFromCmdRegistry]
        [Command("circularsub", "circular sub command", false)]
        private class CircularSubCommand : SimpleCommand
        {
            [Subcommand] private CircularBaseCommand subcommand;
            protected override CommandResult Execute(CommandContext context)
            {
                throw new NotImplementedException();
            }
        }
        
        [ExcludeFromCmdRegistry]
        [Command("invalidsubcommandcommand", "Invalid command", true)]
        private class InvalidSubcommandCommand : SimpleCommand
        {
            [Subcommand] private NonCommand test;
            protected override CommandResult Execute(CommandContext context)
            {
                throw new NotImplementedException();
            }
        }

        [ExcludeFromCmdRegistry]
        [Command("", "No name command", true)]
        private class NoNameCommand : SimpleCommand
        {
            protected override CommandResult Execute(CommandContext context)
            {
                throw new NotImplementedException();
            }
        }
        
        [ExcludeFromCmdRegistry]
        [Command("named", "named command with unnamed sub", true)]
        private class NamedCommand : SimpleCommand
        {
            [Subcommand] NoNameSubCommand subcommand;
            protected override CommandResult Execute(CommandContext context)
            {
                throw new NotImplementedException();
            }
        }
        
        [ExcludeFromCmdRegistry]
        [Command("", "No name sub command", false)]
        private class NoNameSubCommand : SimpleCommand
        {
            protected override CommandResult Execute(CommandContext context)
            {
                throw new NotImplementedException();
            }
        }
        
        private class NonCommand : SimpleCommand
        {
            protected override CommandResult Execute(CommandContext context)
            {
                throw new NotImplementedException();
            }
        }
        
        #endregion

        private IAutoRegistrationProvider _autoRegistration;
        private List<(Type, CommandAttribute)> _baseCommands = new();
        
        [SetUp]
        public void SetUp()
        {
            _baseCommands.Clear();
            _autoRegistration = new AutoRegistration();
        }
        
        [Test]
        public void AutoRegistrationTest_Passes()
        {
            _baseCommands.Add((typeof(Base1), new CommandAttribute("base1", "Base command 1", false)));
            _baseCommands.Add((typeof(Base2), new CommandAttribute("base2", "Base command 2", false)));
            
            var result = _autoRegistration.AllCommands(new MockAutoRegistration(_baseCommands));
            
            Assert.AreEqual(result.Count, 5);
            Assert.True(result.Any(kvp => kvp.Key == "base1"));
            Assert.True(result.Any(kvp => kvp.Key == "base1.sub"));
            Assert.True(result.Any(kvp => kvp.Key == "base1.sub.subsub"));
            Assert.True(result.Any(kvp => kvp.Key == "base2"));
            Assert.True(result.Any(kvp => kvp.Key == "base2.subsub"));
        }
        
        [Test]
        public void AutoRegistrationTest_Depth0Exceeded()
        {
            _baseCommands.Add((typeof(Base1), new CommandAttribute("base1", "Base command 1", false)));
            _baseCommands.Add((typeof(Base2), new CommandAttribute("base2", "Base command 2", false)));
            
            using var logCatcher = new SilentLogCapture();
            
            var result = _autoRegistration.AllCommands(new MockAutoRegistration(_baseCommands), 0);
            
            Assert.AreEqual(result.Count, 2);
            Assert.True(result.Any(kvp => kvp.Key == "base1"));
            Assert.True(result.Any(kvp => kvp.Key == "base2"));
            
            Assert.AreEqual(logCatcher.Count(LogType.Warning), 2);
            Assert.True(logCatcher.HasLog(LogType.Warning, "depth at base1"));
            Assert.True(logCatcher.HasLog(LogType.Warning, "depth at base2"));
        }
        
        [Test]
        public void AutoRegistrationTest_Depth1Exceeded()
        {
            _baseCommands.Add((typeof(Base1), new CommandAttribute("base1", "Base command 1", false)));
            _baseCommands.Add((typeof(Base2), new CommandAttribute("base2", "Base command 2", false)));
            
            using var logCatcher = new SilentLogCapture();
         
            var result = _autoRegistration.AllCommands(new MockAutoRegistration(_baseCommands), 1);
            
            Assert.AreEqual(result.Count, 4);
            Assert.True(result.Any(kvp => kvp.Key == "base1"));
            Assert.True(result.Any(kvp => kvp.Key == "base1.sub"));
            Assert.True(result.Any(kvp => kvp.Key == "base2"));
            Assert.True(result.Any(kvp => kvp.Key == "base2.subsub"));
            
            Assert.AreEqual(logCatcher.Count(LogType.Warning), 1);
            Assert.True(logCatcher.HasLog(LogType.Warning, "depth at base1.sub.subsub"));
        }
        
        [Test]
        public void AutoRegistrationTest_SelfCircularReferences()
        {
            _baseCommands.Add((typeof(SelfCircularBaseCommand), new CommandAttribute("selfcircularbase", "circular command", false)));
            _baseCommands.Add((typeof(SelfCircularSubCommand), new CommandAttribute("selfcircularsub", "circular command", false)));
            
            using var logCatcher = new SilentLogCapture();
         
            var result = _autoRegistration.AllCommands(new MockAutoRegistration(_baseCommands));
            
            Assert.AreEqual(result.Count, 2);
            Assert.True(result.Any(kvp => kvp.Key == "selfcircularbase"));
            Assert.True(result.Any(kvp => kvp.Key == "selfcircularsub"));
            
            Assert.AreEqual(logCatcher.Count(LogType.Warning), 2);
            Assert.True(logCatcher.HasLog(LogType.Warning, "SelfCircularBaseCommand -> SelfCircularBaseCommand"));
            Assert.True(logCatcher.HasLog(LogType.Warning, "SelfCircularSubCommand -> SelfCircularSubCommand"));
        }
        
        [Test]
        public void AutoRegistrationTest_CircularReferences()
        {
            _baseCommands.Add((typeof(CircularBaseCommand), new CommandAttribute("circularbase", "circular command", false)));
            
            using var logCatcher = new SilentLogCapture();
         
            var result = _autoRegistration.AllCommands(new MockAutoRegistration(_baseCommands));
            
            Assert.AreEqual(result.Count, 2);
            Assert.True(result.Any(kvp => kvp.Key == "circularbase"));
            Assert.True(result.Any(kvp => kvp.Key == "circularbase.circularsub"));
            
            Assert.AreEqual(logCatcher.Count(LogType.Warning), 1);
            Assert.True(logCatcher.HasLog(LogType.Warning, "CircularSubCommand -> CircularBaseCommand"));
        }
        
        [Test]
        public void AutoRegistrationTest_NoNameCommand()
        {
            _baseCommands.Add((typeof(NoNameCommand), new CommandAttribute("", "no name command", false)));
            
            using var logCatcher = new SilentLogCapture();
         
            var result = _autoRegistration.AllCommands(new MockAutoRegistration(_baseCommands));
            
            Assert.AreEqual(result.Count, 0);
            
            Assert.AreEqual(logCatcher.Count(LogType.Error), 1);
            Assert.True(logCatcher.HasLog(LogType.Error, $"Command name for NoNameCommand is null or empty. This is not allowed " +
                                                           $"and the command will not be available."));
        }
        
        [Test]
        public void AutoRegistrationTest_NoNameSubCommand()
        {
            _baseCommands.Add((typeof(NamedCommand), new CommandAttribute("named", "no name sub command", false)));
            
            using var logCatcher = new SilentLogCapture();
         
            var result = _autoRegistration.AllCommands(new MockAutoRegistration(_baseCommands));
            
            Assert.AreEqual(result.Count, 1);
            Assert.True(result.Any(kvp => kvp.Key == "named"));
            
            Assert.AreEqual(logCatcher.Count(LogType.Error), 1);
            Assert.True(logCatcher.HasLog(LogType.Error, $"Command name for NoNameSubCommand is null or empty. This is not allowed " +
                                                         $"and the command will not be available."));
        }
        
        [Test]
        public void AutoRegistrationTest_InvalidSubcommandTag()
        {
            _baseCommands.Add((typeof(InvalidSubcommandCommand), new CommandAttribute("invalidsubcommandcommand", "invalid command", false)));
            
            using var logCatcher = new SilentLogCapture();
         
            var result = _autoRegistration.AllCommands(new MockAutoRegistration(_baseCommands));
            
            Assert.AreEqual(result.Count, 1);
            Assert.True(result.Any(kvp => kvp.Key == "invalidsubcommandcommand"));
            
            Assert.AreEqual(logCatcher.Count(LogType.Warning), 1);
            Assert.True(logCatcher.HasLog(LogType.Warning, "'test' on 'InvalidSubcommandCommand' does not reference a type with CommandAttribute"));
        }
    }
}