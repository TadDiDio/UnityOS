using System.Collections.Generic;
using DeveloperConsole.Parsing.Tokenizing;
using DeveloperConsole.Parsing.TypeAdapting;
using NUnit.Framework;
using UnityEngine;

namespace DeveloperConsole.Tests.Parsing
{
    public class AlphaColorAdapterTest
    {
        [Test]
        public void AlphaColorParser_Parseable()
        {
            var adapter = new AlphaColorAdapter();

            List<string> tokens = new() { "0", ".5", "1", "1"};
            TokenStream stream = new(tokens);

            var result = adapter.AdaptFromTokens(stream);

            Assert.True(result.Success);
            Assert.AreEqual(new Color(0, 0.5f, 1), result.Value);
            Assert.IsFalse(stream.HasMore());
        }

        [Test]
        public void AlphaColorParser_NotParseable1()
        {
            var adapter = new AlphaColorAdapter();

            List<string> tokens = new() { null, "1", "1", "1"};
            TokenStream stream = new(tokens);

            var result = adapter.AdaptFromTokens(stream);
            Assert.False(result.Success);
        }

        [Test]
        public void AlphaColorParser_NotParseable2()
        {
            var adapter = new AlphaColorAdapter();

            List<string> tokens = new() { "0", "1", "1", "1.1" };
            TokenStream stream = new(tokens);

            var result = adapter.AdaptFromTokens(stream);
            Assert.False(result.Success);
        }

        [Test]
        public void AlphaColorParser_NotParseable3()
        {
            var adapter = new AlphaColorAdapter();

            List<string> tokens = new() { "0,", "1", "1", "-1" };
            TokenStream stream = new(tokens);

            var result = adapter.AdaptFromTokens(stream);
            Assert.False(result.Success);
        }

        [Test]
        public void AlphaColorParser_NotParseable4()
        {
            var adapter = new AlphaColorAdapter();

            List<string> tokens = new() { "0", "1", "1", "a" };
            TokenStream stream = new(tokens);

            var result = adapter.AdaptFromTokens(stream);
            Assert.False(result.Success);
        }

        [Test]
        public void AlphaColorParser_NotParseable5()
        {
            var adapter = new AlphaColorAdapter();

            List<string> tokens = new() { "0", "1", "1" };
            TokenStream stream = new(tokens);

            var result = adapter.AdaptFromTokens(stream);
            Assert.False(result.Success);
        }
    }
}
