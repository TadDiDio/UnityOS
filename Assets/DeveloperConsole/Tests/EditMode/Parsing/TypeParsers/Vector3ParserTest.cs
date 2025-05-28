using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace DeveloperConsole.Tests
{
    public class Vector3ParserTest
    {
        [Test]
        public void Vector3Parser_Parseable()
        {
            var parser = new Vector3Parser();
            
            Assert.AreEqual(parser.TokenCount(), 3);

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
        public void Vector3Parser_NotParseable()
        {
            var parser = new Vector3Parser();

            List<string> tokens = new() { null, "1", "one", "3", "1"};
            TokenStream stream = new(tokens);

            Assert.False(parser.TryParse(stream, out object x));
            Assert.AreEqual(stream.Remaining().Count(), 2);
            
            Assert.False(parser.TryParse(stream, out object y));
            Assert.False(stream.HasMore());
        }
    }
}