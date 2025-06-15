using System.Collections.Generic;
using DeveloperConsole.Parsing;
using DeveloperConsole.Parsing.Tokenizing;
using NUnit.Framework;

namespace DeveloperConsole.Tests.Parsing
{
    public class FloatParserTest
    {
        [Test]
        public void FloatParser_NotParseable1()
        {
            var parser = new FloatParser();

            List<string> tokens = new() { null };
            TokenStream stream = new(tokens);

            var result = parser.TryParseStream(stream);    
            Assert.False(result.Success);
        }
        
        [Test]
        public void FloatParser_NotParseable2()
        {
            var parser = new FloatParser();

            List<string> tokens = new() { "" };
            TokenStream stream = new(tokens);

            var result = parser.TryParseStream(stream);    
            Assert.False(result.Success);
        }
    
        [Test]
        public void FloatParser_NotParseable3()
        {
            var parser = new FloatParser();

            List<string> tokens = new() { "asd" };
            TokenStream stream = new(tokens);

            var result = parser.TryParseStream(stream);    
            Assert.False(result.Success);
        }
        
        [Test]
        public void FloatParser_NotParseable4()
        {
            var parser = new FloatParser();

            List<string> tokens = new() { "-123d" };
            TokenStream stream = new(tokens);

            var result = parser.TryParseStream(stream);    
            Assert.False(result.Success);
        }
        
        [Test]
        public void FloatParser_NotParseable5()
        {
            var parser = new FloatParser();

            List<string> tokens = new() { "123f" };
            TokenStream stream = new(tokens);

            var result = parser.TryParseStream(stream);    
            Assert.False(result.Success);
        }
        
        [Test]
        public void FloatParser_NotParseable6()
        {
            var parser = new FloatParser();

            List<string> tokens = new() { "10d" };
            TokenStream stream = new(tokens);

            var result = parser.TryParseStream(stream);    
            Assert.False(result.Success);
        }
        
        
        [Test]
        public void FloatParser_Parseable()
        {
            var parser = new FloatParser();

            List<string> tokens = new() { "-1", "0", "1", "-123123", "8439278", "324.432", ".004324", "-.0432"};
            TokenStream stream = new(tokens);

            int count = 0;
            while (stream.HasMore())
            {
                var result = parser.TryParseStream(stream);
                Assert.True(result.Success);
                Assert.AreEqual(float.Parse(tokens[count++]), result.Value);
            }
        }
    }
}