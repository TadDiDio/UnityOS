using System.Linq;
using DeveloperConsole.Command;
using NUnit.Framework;
using UnityEngine;

namespace DeveloperConsole.Tests.Command
{
    // TODO: Test max recursion depth once configurable
    public class CommandRegistryTest
    {
        [Test]
        public void CommandRegistry_ConstructorEntriesValid()
        {
            ICommandRegistry registry = new CommandRegistry(
                new MockCommandDiscovery(TestUtilities.CommandRegistryValidTypes));
            
            Assert.AreEqual(registry.SchemaTable.Count, 4);

            var test1Entry = registry.SchemaTable.Values.FirstOrDefault(schema => schema.Name == CommandMetaProcessor.Name("test1"));
            var test2Entry = registry.SchemaTable.Values.FirstOrDefault(schema => schema.Name == CommandMetaProcessor.Name("test2"));
            var subOfTest1Entry = registry.SchemaTable.Values.FirstOrDefault(schema => schema.Name == CommandMetaProcessor.Name("suboftest1"));
            var subOfSubOfTest1Entry = registry.SchemaTable.Values.FirstOrDefault(schema => schema.Name == CommandMetaProcessor.Name("subofsuboftest1"));
            
            Assert.NotNull(test1Entry);
            Assert.NotNull(test2Entry);
            Assert.NotNull(subOfTest1Entry);
            Assert.NotNull(subOfSubOfTest1Entry);
            
            Assert.AreEqual(test1Entry.Subcommands.Count, 1);
            Assert.AreEqual(test2Entry.Subcommands.Count, 0);
            Assert.AreEqual(subOfTest1Entry.Subcommands.Count, 1);
            Assert.AreEqual(subOfSubOfTest1Entry.Subcommands.Count, 0);

            Assert.AreEqual(test1Entry.Subcommands.Single().Name, CommandMetaProcessor.Name("suboftest1"));
            Assert.AreEqual(subOfTest1Entry.Subcommands.Single().Name, CommandMetaProcessor.Name("subofsuboftest1"));
        }
        
        [Test]
        public void CommandRegistry_ConstructorCyclicalReferences()
        {
            using SilentLogCapture log = new SilentLogCapture();
            
            ICommandRegistry registry = new CommandRegistry(
                new MockCommandDiscovery(TestUtilities.CommandRegistryCyclicalTypes));
            
            Assert.AreEqual(registry.SchemaTable.Count, 0);
            Assert.AreEqual(log.Count(LogType.Error), 2);
            Assert.True(log.HasLog(LogType.Error, "hierarchy at 'cyclicalsub1.cyclicalsub2'"));
            Assert.True(log.HasLog(LogType.Error, "hierarchy at 'cyclicalsub2.cyclicalsub1'"));
        }
        
        [Test]
        public void CommandRegistry_ConstructorSelfCyclical()
        {
            using SilentLogCapture log = new SilentLogCapture();
            
            ICommandRegistry registry = new CommandRegistry(
                new MockCommandDiscovery(TestUtilities.CommandRegistrySelfCyclicalTypes));
            
            Assert.AreEqual(registry.SchemaTable.Count, 0);
            Assert.AreEqual(log.Count(LogType.Error), 1);
            Assert.True(log.HasLog(LogType.Error, "hierarchy at 'selfcyclical'"));
        }
        
        [Test]
        public void CommandRegistry_ConstructorBadType()
        {
            using SilentLogCapture log = new SilentLogCapture();
            
            ICommandRegistry registry = new CommandRegistry(
                new MockCommandDiscovery(TestUtilities.CommandRegistryBadParent));
            
            Assert.AreEqual(registry.SchemaTable.Count, 0);
            Assert.AreEqual(log.Count(LogType.Error), 1);
            Assert.True(log.HasLog(LogType.Error, "is missing CommandAttribute"));
        }
        
        [Test]
        public void CommandRegistry_ConstructorNoParent()
        {
            using SilentLogCapture log = new SilentLogCapture();
            
            ICommandRegistry registry = new CommandRegistry(
                new MockCommandDiscovery(TestUtilities.CommandRegistryNoParent));
            
            Assert.AreEqual(registry.SchemaTable.Count, 0);
            Assert.AreEqual(log.Count(LogType.Error), 1);
            Assert.True(log.HasLog(LogType.Error, "not found for subcommand"));
        }
    }
}