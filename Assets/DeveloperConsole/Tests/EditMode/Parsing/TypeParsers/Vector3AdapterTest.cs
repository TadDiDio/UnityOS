using System.Collections.Generic;
using DeveloperConsole.Parsing.Tokenizing;
using DeveloperConsole.Parsing.TypeAdapting;
using NUnit.Framework;
using UnityEngine;

namespace DeveloperConsole.Tests.Parsing
{
    public class Vector3AdapterTest
    {
        [Test]
        public void Vector3Parser_Parseable()
        {
            var adapter = new Vector3Adapter();

            List<string> tokens = new() { "-1", "0", "10" };
            TokenStream stream = new(tokens);

            var result = adapter.AdaptFromTokens(stream);
            Assert.True(result.Success);

            Assert.AreEqual(new Vector3(-1, 0, 10), result.Value);
            Assert.False(stream.HasMore());
        }

        [Test]
        public void Vector3Parser_NotParseable1()
        {
            var adapter = new Vector3Adapter();

            List<string> tokens = new() { null, "1", "one" };
            TokenStream stream = new(tokens);

            Assert.False(adapter.AdaptFromTokens(stream).Success);
        }

        [Test]
        public void Vector3Parser_NotParseable2()
        {
            var adapter = new Vector3Adapter();

            List<string> tokens = new() { "3", "1" };
            TokenStream stream = new(tokens);

            Assert.False(adapter.AdaptFromTokens(stream).Success);
        }
    }
}
