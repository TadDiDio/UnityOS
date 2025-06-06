using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;

namespace DeveloperConsole.Tests
{
    public class ArgumentParserTest
    {
        #region TEST TYPES
        [ExcludeFromCmdRegistry]
        [Command("positionaltest", "", true)]
        private class PositionalTest : CommandBase
        {
            [PositionalArg(0)]
            public float floatingPoint;
            
            [PositionalArg(1)]
            public int integer;
            
            [PositionalArg(3)]
            public string str;
            
            [PositionalArg(2)]
            public bool boolean;

            public override Task<CommandResult> ExecuteAsync(CommandContext context)
            {
                throw new System.NotImplementedException();
            }
        }
        
        [ExcludeFromCmdRegistry]
        [Command("switchtest", "", true)]
        private class SwitchTest : CommandBase
        {
            [SwitchArg("option1", 'o')]
            public float floatingPoint;
            
            [SwitchArg("message", 'm')]
            public string message;
            
            public override Task<CommandResult> ExecuteAsync(CommandContext context)
            {
                throw new System.NotImplementedException();
            }
        }
        [ExcludeFromCmdRegistry]
        [Command("switchtestrequired", "", true)]
        private class SwitchTestRequired : CommandBase
        {
            [SwitchArg("option", 'o')] [RequiredArg]
            public float option;
            
            [SwitchArg("message", 'm')] [RequiredArg]
            public string message;
            
            public override Task<CommandResult> ExecuteAsync(CommandContext context)
            {
                throw new System.NotImplementedException();
            }
        }
        
        [ExcludeFromCmdRegistry]
        [Command("switchandpositionaltest", "", true)]
        private class SwitchAndPositionalTest : CommandBase
        {
            [PositionalArg(0)]
            public Vector3 vector;
            
            [SwitchArg("option1", 'o')]
            public float floatingPoint;
            
            [SwitchArg("message", 'm')]
            public string message = "default message";
            
            public override Task<CommandResult> ExecuteAsync(CommandContext context)
            {
                throw new System.NotImplementedException();
            }
        }
        
        [ExcludeFromCmdRegistry]
        [Command("variadictest", "", true)]
        private class VariadicTest : CommandBase
        {
            [VariadicArgs] public List<float> args;
            public override Task<CommandResult> ExecuteAsync(CommandContext context)
            {
                throw new System.NotImplementedException();
            }
        }
        
        [ExcludeFromCmdRegistry]
        [Command("variadicbadcontainertest", "", true)]
        private class VariadicBadContainerTest : CommandBase
        {
            [VariadicArgs] public float[] args;
            public override Task<CommandResult> ExecuteAsync(CommandContext context)
            {
                throw new System.NotImplementedException();
            }
        }
        
        [ExcludeFromCmdRegistry]
        [Command("switchandpositionalandvariadictest", "", true)]
        private class SwitchAndPositionalAndVariadicTest : CommandBase
        {
            [PositionalArg(0)] public float number;
            
            [SwitchArg("option", 'o')]
            public float option;
            
            [SwitchArg("message", 'm')]
            public string message;

            [VariadicArgs] 
            public List<Vector2> vectors;
            public override Task<CommandResult> ExecuteAsync(CommandContext context)
            {
                throw new System.NotImplementedException();
            }
        }
        
        [ExcludeFromCmdRegistry]
        [Command("boolcoalescing", "", true)]
        private class BoolCoalescing : CommandBase
        {
            [SwitchArg("option", 'o')]
            public bool option;
            
            [SwitchArg("flag", 'f')]
            public bool flag;

            [SwitchArg("number", 'n')]
            public float number;
            public override Task<CommandResult> ExecuteAsync(CommandContext context)
            {
                throw new System.NotImplementedException();
            }
        }
        #endregion
        
