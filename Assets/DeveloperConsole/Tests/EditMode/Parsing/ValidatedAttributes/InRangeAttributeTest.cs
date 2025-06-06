using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using NUnit.Framework;

namespace DeveloperConsole.Tests.ValidatedAttributes
{
    public class InRangeAttributeTest
    {
        #region TEST TYPES

        [ExcludeFromCmdRegistry]
        [Command("test", "test", true)]
        private class TestCommand : SimpleCommand
        {
            [InRange(0, 10)] [PositionalArg(0)] public int num;
            protected override CommandResult Execute(CommandContext context)
            {
                throw new System.NotImplementedException();
            }
        }
        
        #endregion
        
        [Test]
        public async Task InRangeAttribute_Test()
        {
            var map = new Dictionary<string, Type> { { "test", typeof(TestCommand) } };
            CommandRegistry registry = new CommandRegistry(map);
            CommandParser parser = new CommandParser(registry);
            
            List<string> tokens = new() { "test", "10" };
            TokenStream stream = new(tokens);

            var result = await parser.Parse(stream);
            var field = result.Command.GetType().GetField("num");
            var attribute = field.GetCustomAttribute<InRangeAttribute>();
            Assert.NotNull(attribute);
            AttributeValidationData data = new()
            {
                FieldInfo = field,
                Object = result.Command
            };
            Assert.True(attribute.Validate(data));
            
            tokens = new() { "test", "0" };
            stream = new(tokens);
            result = await parser.Parse(stream);
            field = result.Command.GetType().GetField("num");
            attribute = field.GetCustomAttribute<InRangeAttribute>();
            Assert.NotNull(attribute);
            data = new()
            {
                FieldInfo = field,
                Object = result.Command
            };
            Assert.True(attribute.Validate(data));
            
            tokens = new() { "test", "-1" };
            stream = new(tokens);
            result = await parser.Parse(stream);
            Assert.Null(result.Command);
            
            tokens = new() { "test", "11" };
            stream = new(tokens);
            result = await parser.Parse(stream);
            Assert.Null(result.Command);
        }
    }
}