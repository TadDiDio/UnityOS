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
        public void Vector2Parser_Parseable1()
        {
            var parser = new Vector2Parser();

            List<string> tokens = new() { "-1", "0" };
            TokenStream stream = new(tokens);

            var result = parser.TryParseStream(stream);
            Assert.True(result.Success);
            Assert.AreEqual(new Vector2(-1, 0), result.Value);
            Assert.False(stream.HasMore());
        }
        
        
        [Test]
        public void Vector2Parser_NotParseable1()
        {
            var parser = new Vector2Parser();

            List<string> tokens = new() { null, "1" };
            TokenStream stream = new(tokens);

            Assert.False(parser.TryParseStream(stream).Success);
        }
        
        [Test]
        public void Vector2Parser_NotParseable2()
        {
            var parser = new Vector2Parser();

            List<string> tokens = new() { "1" };
            TokenStream stream = new(tokens);

            Assert.False(parser.TryParseStream(stream).Success);
        }
        
        [Test]
        public void Vector2Parser_NotParseable3()
        {
            var parser = new Vector2Parser();

            List<string> tokens = new() { "1" };
            TokenStream stream = new(tokens);

            Assert.False(parser.TryParseStream(stream).Success);
        }
    }
}