        [Test]
        public async Task ArgumentParserTest_PositionalArgsPassing()
        {
            PositionalTest command = new();
            
            List<string> tokens = new()
            {
                "1.5",
                "1",
                "true",
                "stringy"
            };

            ReflectionParser reflectionParser = new(command);
            ArgumentParser parser = new(command, new TokenStream(tokens), reflectionParser);

            var result = await parser.ParseAsync();
            
            Assert.True(result.Success);
            Assert.That(command.floatingPoint, Is.EqualTo(1.5f).Within(0.0001f));
            Assert.AreEqual(command.integer, 1);
            Assert.AreEqual(command.boolean, true);
            Assert.AreEqual(command.str, "stringy");
        }
        
        [Test]
        public async Task ArgumentParserTest_PositionalArgsMissing()
        {
            PositionalTest command = new();
            
            List<string> tokens = new()
            {
                "1.5",
                "1",
                "true",
            };

            ReflectionParser reflectionParser = new(command);
            ArgumentParser parser = new(command, new TokenStream(tokens), reflectionParser);

            var result = await parser.ParseAsync();
            
            Assert.False(result.Success);
            Assert.AreEqual(result.Error, ArgumentParseError.MissingPositionalArg);
            Assert.AreEqual(result.ErroneousField.Name, "str");
        }
        
        [Test]
        public async Task ArgumentParserTest_PositionalArgsTooMany()
        {
            PositionalTest command = new();
            
            List<string> tokens = new()
            {
                "1.5",
                "1",
                "true",
                "stringy",
                "uh oh"
            };

            ReflectionParser reflectionParser = new(command);
            ArgumentParser parser = new(command, new TokenStream(tokens), reflectionParser);
            
            var result = await parser.ParseAsync();
            
            Assert.False(result.Success);
            Assert.AreEqual(result.Error, ArgumentParseError.UnexpectedToken);
            Assert.AreEqual(result.ErroneousToken, "uh oh");
        }
        
        [Test]
        public async Task ArgumentParserTest_SwitchesPassingNoSwitchesAdded()
        {
            SwitchTest command = new();
            
            List<string> tokens = new()
            {
            };

            ReflectionParser reflectionParser = new(command);
            ArgumentParser parser = new(command, new TokenStream(tokens), reflectionParser);
            
            var result = await parser.ParseAsync();
            
            Assert.True(result.Success);
        }
        
        [Test]
        public async Task ArgumentParserTest_SwitchesRequiredButNone()
        {
            SwitchTestRequired command = new();
            
            List<string> tokens = new()
            {
            };

            ReflectionParser reflectionParser = new(command);
            ArgumentParser parser = new(command, new TokenStream(tokens), reflectionParser);
            
            var result = await parser.ParseAsync();
            
            Assert.False(result.Success);
            Assert.AreEqual(result.Error, ArgumentParseError.AttributeValidationError);
            Assert.AreEqual(result.ErroneousField.Name, "option");
            Assert.AreEqual(result.ErroneousAttribute.GetType(), typeof(RequiredArgAttribute));
        }
        [Test]
        public async Task ArgumentParserTest_SwitchesRequiredButMissingOne()
        {
            SwitchTestRequired command = new();
            
            List<string> tokens = new()
            {
                "-o",
                "1.5",
            };

            ReflectionParser reflectionParser = new(command);
            ArgumentParser parser = new(command, new TokenStream(tokens), reflectionParser);
            
            var result = await parser.ParseAsync();
            
            Assert.False(result.Success);
            Assert.AreEqual(result.Error, ArgumentParseError.AttributeValidationError);
            Assert.AreEqual(result.ErroneousField.Name, "message");
            Assert.AreEqual(result.ErroneousAttribute.GetType(), typeof(RequiredArgAttribute));
        }
        [Test]
        public async Task ArgumentParserTest_SwitchesRequiredAndGiven()
        {
            SwitchTestRequired command = new();
            
            List<string> tokens = new()
            {
                "-o",
                "1.5",
                "-m",
                "message"
            };

            ReflectionParser reflectionParser = new(command);
            ArgumentParser parser = new(command, new TokenStream(tokens), reflectionParser);
            
            var result = await parser.ParseAsync();
            
            Assert.True(result.Success);
        }
        
