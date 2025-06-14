using System.Collections.Generic;
using System.Linq;
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
            
            List<string> tokens = new() { "0", ".5", "0", "1", "0.1", ".4325643"};
            TokenStream stream = new(tokens);

            Assert.True(parser.TryParse(stream, out object x));
            Assert.AreEqual(new Color(0, 0.5f, 0), x);
            Assert.AreEqual(stream.Remaining().Count(), 3);
            
            Assert.True(parser.TryParse(stream, out object y));
            Assert.AreEqual(new Color(1, 0.1f, .4325643f), y);
            Assert.False(stream.HasMore());
        }
        
        [Test]
        public void ColorParser_NotParseable1()
        {
            var parser = new ColorParser();

            List<string> tokens = new() { null, "1", "1"};
            TokenStream stream = new(tokens);

            Assert.False(parser.TryParse(stream, out _));
        }
        
        [Test]
        public void ColorParser_NotParseable2()
        {
            var parser = new ColorParser();

            List<string> tokens = new() { "1", "1", "1.1" };
            TokenStream stream = new(tokens);

            Assert.False(parser.TryParse(stream, out _));
        }
        
        [Test]
        public void ColorParser_NotParseable3()
        {
            var parser = new ColorParser();

            List<string> tokens = new() { "-1", "0", "0" };
            TokenStream stream = new(tokens);

            Assert.False(parser.TryParse(stream, out _));
        }
        
        [Test]
        public void ColorParser_NotParseable4()
        {
            var parser = new ColorParser();

            List<string> tokens = new() { ".5", ".5" };
            TokenStream stream = new(tokens);

            Assert.False(parser.TryParse(stream, out _));
        }
    }
}