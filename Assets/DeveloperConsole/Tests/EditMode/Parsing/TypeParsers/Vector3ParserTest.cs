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
            
            List<string> tokens = new() { "-1", "0", "10" };
            TokenStream stream = new(tokens);

            var result = parser.TryParseStream(stream);
            Assert.True(result.Success);
            
            Assert.AreEqual(new Vector3(-1, 0, 10), result.Value);
            Assert.False(stream.HasMore());
        }
        
        [Test]
        public void Vector3Parser_NotParseable1()
        {
            var parser = new Vector3Parser();

            List<string> tokens = new() { null, "1", "one" };
            TokenStream stream = new(tokens);

            Assert.False(parser.TryParseStream(stream).Success);
        }

        [Test]
        public void Vector3Parser_NotParseable2()
        {
            var parser = new Vector3Parser();

            List<string> tokens = new() { "3", "1" };
            TokenStream stream = new(tokens);

            Assert.False(parser.TryParseStream(stream).Success);
        }
    }
}