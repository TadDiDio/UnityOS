using System.Collections.Generic;
using DeveloperConsole.Parsing.Tokenizing;
using DeveloperConsole.Parsing.TypeAdapting;
using NUnit.Framework;
using UnityEngine;

namespace DeveloperConsole.Tests.Parsing
{
    public class Vector2AdapterTest
    {
        [Test]
        public void Vector2Parser_Parseable1()
        {
            var adapter = new Vector2Adapter();

            List<string> tokens = new() { "-1", "0" };
            TokenStream stream = new(tokens);

            var result = adapter.AdaptFromTokens(stream);
            Assert.True(result.Success);
            Assert.AreEqual(new Vector2(-1, 0), result.Value);
            Assert.False(stream.HasMore());
        }


        [Test]
        public void Vector2Parser_NotParseable1()
        {
            var adapter = new Vector2Adapter();

            List<string> tokens = new() { null, "1" };
            TokenStream stream = new(tokens);

            Assert.False(adapter.AdaptFromTokens(stream).Success);
        }

        [Test]
        public void Vector2Parser_NotParseable2()
        {
            var adapter = new Vector2Adapter();

            List<string> tokens = new() { "1" };
            TokenStream stream = new(tokens);

            Assert.False(adapter.AdaptFromTokens(stream).Success);
        }

        [Test]
        public void Vector2Parser_NotParseable3()
        {
            var adapter = new Vector2Adapter();

            List<string> tokens = new() { "1" };
            TokenStream stream = new(tokens);

            Assert.False(adapter.AdaptFromTokens(stream).Success);
        }
    }
}
