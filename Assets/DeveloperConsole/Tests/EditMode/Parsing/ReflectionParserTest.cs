using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace DeveloperConsole.Tests
{
    public class ReflectionParserTest
    {
        #region TEST TYPES
        [ExcludeFromCmdRegistry]
        [Command("variadicargsbadcontainer", "", false)]
        private class VariadicArgsBadContainerCommand : SimpleCommand
        {
            [VariadicArgs] private int[] variadic;
            protected override CommandResult Execute(CommandContext context)
            {
                throw new System.NotImplementedException();
            }
        }
        
        [ExcludeFromCmdRegistry]
        [Command("variadicargs", "", false)]
        private class VariadicArgsCommand : SimpleCommand
        {
            [VariadicArgs] private List<int> variadic;
            protected override CommandResult Execute(CommandContext context)
            {
                throw new System.NotImplementedException();
            }
        }

        [ExcludeFromCmdRegistry]
        [Command("subtest", "sub test command for recursion", true)]
        private class TestSubcommand : SimpleCommand
        {
            protected override CommandResult Execute(CommandContext context)
            {
                return new CommandResult("Ran the sub command");
            }
        }
        
        [ExcludeFromCmdRegistry]
        [Command("basetest", "base test command for recursion", false)]
        private class TestCommand : SimpleCommand
        {
            [PositionalArg(1)] private int one;
            [PositionalArg(0)] private int zero;
            
            [SwitchArg("test1", 't')] private bool test1;
            [SwitchArg("test2", 'd')] private bool test2;
            
            [Subcommand]
            private TestSubcommand subcommand;
            protected override CommandResult Execute(CommandContext context)
            {
                return new CommandResult("Ran the base command");
            }
        }

        #endregion

        ReflectionParser parser;
        [SetUp]
        public void SetUp()
        {
            ICommand testCommand = new TestCommand();
            parser = new ReflectionParser(testCommand);
        }
        
        
        [Test]
        public void ReflectionParser_HasSubcommand()
        {
            Assert.True(parser.HasSubcommandWithSimpleName("subtest"));
            Assert.True(parser.HasSubcommandWithSimpleName(" SUBTEST "));
        }
        
        [Test]
        public void ReflectionParser_Positionals()
        {
            var fields = parser.GetPositionalArgFieldsInOrder();
            Assert.AreEqual(fields[0].Name, "zero");
            Assert.AreEqual(fields[1].Name, "one");
        }

        [Test]
        public void ReflectionParser_SwitchesLongName()
        {
            var switch1 = parser.GetSwitchField("test1");
            Assert.NotNull(switch1);

            var (field, switchAttr) = switch1.Value;
            Assert.AreEqual(field.Name, "test1");
        }
        
        [Test]
        public void ReflectionParser_SwitchesShortName()
        {
            var switch2 = parser.GetSwitchField("-d");
            Assert.NotNull(switch2);

            var (field, switchAttr) = switch2.Value;
            Assert.AreEqual(field.Name, "test2");
            
            var switch2two = parser.GetSwitchField("-d");
            Assert.NotNull(switch2two);

            var (field2, switchAttr2) = switch2two.Value;
            Assert.AreEqual(field.Name, "test2");
        }
        
        [Test]
        public void ReflectionParser_SwitchesLongNameDashes()
        {
            var switch2 = parser.GetSwitchField("--test2");
            Assert.NotNull(switch2);

            var (field, switchAttr) = switch2.Value;
            Assert.AreEqual(field.Name, "test2");
            
            var switch2two = parser.GetSwitchField("-test2");
            Assert.NotNull(switch2two);

            var (field2, _) = switch2two.Value;
            Assert.AreEqual(field2.Name, "test2");
        }
        
        [Test]
        public void ReflectionParser_SwitchesShortNameDash()
        {
            var switch1 = parser.GetSwitchField("-t");
            Assert.NotNull(switch1);

            var (field, switchAttr) = switch1.Value;
            Assert.AreEqual(field.Name, "test1");
        }
        
        [Test]
        public void ReflectionParser_InvalidSwitch()
        {
            var switch1 = parser.GetSwitchField("none");
            Assert.Null(switch1);
            
            var switch2 = parser.GetSwitchField("--none");
            Assert.Null(switch2);
            
            var switch3 = parser.GetSwitchField("-n");
            Assert.Null(switch3);
        }

        [Test]
        public void ReflectionParser_GetAllFields()
        {
            var fields = parser.GetAllFields();
            Assert.AreEqual(fields.Count, 5);
            
            Assert.True(fields.Any(field => field.Name == "zero"));
            Assert.True(fields.Any(field => field.Name == "one"));
            Assert.True(fields.Any(field => field.Name == "test1"));
            Assert.True(fields.Any(field => field.Name == "test2"));
            Assert.True(fields.Any(field => field.Name == "subcommand"));
        }

        [Test]
        public void ReflectionParser_NoVariadicArgs()
        {
            var container = parser.GetVariadicArgsField(out bool _);
            Assert.Null(container);
        }
        
        [Test]
        public void ReflectionParser_VariadicArgs()
        {
            ICommand variadicArgs = new VariadicArgsCommand();
            parser = new ReflectionParser(variadicArgs);

            var container = parser.GetVariadicArgsField(out bool badContainer);
            Assert.NotNull(container);
            Assert.False(badContainer);
        }
        
        [Test]
        public void ReflectionParser_VariadicArgsBadContainer()
        {
            ICommand variadicArgs = new VariadicArgsBadContainerCommand();
            parser = new ReflectionParser(variadicArgs);
            
            var container = parser.GetVariadicArgsField(out bool badContainer);
            Assert.Null(container);
            Assert.True(badContainer);
        }
    }
}