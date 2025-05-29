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

            protected override string Name() => "positionaltest";

            protected override string Description() => "positionaltest";

            public override Task<CommandResult> ExecuteAsync(CommandArgsBase args)
            {
                throw new System.NotImplementedException();
            }
        }
        
        [ExcludeFromCmdRegistry]
        private class SwitchTest : CommandBase
        {
            [SwitchArg("option1", 'o')]
            public float floatingPoint;
            
            [SwitchArg("message", 'm')]
            public string message;
            
            protected override string Name() => "switchtest";
            protected override string Description() => "switchtest";

            public override Task<CommandResult> ExecuteAsync(CommandArgsBase args)
            {
                throw new System.NotImplementedException();
            }
        }
        
        [ExcludeFromCmdRegistry]
        private class SwitchAndPositionalTest : CommandBase
        {
            [PositionalArg(0)]
            public Vector3 vector;
            
            [SwitchArg("option1", 'o')]
            public float floatingPoint;
            
            [SwitchArg("message", 'm')]
            public string message = "default message";
            
            protected override string Name() => "switchandpositionaltest";
            protected override string Description() => "switchandpositionaltest";

            public override Task<CommandResult> ExecuteAsync(CommandArgsBase args)
            {
                throw new System.NotImplementedException();
            }
        }
        
        [ExcludeFromCmdRegistry]
        private class VariadicTest : CommandBase
        {
            [VariadicArgs] public List<float> args;
            protected override string Name() => "variadictest";
            protected override string Description() => "variadictest";

            public override Task<CommandResult> ExecuteAsync(CommandArgsBase args)
            {
                throw new System.NotImplementedException();
            }
        }
        
        [ExcludeFromCmdRegistry]
        private class VariadicBadContainerTest : CommandBase
        {
            [VariadicArgs] public float[] args;
            protected override string Name() => "variadicbadcontainertest";
            protected override string Description() => "variadicbadcontainertest";

            public override Task<CommandResult> ExecuteAsync(CommandArgsBase args)
            {
                throw new System.NotImplementedException();
            }
        }
        
        [ExcludeFromCmdRegistry]
        private class SwitchAndPositionalAndVariadicTest : CommandBase
        {
            [PositionalArg(0)] public float number;
            
            [SwitchArg("option", 'o')]
            public float option;
            
            [SwitchArg("message", 'm')]
            public string message;

            [VariadicArgs] 
            public List<Vector2> vectors;
            protected override string Name() => "switchandpositionalandvariadictest";
            protected override string Description() => "switchandpositionalandvariadictest";

            public override Task<CommandResult> ExecuteAsync(CommandArgsBase args)
            {
                throw new System.NotImplementedException();
            }
        }
        #endregion
        
        [Test]
        public void ArgumentParserTest_PositionalArgsPassing()
        {
            PositionalTest command = new();
            
            List<string> tokens = new()
            {
                "positionaltest",
                "1.5",
                "1",
                "true",
                "stringy"
            };

            ReflectionParser reflectionParser = new(command.GetType());
            ArgumentParser parser = new(command, tokens, reflectionParser);
            
            var result = parser.Parse();
            
            Assert.True(result.Success);
            Assert.That(command.floatingPoint, Is.EqualTo(1.5f).Within(0.0001f));
            Assert.AreEqual(command.integer, 1);
            Assert.AreEqual(command.boolean, true);
            Assert.AreEqual(command.str, "stringy");
        }
        
        [Test]
        public void ArgumentParserTest_PositionalArgsMissing()
        {
            PositionalTest command = new();
            
            List<string> tokens = new()
            {
                "positionaltest",
                "1.5",
                "1",
                "true",
            };

            ReflectionParser reflectionParser = new(command.GetType());
            ArgumentParser parser = new(command, tokens, reflectionParser);
            
            var result = parser.Parse();
            
            Assert.False(result.Success);
            Assert.AreEqual(result.Error, ArgumentParseError.MissingPositionalArg);
            Assert.AreEqual(result.ErroneousField.Name, "str");
        }
        
        [Test]
        public void ArgumentParserTest_PositionalArgsTooMany()
        {
            PositionalTest command = new();
            
            List<string> tokens = new()
            {
                "positionaltest",
                "1.5",
                "1",
                "true",
                "stringy",
                "uh oh"
            };

            ReflectionParser reflectionParser = new(command.GetType());
            ArgumentParser parser = new(command, tokens, reflectionParser);
            
            var result = parser.Parse();
            
            Assert.False(result.Success);
            Assert.AreEqual(result.Error, ArgumentParseError.UnexpectedToken);
            Assert.AreEqual(result.ErroneousToken, "uh oh");
        }
        
        [Test]
        public void ArgumentParserTest_SwitchesPassingNoSwitchesAdded()
        {
            SwitchTest command = new();
            
            List<string> tokens = new()
            {
                "switchtest",
            };

            ReflectionParser reflectionParser = new(command.GetType());
            ArgumentParser parser = new(command, tokens, reflectionParser);
            
            var result = parser.Parse();
            
            Assert.True(result.Success);
        }
        
        [Test]
        public void ArgumentParserTest_SwitchesPassingSomeSwitchesAdded()
        {
            SwitchTest command = new();
            
            List<string> tokens = new()
            {
                "switchtest",
                "-o",
                "1.5",
            };

            ReflectionParser reflectionParser = new(command.GetType());
            ArgumentParser parser = new(command, tokens, reflectionParser);
            
            var result = parser.Parse();
            
            Assert.True(result.Success);
            Assert.That(command.floatingPoint, Is.EqualTo(1.5f).Within(0.0001f));
        }
        
        [Test]
        public void ArgumentParserTest_SwitchesPassingAllSwitchesAdded()
        {
            SwitchTest command = new();
            
            List<string> tokens = new()
            {
                "switchtest",
                "--message",
                "this is a test",
                "-o",
                "1.5",
            };

            ReflectionParser reflectionParser = new(command.GetType());
            ArgumentParser parser = new(command, tokens, reflectionParser);
            
            var result = parser.Parse();
            
            Assert.True(result.Success);
            Assert.That(command.floatingPoint, Is.EqualTo(1.5f).Within(0.0001f));
            Assert.AreEqual(command.message, "this is a test");
        }
        
        [Test]
        public void ArgumentParserTest_SwitchesBadType()
        {
            SwitchTest command = new();
            
            List<string> tokens = new()
            {
                "switchtest",
                "--message",
                "this is a test",
                "-o",
                "true",
            };

            ReflectionParser reflectionParser = new(command.GetType());
            ArgumentParser parser = new(command, tokens, reflectionParser);
            
            var result = parser.Parse();
            
            Assert.False(result.Success);
            Assert.AreEqual(result.Error, ArgumentParseError.TypeParseFailed);
            Assert.AreEqual(result.ErroneousField.Name, "floatingPoint");
        }
        
        [Test]
        public void ArgumentParserTest_SwitchesAndPositionals()
        {
            SwitchAndPositionalTest command = new();
            
            List<string> tokens = new()
            {
                "switchandpositionaltest",
                "-5",
                "1",
                "10",
                "-o",
                "1.5"
            };

            ReflectionParser reflectionParser = new(command.GetType());
            ArgumentParser parser = new(command, tokens, reflectionParser);
            
            var result = parser.Parse();
            
            Assert.True(result.Success);
            Assert.AreEqual(command.vector, new Vector3(-5, 1f, 10));
            Assert.That(command.floatingPoint, Is.EqualTo(1.5f).Within(0.0001f));
            Assert.AreEqual(command.message, "default message");
        }

        [Test]
        public void ArgumentParserTest_SwitchesAndPositionalsBadOrder()
        {
            SwitchAndPositionalTest command = new();
            
            List<string> tokens = new()
            {
                "switchandpositionaltest",
                "-o",
                "1.5",
                "-5",
                "0.4",
                "10"
            };

            ReflectionParser reflectionParser = new(command.GetType());
            ArgumentParser parser = new(command, tokens, reflectionParser);
            
            var result = parser.Parse();
            
            Assert.False(result.Success);
            Assert.AreEqual(result.Error, ArgumentParseError.TypeParseFailed);
            Assert.AreEqual(result.ErroneousToken, "-o, 1.5, -5");
            Assert.AreEqual(result.ErroneousField.Name, "vector");
        }
        
        [Test]
        public void ArgumentParserTest_VariadicPassing()
        {
            VariadicTest command = new();
            
            List<string> tokens = new()
            {
                "variadictest",
                "1.5",
                "-5",
                "0.4",
                "10"
            };

            ReflectionParser reflectionParser = new(command.GetType());
            ArgumentParser parser = new(command, tokens, reflectionParser);
            
            var result = parser.Parse();
            
            Assert.True(result.Success);
            Assert.That(command.args[0], Is.EqualTo(1.5).Within(0.0001f));
            Assert.That(command.args[1], Is.EqualTo(-5).Within(0.0001f));
            Assert.That(command.args[2], Is.EqualTo(0.4).Within(0.0001f));
            Assert.That(command.args[3], Is.EqualTo(10).Within(0.0001f));
        }
        
        [Test]
        public void ArgumentParserTest_VariadicBadType()
        {
            VariadicTest command = new();
            
            List<string> tokens = new()
            {
                "variadictest",
                "1.5",
                "-5",
                "0.4",
                "10",
                "true"
            };

            ReflectionParser reflectionParser = new(command.GetType());
            ArgumentParser parser = new(command, tokens, reflectionParser);
            
            var result = parser.Parse();
            
            Assert.False(result.Success);
            Assert.AreEqual(result.Error, ArgumentParseError.TypeParseFailed);
            Assert.AreEqual(result.ErroneousToken, "true");
            Assert.AreEqual(result.ErroneousField.Name, "args");
        }
        
        [Test]
        public void ArgumentParserTest_VariadicBadContainer()
        {
            VariadicBadContainerTest command = new();
            
            List<string> tokens = new()
            {
                "variadicbadcontainertest",
                "1.5",
                "-5",
                "0.4",
                "10",
                "5"
            };

            ReflectionParser reflectionParser = new(command.GetType());
            ArgumentParser parser = new(command, tokens, reflectionParser);
            
            var result = parser.Parse();
            
            Assert.False(result.Success);
            Assert.AreEqual(result.Error, ArgumentParseError.BadVariadicContainer);
        }
        
        [Test]
        public void ArgumentParserTest_SwitchesAndPositionalsAndVariadicPassing()
        {
            SwitchAndPositionalAndVariadicTest command = new();
            
            List<string> tokens = new()
            {
                "switchandpositionalandvariadictest",
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

            ReflectionParser reflectionParser = new(command.GetType());
            ArgumentParser parser = new(command, tokens, reflectionParser);
            
            var result = parser.Parse();
            Assert.True(result.Success);
            Assert.That(command.number, Is.EqualTo(4.23).Within(0.0001f));
            Assert.AreEqual(command.option, -5);
            Assert.AreEqual(command.message, "message");
            Assert.AreEqual(command.vectors.Count, 2);
            Assert.AreEqual(command.vectors[0], new Vector2(76, 1));
            Assert.AreEqual(command.vectors[1], new Vector2(5, 3));
        }
        
        [Test]
        public void ArgumentParserTest_SwitchesAndPositionalsAndVariadicBadParsing()
        {
            SwitchAndPositionalAndVariadicTest command = new();
            
            List<string> tokens = new()
            {
                "switchandpositionalandvariadictest",
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

            ReflectionParser reflectionParser = new(command.GetType());
            ArgumentParser parser = new(command, tokens, reflectionParser);
            
            var result = parser.Parse();
            Assert.False(result.Success);
            Assert.AreEqual(result.Error, ArgumentParseError.TypeParseFailed);
            Assert.AreEqual(result.ErroneousToken, "5");
        }
    }
}


