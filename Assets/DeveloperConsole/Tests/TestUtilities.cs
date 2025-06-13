using System;
using System.Collections.Generic;
using DeveloperConsole.Command;

namespace DeveloperConsole.Tests
{
    public static class TestUtilities
    {
        public static readonly List<Type> CommandRegistryValidTypes = new()
        {
            typeof(TestCommand1),
            typeof(TestCommand2),
            typeof(SubOfTest1Command),
            typeof(SubOfSubOfTest1Command),
        };
        
        public static readonly List<Type> CommandRegistryCyclicalTypes = new()
        {
            typeof(CyclicalSub1Command),
            typeof(CyclicalSub2Command)
        };
        
        public static readonly List<Type> CommandRegistrySelfCyclicalTypes = new()
        {
            typeof(SelfCyclicalCommand)
        };
        
        public static readonly List<Type> CommandRegistryNoParent = new()
        {
            typeof(SubOfTest1Command)
        };
        
        public static readonly List<Type> CommandRegistryBadParent = new()
        {
            typeof(BadParentCommand)
        };
    }
    
    public class MockCommandDiscovery : ICommandDiscoveryStrategy
    {
        private List<Type> _info;
        public MockCommandDiscovery(List<Type> info)
        {
            _info = info;
        }
        public List<Type> GetAllCommandTypes() => _info;
    }
    
    [ExcludeFromCmdRegistry]
    [Command("test1", "test1 description")]
    public class TestCommand1 : SimpleCommand
    {
        protected override CommandOutput Execute(CommandContext context)
        {
            throw new NotImplementedException();
        }
    }

    [ExcludeFromCmdRegistry]
    [Command("Test2", "Test2 description")]
    public class TestCommand2 : SimpleCommand
    {
        protected override CommandOutput Execute(CommandContext context)
        {
            throw new NotImplementedException();
        }
    }
    
    [ExcludeFromCmdRegistry]
    [Subcommand("subOfTest1", "Subcommand of test1.", typeof(TestCommand1))]
    public class SubOfTest1Command : SimpleCommand
    {
        protected override CommandOutput Execute(CommandContext context)
        {
            throw new NotImplementedException();
        }
    }
    
    [ExcludeFromCmdRegistry]
    [Subcommand("subOfSubOfTest1", "Subcommand of subOfSubOfTest1.", typeof(SubOfTest1Command))]
    public class SubOfSubOfTest1Command : SimpleCommand
    {
        protected override CommandOutput Execute(CommandContext context)
        {
            throw new NotImplementedException();
        }
    }
    
    [ExcludeFromCmdRegistry]
    [Subcommand("cyclicalsub1", "sub1 of cyclical references", typeof(CyclicalSub2Command))]
    public class CyclicalSub1Command : SimpleCommand
    {
        protected override CommandOutput Execute(CommandContext context)
        {
            throw new NotImplementedException();
        }
    }
    
    [ExcludeFromCmdRegistry]
    [Subcommand("cyclicalsub2", "sub2 of cyclical references", typeof(CyclicalSub1Command))]
    public class CyclicalSub2Command : SimpleCommand
    {
        protected override CommandOutput Execute(CommandContext context)
        {
            throw new NotImplementedException();
        }
    }
    
    [ExcludeFromCmdRegistry]
    [Subcommand("selfcyclical", "self cyclical command", typeof(SelfCyclicalCommand))]
    public class SelfCyclicalCommand : SimpleCommand
    {
        protected override CommandOutput Execute(CommandContext context)
        {
            throw new NotImplementedException();
        }
    }
    
    [ExcludeFromCmdRegistry]
    [Subcommand("badparent", "bad parent.", typeof(int))]
    public class BadParentCommand : SimpleCommand
    {
        protected override CommandOutput Execute(CommandContext context)
        {
            throw new NotImplementedException();
        }
    }
}