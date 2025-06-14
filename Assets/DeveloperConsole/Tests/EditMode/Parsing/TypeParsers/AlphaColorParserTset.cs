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
            
            List<string> tokens = new() { "0", ".5", "1", "1", "0.1", ".4325643", "0.0001", "1"};
            TokenStream stream = new(tokens);

            Assert.True(parser.TryParse(stream, out object x));
            Assert.AreEqual(new Color(0, 0.5f, 1), x);
            Assert.AreEqual(stream.Remaining().Count(), 4);
            
            Assert.True(parser.TryParse(stream, out object y));
            Assert.AreEqual(new Color(0.1f, .4325643f, 0.0001f), y);
            Assert.False(stream.HasMore());
        }
        
        [Test]
        public void AlphaColorParser_NotParseable1()
        {
            var parser = new AlphaColorParser();

            List<string> tokens = new() { null, "1", "1", "1"};
            TokenStream stream = new(tokens);

            Assert.False(parser.TryParse(stream, out _));
        }
        
        [Test]
        public void AlphaColorParser_NotParseable2()
        {
            var parser = new AlphaColorParser();

            List<string> tokens = new() { "0", "1", "1", "1.1" };
            TokenStream stream = new(tokens);

            Assert.False(parser.TryParse(stream, out _));
        }
        
        [Test]
        public void AlphaColorParser_NotParseable3()
        {
            var parser = new AlphaColorParser();

            List<string> tokens = new() { "0,", "1", "1", "-1" };
            TokenStream stream = new(tokens);

            Assert.False(parser.TryParse(stream, out _));
        }
        
        [Test]
        public void AlphaColorParser_NotParseable4()
        {
            var parser = new AlphaColorParser();

            List<string> tokens = new() { "0", "1", "1", "a" };
            TokenStream stream = new(tokens);

            Assert.False(parser.TryParse(stream, out _));
        }
        
        [Test]
        public void AlphaColorParser_NotParseable5()
        {
            var parser = new AlphaColorParser();

            List<string> tokens = new() { "0", "1", "1" };
            TokenStream stream = new(tokens);

            Assert.False(parser.TryParse(stream, out _));
        }
    }
}