        [Test]
        public async Task ArgumentParserTest_UnrecognizedSwitch()
        {
            SwitchTest command = new();
            
            List<string> tokens = new()
            {
                "-a",
                "1.5",
            };

            ReflectionParser reflectionParser = new(command);
            ArgumentParser parser = new(command, new TokenStream(tokens), reflectionParser);
            
            var result = await parser.ParseAsync();
            
            Assert.False(result.Success);
            Assert.AreEqual(result.ErroneousToken, "-a");
        }
        
        [Test]
        public async Task ArgumentParserTest_DuplicateSwitch()
        {
            SwitchTest command = new();
            
            List<string> tokens = new()
            {
                "-o",
                "1.5",
                "-o",
                "5"
            };

            ReflectionParser reflectionParser = new(command);
            ArgumentParser parser = new(command, new TokenStream(tokens), reflectionParser);
            
            var result = await parser.ParseAsync();
            
            Assert.False(result.Success);
            Assert.AreEqual(result.ErroneousToken, "-o");
            Assert.AreEqual(result.ErroneousField.Name, "floatingPoint");
            Assert.AreEqual(result.ErroneousAttribute.GetType(), typeof(SwitchArgAttribute));
        }
        
        [Test]
        public async Task ArgumentParserTest_SwitchesPassingSomeSwitchesAdded()
        {
            SwitchTest command = new();
            
            List<string> tokens = new()
            {
                "-o",
                "1.5",
            };

            ReflectionParser reflectionParser = new(command);
            ArgumentParser parser = new(command, new TokenStream(tokens), reflectionParser);
            
            var result = await parser.ParseAsync();
            
            Assert.True(result.Success);
            Assert.That(command.floatingPoint, Is.EqualTo(1.5f).Within(0.0001f));
        }
        
        [Test]
        public async Task ArgumentParserTest_SwitchesPassingAllSwitchesAdded()
        {
            SwitchTest command = new();
            
            List<string> tokens = new()
            {
                "--message",
                "this is a test",
                "-o",
                "1.5",
            };

            ReflectionParser reflectionParser = new(command);
            ArgumentParser parser = new(command, new TokenStream(tokens), reflectionParser);
            
            var result = await parser.ParseAsync();
            
            Assert.True(result.Success);
            Assert.That(command.floatingPoint, Is.EqualTo(1.5f).Within(0.0001f));
            Assert.AreEqual(command.message, "this is a test");
        }
        
        [Test]
        public async Task ArgumentParserTest_SwitchesBadType()
        {
            SwitchTest command = new();
            
            List<string> tokens = new()
            {
                "--message",
                "this is a test",
                "-o",
                "true",
            };

            ReflectionParser reflectionParser = new(command);
            ArgumentParser parser = new(command, new TokenStream(tokens), reflectionParser);
            
            var result = await parser.ParseAsync();
            
            Assert.False(result.Success);
            Assert.AreEqual(result.Error, ArgumentParseError.TypeParseFailed);
            Assert.AreEqual(result.ErroneousField.Name, "floatingPoint");
        }
        
        [Test]
        public async Task ArgumentParserTest_SwitchesAndPositionals()
        {
            SwitchAndPositionalTest command = new();
            
            List<string> tokens = new()
            {
                "-5",
                "1",
                "10",
                "-o",
                "1.5"
            };

            ReflectionParser reflectionParser = new(command);
            ArgumentParser parser = new(command, new TokenStream(tokens), reflectionParser);
            
            var result = await parser.ParseAsync();
            
            Assert.True(result.Success);
            Assert.AreEqual(command.vector, new Vector3(-5, 1f, 10));
            Assert.That(command.floatingPoint, Is.EqualTo(1.5f).Within(0.0001f));
            Assert.AreEqual(command.message, "default message");
        }

