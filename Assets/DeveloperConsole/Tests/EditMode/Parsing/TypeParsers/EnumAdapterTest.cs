using System.Collections.Generic;
using DeveloperConsole.Parsing.Tokenizing;
using DeveloperConsole.Parsing.TypeAdapting;
using NUnit.Framework;

namespace DeveloperConsole.Tests.Parsing
{
    public class EnumAdapterTest
    {
        private enum TestEnum
        {
            Dog,
            Cat
        }

        [Test]
        public void EnumColorParser_Parseable1()
        {
            var adapter = new EnumAdapter<TestEnum>();

            List<string> tokens = new() { "dog"};
            TokenStream stream = new(tokens);

            var result = adapter.AdaptFromTokens(stream);
            Assert.True(result.Success);
            Assert.AreEqual(TestEnum.Dog, result.Value);
        }

        [Test]
        public void EnumColorParser_Parseable2()
        {
            var adapter = new EnumAdapter<TestEnum>();

            List<string> tokens = new() { "cat" };
            TokenStream stream = new(tokens);

            var result = adapter.AdaptFromTokens(stream);
            Assert.True(result.Success);
            Assert.AreEqual(TestEnum.Cat, result.Value);
        }

        [Test]
        public void EnumColorParser_Parseable3()
        {
            var adapter = new EnumAdapter<TestEnum>();

            List<string> tokens = new() { " dog" };
            TokenStream stream = new(tokens);

            var result = adapter.AdaptFromTokens(stream);
            Assert.True(result.Success);
            Assert.AreEqual(TestEnum.Dog, result.Value);
        }

        [Test]
        public void EnumColorParser_Parseable4()
        {
            var adapter = new EnumAdapter<TestEnum>();

            List<string> tokens = new() { " 1 ", "DOG", "CAT" };
            TokenStream stream = new(tokens);

            var result = adapter.AdaptFromTokens(stream);
            Assert.True(result.Success);
            Assert.AreEqual(TestEnum.Cat, result.Value);
        }

        [Test]
        public void EnumColorParser_Parseable5()
        {
            var adapter = new EnumAdapter<TestEnum>();

            List<string> tokens = new() {  "DOG" };
            TokenStream stream = new(tokens);

            var result = adapter.AdaptFromTokens(stream);
            Assert.True(result.Success);
            Assert.AreEqual(TestEnum.Dog, result.Value);
        }

        [Test]
        public void EnumParser_NotParseable1()
        {
            var adapter = new EnumAdapter<TestEnum>();

            List<string> tokens = new() { null };
            TokenStream stream = new(tokens);

            var result = adapter.AdaptFromTokens(stream);
            Assert.False(result.Success);
        }

        [Test]
        public void EnumParser_NotParseable2()
        {
            var adapter = new EnumAdapter<TestEnum>();

            List<string> tokens = new() { "a" };
            TokenStream stream = new(tokens);

            var result = adapter.AdaptFromTokens(stream);
            Assert.False(result.Success);
        }

        [Test]
        public void EnumParser_NotParseable3()
        {
            var adapter = new EnumAdapter<TestEnum>();

            List<string> tokens = new() { "20" };
            TokenStream stream = new(tokens);

            var result = adapter.AdaptFromTokens(stream);
            Assert.False(result.Success);
        }

        [Test]
        public void EnumParser_NotParseable4()
        {
            var adapter = new EnumAdapter<TestEnum>();

            List<string> tokens = new() { "catt" };
            TokenStream stream = new(tokens);

            var result = adapter.AdaptFromTokens(stream);
            Assert.False(result.Success);
        }

        [Test]
        public void EnumParser_NotParseable5()
        {
            var adapter = new EnumAdapter<TestEnum>();

            List<string> tokens = new() { "adog"};
            TokenStream stream = new(tokens);

            var result = adapter.AdaptFromTokens(stream);
            Assert.False(result.Success);
        }
    }
}
