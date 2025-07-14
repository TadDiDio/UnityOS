using System.Collections.Generic;
using DeveloperConsole.Parsing.Tokenizing;
using DeveloperConsole.Parsing.TypeAdapting;
using NUnit.Framework;
using UnityEngine;

namespace DeveloperConsole.Tests.Parsing
{
    public class ColorAdapterTest
    {
        [Test]
        public void ColorParser_Parseable()
        {
            var adapter = new ColorAdapter();

            List<string> tokens = new() { "0", ".5", "1"};
            TokenStream stream = new(tokens);

            var result = adapter.AdaptFromTokens(stream);
            Assert.True(result.Success);
            Assert.AreEqual(new Color(0, 0.5f, 1), result.Value);
            Assert.False(stream.HasMore());
        }

        [Test]
        public void ColorParser_NotParseable1()
        {
            var adapter = new ColorAdapter();

            List<string> tokens = new() { null, "1", "1"};
            TokenStream stream = new(tokens);

            var result = adapter.AdaptFromTokens(stream);
            Assert.False(result.Success);
        }

        [Test]
        public void ColorParser_NotParseable2()
        {
            var adapter = new ColorAdapter();

            List<string> tokens = new() { "1", "1", "1.1" };
            TokenStream stream = new(tokens);

            var result = adapter.AdaptFromTokens(stream);
            Assert.False(result.Success);
        }

        [Test]
        public void ColorParser_NotParseable3()
        {
            var adapter = new ColorAdapter();

            List<string> tokens = new() { "-1", "0", "0" };
            TokenStream stream = new(tokens);

            var result = adapter.AdaptFromTokens(stream);
            Assert.False(result.Success);
        }

        [Test]
        public void ColorParser_NotParseable4()
        {
            var adapter = new ColorAdapter();

            List<string> tokens = new() { ".5", ".5" };
            TokenStream stream = new(tokens);

            var result = adapter.AdaptFromTokens(stream);
            Assert.False(result.Success);
        }
    }
}