        [Test]
        public async Task ArgumentParserTest_SwitchesAndPositionalsBadOrder()
        {
            SwitchAndPositionalTest command = new();
            
            List<string> tokens = new()
            {
                "-o",
                "1.5",
                "-5",
                "0.4",
                "10"
            };

            ReflectionParser reflectionParser = new(command);
            ArgumentParser parser = new(command, new TokenStream(tokens), reflectionParser);
            
            var result = await parser.ParseAsync();
            
            Assert.False(result.Success);
            Assert.AreEqual(result.Error, ArgumentParseError.TypeParseFailed);
            Assert.AreEqual(result.ErroneousToken, "-o, 1.5, -5");
            Assert.AreEqual(result.ErroneousField.Name, "vector");
        }
        
        [Test]
        public async Task ArgumentParserTest_VariadicPassing()
        {
            VariadicTest command = new();
            
            List<string> tokens = new()
            {
                "1.5",
                "-5",
                "0.4",
                "10"
            };

            ReflectionParser reflectionParser = new(command);
            ArgumentParser parser = new(command, new TokenStream(tokens), reflectionParser);
            
            var result = await parser.ParseAsync();
            
            Assert.True(result.Success);
            Assert.That(command.args[0], Is.EqualTo(1.5).Within(0.0001f));
            Assert.That(command.args[1], Is.EqualTo(-5).Within(0.0001f));
            Assert.That(command.args[2], Is.EqualTo(0.4).Within(0.0001f));
            Assert.That(command.args[3], Is.EqualTo(10).Within(0.0001f));
        }
        
        [Test]
        public async Task ArgumentParserTest_VariadicBadType()
        {
            VariadicTest command = new();
            
            List<string> tokens = new()
            {
                "1.5",
                "-5",
                "0.4",
                "10",
                "true"
            };

            ReflectionParser reflectionParser = new(command);
            ArgumentParser parser = new(command, new TokenStream(tokens), reflectionParser);
            
            var result = await parser.ParseAsync();
            
            Assert.False(result.Success);
            Assert.AreEqual(result.Error, ArgumentParseError.TypeParseFailed);
            Assert.AreEqual(result.ErroneousToken, "true");
            Assert.AreEqual(result.ErroneousField.Name, "args");
        }
        
        [Test]
        public async Task ArgumentParserTest_VariadicBadContainer()
        {
            VariadicBadContainerTest command = new();
            
            List<string> tokens = new()
            {
                "1.5",
                "-5",
                "0.4",
                "10",
                "5"
            };

            ReflectionParser reflectionParser = new(command);
            ArgumentParser parser = new(command, new TokenStream(tokens), reflectionParser);
            
            var result = await parser.ParseAsync();
            
            Assert.False(result.Success);
            Assert.AreEqual(result.Error, ArgumentParseError.BadVariadicContainer);
        }
        
        [Test]
        public async Task ArgumentParserTest_SwitchesAndPositionalsAndVariadicPassing()
        {
            SwitchAndPositionalAndVariadicTest command = new();
            
            List<string> tokens = new()
            {
                "4.23",
                "-o",
                "-5",
                "-m",
                "message",
                "76",
                "1",
                "5",
                "3"
            };

            ReflectionParser reflectionParser = new(command);
            ArgumentParser parser = new(command, new TokenStream(tokens), reflectionParser);
            
            var result = await parser.ParseAsync();
            Assert.True(result.Success);
            Assert.That(command.number, Is.EqualTo(4.23).Within(0.0001f));
            Assert.AreEqual(command.option, -5);
            Assert.AreEqual(command.message, "message");
            Assert.AreEqual(command.vectors.Count, 2);
            Assert.AreEqual(command.vectors[0], new Vector2(76, 1));
            Assert.AreEqual(command.vectors[1], new Vector2(5, 3));
        }
        
