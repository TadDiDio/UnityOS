using System.Collections.Generic;
using System.Linq;
using DeveloperConsole.Parsing;
using DeveloperConsole.Parsing.Tokenizing;
using NUnit.Framework;
using UnityEngine;

namespace DeveloperConsole.Tests.Parsing
{
    public class Vector3ParserTest
    {
        [Test]
        public void Vector3Parser_Parseable()
        {
            var parser = new Vector3Parser();
            
            List<string> tokens = new() { "-1", "0", "10", "-123123", "-843927.8", "43.23"};
            TokenStream stream = new(tokens);

            Assert.True(parser.TryParse(stream, out object x));
            Assert.AreEqual(new Vector3(-1, 0, 10), x);
            Assert.AreEqual(stream.Remaining().Count(), 3);
            
            Assert.True(parser.TryParse(stream, out object y));
            Assert.AreEqual(new Vector3(-123123, -843927.8f, 43.23f), y);
            Assert.False(stream.HasMore());
        }
        
        [Test]
        public void Vector3Parser_NotParseable1()
        {
            var parser = new Vector3Parser();

            List<string> tokens = new() { null, "1", "one" };
            TokenStream stream = new(tokens);

            Assert.False(parser.TryParse(stream, out _));
        }
        
        [Test]
        public void Vector3Parser_NotParseable2()
        {
            var parser = new Vector3Parser();

            List<string> tokens = new() { "3", "1" };
            TokenStream stream = new(tokens);

            Assert.False(parser.TryParse(stream, out _));
        }
    }
}