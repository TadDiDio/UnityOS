using NUnit.Framework;
using System.Collections.Generic;
using DeveloperConsole.Parsing;
using DeveloperConsole.Parsing.Tokenizing;

namespace DeveloperConsole.Tests.Parsing
{
    public class IntParserTest
    {
        [Test]
        public void IntParser_NotParseable1()
        {
            var parser = new IntParser();

            List<string> tokens = new() { null };
            TokenStream stream = new(tokens);

            var result = parser.TryParseStream(stream);
            Assert.False(result.Success);
        }
        
        [Test]
        public void IntParser_NotParseable2()
        {
            var parser = new IntParser();

            List<string> tokens = new() { "" };
            TokenStream stream = new(tokens);

            var result = parser.TryParseStream(stream);
            Assert.False(result.Success);
        }
        
        [Test]
        public void IntParser_NotParseable3()
        {
            var parser = new IntParser();

            List<string> tokens = new() { "asd" };
            TokenStream stream = new(tokens);

            var result = parser.TryParseStream(stream);
            Assert.False(result.Success);
        }
        
        [Test]
        public void IntParser_NotParseable4()
        {
            var parser = new IntParser();

            List<string> tokens = new() { "-123.0" };
            TokenStream stream = new(tokens);

            var result = parser.TryParseStream(stream);
            Assert.False(result.Success);
        }
        
        [Test]
        public void IntParser_NotParseable5()
        {
            var parser = new IntParser();

            List<string> tokens = new() { "123.0" };
            TokenStream stream = new(tokens);

            var result = parser.TryParseStream(stream);
            Assert.False(result.Success);
        }
        
        [Test]
        public void IntParser_NotParseable6()
        {
            var parser = new IntParser();

            List<string> tokens = new() { "10f" };
            TokenStream stream = new(tokens);

            var result = parser.TryParseStream(stream);
            Assert.False(result.Success);
        }
        
        [Test]
        public void IntParser_NotParseable7()
        {
            var parser = new IntParser();

            List<string> tokens = new() { "1.5" };
            TokenStream stream = new(tokens);

            var result = parser.TryParseStream(stream);
            Assert.False(result.Success);
        }
        
        [Test]
        public void IntParser_Parseable()
        {
            var parser = new IntParser();

            List<string> tokens = new() { "-1", "0", "1", "-123123", "8439278" };
            TokenStream stream = new(tokens);

            int count = 0;
            while (stream.HasMore())
            {
                var result = parser.TryParseStream(stream);
                Assert.True(result.Success);
                Assert.AreEqual(int.Parse(tokens[count++]), result.Value);
            }
        }
    }
}