        [Test]
        public async Task ArgumentParserTest_SwitchesAndPositionalsAndVariadicBadParsing()
        {
            SwitchAndPositionalAndVariadicTest command = new();
            
            List<string> tokens = new()
            {
                "4.23",
                "-o",
                "-5",
                "-m",
                "message",
                "76",
                "1",
                "5",
                "3",
                "5"
            };

            ReflectionParser reflectionParser = new(command);
            ArgumentParser parser = new(command, new TokenStream(tokens), reflectionParser);
            
            var result = await parser.ParseAsync();
            Assert.False(result.Success);
            Assert.AreEqual(result.Error, ArgumentParseError.TypeParseFailed);
            Assert.AreEqual(result.ErroneousToken, "5");
        }
        
        [Test]
        public async Task ArgumentParserTest_BoolCoalescingOnly()
        {
            BoolCoalescing command = new();
            
            List<string> tokens = new()
            {
                "-of",
            };

            ReflectionParser reflectionParser = new(command);
            ArgumentParser parser = new(command, new TokenStream(tokens), reflectionParser);
            
            var result = await parser.ParseAsync();
            Assert.True(result.Success);
            Assert.True(command.option);
            Assert.True(command.flag);
        }
        
        [Test]
        public async Task ArgumentParserTest_BoolCoalescingWithBefore()
        {
            BoolCoalescing command = new();
            
            List<string> tokens = new()
            {
                "-n",
                "1",
                "-of",
            };

            ReflectionParser reflectionParser = new(command);
            ArgumentParser parser = new(command, new TokenStream(tokens), reflectionParser);
            
            var result = await parser.ParseAsync();
            
            Assert.True(result.Success);    
            Assert.True(command.option);
            Assert.True(command.flag);
            Assert.That(command.number, Is.EqualTo(1).Within(0.0001f));
        }
        
        [Test]
        public async Task ArgumentParserTest_BoolCoalescingWithAfter()
        {
            BoolCoalescing command = new();
            
            List<string> tokens = new()
            {
                "-of",
                "-n",
                "1",
            };

            ReflectionParser reflectionParser = new(command);
            ArgumentParser parser = new(command, new TokenStream(tokens), reflectionParser);
            
            var result = await parser.ParseAsync();
            Assert.True(result.Success);
            Assert.True(command.option);
            Assert.True(command.flag);
            Assert.That(command.number, Is.EqualTo(1).Within(0.0001f));
        }
        
        [Test]
        public async Task ArgumentParserTest_BoolCoalescingNonBool()
        {
            BoolCoalescing command = new();
            
            List<string> tokens = new()
            {
                "-on",
            };

            ReflectionParser reflectionParser = new(command);
            ArgumentParser parser = new(command, new TokenStream(tokens), reflectionParser);
            
            var result = await parser.ParseAsync();
            Assert.False(result.Success);
            Assert.AreEqual(result.Error, ArgumentParseError.NonBoolCoalescing);
        }
        
        [Test]
        public async Task ArgumentParserTest_BoolCoalescingNonBoolWithBool()
        {
            BoolCoalescing command = new();
            
            List<string> tokens = new()
            {
                "-ofn",
            };

            ReflectionParser reflectionParser = new(command);
            ArgumentParser parser = new(command, new TokenStream(tokens), reflectionParser);
            
            var result = await parser.ParseAsync();
            Assert.False(result.Success);
            Assert.AreEqual(result.Error, ArgumentParseError.NonBoolCoalescing);
        }
        
        [Test]
        public async Task ArgumentParserTest_BoolCoalescingUnrecognized()
        {
            BoolCoalescing command = new();
            
            List<string> tokens = new()
            {
                "-oa",
            };

            ReflectionParser reflectionParser = new(command);
            ArgumentParser parser = new(command, new TokenStream(tokens), reflectionParser);
            
            var result = await parser.ParseAsync();
            Assert.False(result.Success);
            Assert.AreEqual(result.Error, ArgumentParseError.UnrecognizedSwitch);
        }
    }
}


