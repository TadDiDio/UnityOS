using NUnit.Framework;
using System.Collections.Generic;
using DeveloperConsole.Parsing.Tokenizing;
using DeveloperConsole.Parsing.TypeAdapting;

namespace DeveloperConsole.Tests.Parsing
{
    public class IntAdapterTest
    {
        [Test]
        public void IntParser_NotParseable1()
        {
            var adapter = new IntAdapter();

            List<string> tokens = new() { null };
            TokenStream stream = new(tokens);

            var result = adapter.AdaptFromTokens(stream);
            Assert.False(result.Success);
        }

        [Test]
        public void IntParser_NotParseable2()
        {
            var adapter = new IntAdapter();

            List<string> tokens = new() { "" };
            TokenStream stream = new(tokens);

            var result = adapter.AdaptFromTokens(stream);
            Assert.False(result.Success);
        }

        [Test]
        public void IntParser_NotParseable3()
        {
            var adapter = new IntAdapter();

            List<string> tokens = new() { "asd" };
            TokenStream stream = new(tokens);

            var result = adapter.AdaptFromTokens(stream);
            Assert.False(result.Success);
        }

        [Test]
        public void IntParser_NotParseable4()
        {
            var adapter = new IntAdapter();

            List<string> tokens = new() { "-123.0" };
            TokenStream stream = new(tokens);

            var result = adapter.AdaptFromTokens(stream);
            Assert.False(result.Success);
        }

        [Test]
        public void IntParser_NotParseable5()
        {
            var adapter = new IntAdapter();

            List<string> tokens = new() { "123.0" };
            TokenStream stream = new(tokens);

            var result = adapter.AdaptFromTokens(stream);
            Assert.False(result.Success);
        }

        [Test]
        public void IntParser_NotParseable6()
        {
            var adapter = new IntAdapter();

            List<string> tokens = new() { "10f" };
            TokenStream stream = new(tokens);

            var result = adapter.AdaptFromTokens(stream);
            Assert.False(result.Success);
        }

        [Test]
        public void IntParser_NotParseable7()
        {
            var adapter = new IntAdapter();

            List<string> tokens = new() { "1.5" };
            TokenStream stream = new(tokens);

            var result = adapter.AdaptFromTokens(stream);
            Assert.False(result.Success);
        }

        [Test]
        public void IntParser_Parseable()
        {
            var adapter = new IntAdapter();

            List<string> tokens = new() { "-1", "0", "1", "-123123", "8439278" };
            TokenStream stream = new(tokens);

            int count = 0;
            while (stream.HasMore())
            {
                var result = adapter.AdaptFromTokens(stream);
                Assert.True(result.Success);
                Assert.AreEqual(int.Parse(tokens[count++]), result.Value);
            }
        }
    }
}
