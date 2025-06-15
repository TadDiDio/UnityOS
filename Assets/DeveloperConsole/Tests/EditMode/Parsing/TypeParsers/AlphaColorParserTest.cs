using System.Collections.Generic;
using System.Linq;
using DeveloperConsole.Parsing;
using DeveloperConsole.Parsing.Tokenizing;
using NUnit.Framework;
using UnityEngine;

namespace DeveloperConsole.Tests.Parsing
{
    public class AlphaColorParserTest
    {
        [Test]
        public void AlphaColorParser_Parseable()
        {
            var parser = new AlphaColorParser();
            
            List<string> tokens = new() { "0", ".5", "1", "1"};
            TokenStream stream = new(tokens);

            var result = parser.TryParseStream(stream);
            
            Assert.True(result.Success);
            Assert.AreEqual(new Color(0, 0.5f, 1), result.Value);
            Assert.IsFalse(stream.HasMore());
        }
        
        [Test]
        public void AlphaColorParser_NotParseable1()
        {
            var parser = new AlphaColorParser();

            List<string> tokens = new() { null, "1", "1", "1"};
            TokenStream stream = new(tokens);

            var result = parser.TryParseStream(stream);
            Assert.False(result.Success);
        }
        
        [Test]
        public void AlphaColorParser_NotParseable2()
        {
            var parser = new AlphaColorParser();

            List<string> tokens = new() { "0", "1", "1", "1.1" };
            TokenStream stream = new(tokens);

            var result = parser.TryParseStream(stream);
            Assert.False(result.Success);
        }
        
        [Test]
        public void AlphaColorParser_NotParseable3()
        {
            var parser = new AlphaColorParser();

            List<string> tokens = new() { "0,", "1", "1", "-1" };
            TokenStream stream = new(tokens);

            var result = parser.TryParseStream(stream);
            Assert.False(result.Success);
        }
        
        [Test]
        public void AlphaColorParser_NotParseable4()
        {
            var parser = new AlphaColorParser();

            List<string> tokens = new() { "0", "1", "1", "a" };
            TokenStream stream = new(tokens);

            var result = parser.TryParseStream(stream);
            Assert.False(result.Success);
        }
        
        [Test]
        public void AlphaColorParser_NotParseable5()
        {
            var parser = new AlphaColorParser();

            List<string> tokens = new() { "0", "1", "1" };
            TokenStream stream = new(tokens);

            var result = parser.TryParseStream(stream);
            Assert.False(result.Success);
        }
    }
}