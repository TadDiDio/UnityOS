using System.Collections.Generic;
using DeveloperConsole.Parsing;
using DeveloperConsole.Parsing.Tokenizing;
using NUnit.Framework;
using UnityEngine;

namespace DeveloperConsole.Tests.Parsing
{
    public class ColorParserTest
    {
        [Test]
        public void ColorParser_Parseable()
        {
            var parser = new ColorParser();
            
            List<string> tokens = new() { "0", ".5", "1"};
            TokenStream stream = new(tokens);

            var result = parser.TryParseStream(stream);
            Assert.True(result.Success);
            Assert.AreEqual(new Color(0, 0.5f, 1), result.Value);
            Assert.False(stream.HasMore());
        }
        
        [Test]
        public void ColorParser_NotParseable1()
        {
            var parser = new ColorParser();

            List<string> tokens = new() { null, "1", "1"};
            TokenStream stream = new(tokens);

            var result = parser.TryParseStream(stream);
            Assert.False(result.Success);
        }
        
        [Test]
        public void ColorParser_NotParseable2()
        {
            var parser = new ColorParser();

            List<string> tokens = new() { "1", "1", "1.1" };
            TokenStream stream = new(tokens);

            var result = parser.TryParseStream(stream);
            Assert.False(result.Success);
        }
        
        [Test]
        public void ColorParser_NotParseable3()
        {
            var parser = new ColorParser();

            List<string> tokens = new() { "-1", "0", "0" };
            TokenStream stream = new(tokens);

            var result = parser.TryParseStream(stream);
            Assert.False(result.Success);
        }
        
        [Test]
        public void ColorParser_NotParseable4()
        {
            var parser = new ColorParser();

            List<string> tokens = new() { ".5", ".5" };
            TokenStream stream = new(tokens);

            var result = parser.TryParseStream(stream);
            Assert.False(result.Success);
        }
    }
}