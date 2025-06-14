using System.Collections.Generic;
using System.Linq;
using DeveloperConsole.Parsing;
using DeveloperConsole.Parsing.Tokenizing;
using NUnit.Framework;
using UnityEngine;

namespace DeveloperConsole.Tests.Parsing
{
    public class Vector2ParserTest
    {
        [Test]
        public void Vector2Parser_Parseable()
        {
            var parser = new Vector2Parser();

            List<string> tokens = new() { "-1", "0", "10", "-123123", "-843927.8", "43.23"};
            TokenStream stream = new(tokens);

            Assert.True(parser.TryParse(stream, out object x));
            Assert.AreEqual(new Vector2(-1, 0), x);
            Assert.AreEqual(stream.Remaining().Count(), 4);
            
            Assert.True(parser.TryParse(stream, out object y));
            Assert.AreEqual(new Vector2(10, -123123), y);
            Assert.AreEqual(stream.Remaining().Count(), 2);
            
            Assert.True(parser.TryParse(stream, out object z));
            Assert.AreEqual(new Vector2(-843927.8f, 43.23f), z);
            Assert.False(stream.HasMore());
        }
        
        [Test]
        public void Vector2Parser_NotParseable1()
        {
            var parser = new Vector2Parser();

            List<string> tokens = new() { null, "1" };
            TokenStream stream = new(tokens);

            Assert.False(parser.TryParse(stream, out _));
        }
        
        [Test]
        public void Vector2Parser_NotParseable2()
        {
            var parser = new Vector2Parser();

            List<string> tokens = new() { "1" };
            TokenStream stream = new(tokens);

            Assert.False(parser.TryParse(stream, out _));
        }
        
        [Test]
        public void Vector2Parser_NotParseable3()
        {
            var parser = new Vector2Parser();

            List<string> tokens = new() { "1" };
            TokenStream stream = new(tokens);

            Assert.False(parser.TryParse(stream, out _));
        }
    }
}