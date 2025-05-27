using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace DeveloperConsole.Tests
{
    public class AlphaColorParserTest
    {
        [Test]
        public void AlphaColorParser_Parseable()
        {
            var parser = new AlphaColorParser();
            
            Assert.AreEqual(parser.TokenCount(), 4);

            List<string> tokens = new() { "0", ".5", "1", "1", "0.1", ".4325643", "0.0001", "1"};
            TokenStream stream = new(tokens);

            Assert.True(parser.TryParse(stream, out Color x));
            Assert.AreEqual(new Color(0, 0.5f, 1), x);
            Assert.AreEqual(stream.Remaining().Count(), 4);
            
            Assert.True(parser.TryParse(stream, out Color y));
            Assert.AreEqual(new Color(0.1f, .4325643f, 0.0001f), y);
            Assert.False(stream.HasMore());
        }
        
        [Test]
        public void AlphaColorParser_NotParseable()
        {
            var parser = new AlphaColorParser();

            List<string> tokens = new() { null, "1", "1", "1", "0", "1", "1", "1.1", "0,", "1", "1", "-1", "0", "1", "1", "a", "1", "1", "1"};
            TokenStream stream = new(tokens);

            Assert.False(parser.TryParse(stream, out _));
            Assert.AreEqual(stream.Remaining().Count(), 15);
            
            Assert.False(parser.TryParse(stream, out _));
            Assert.AreEqual(stream.Remaining().Count(), 11);
            
            Assert.False(parser.TryParse(stream, out _));
            Assert.AreEqual(stream.Remaining().Count(), 7);
            
            Assert.False(parser.TryParse(stream, out _));
            Assert.AreEqual(stream.Remaining().Count(), 3);
            
            Assert.False(parser.TryParse(stream, out _));
            Assert.False(stream.HasMore());
        }
    }
}