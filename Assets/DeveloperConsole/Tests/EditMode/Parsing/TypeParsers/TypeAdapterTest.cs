using System.Collections.Generic;
using DeveloperConsole.Parsing.Tokenizing;
using DeveloperConsole.Parsing.TypeAdapting;
using NUnit.Framework;

namespace DeveloperConsole.Tests.Parsing
{
    public class TypeAdapterTest : ConsoleTest
    {
        [Test]
        public void TryParse_ReturnsTrueAndCorrectType_WhenTypeNameIsValid()
        {
            var adapter = new TypeAdapter();
            var tokens = new TokenStream(new List<string> { "int" });

            var result = adapter.AdaptFromTokens(tokens);

            Assert.IsTrue(result.Success);
            Assert.AreEqual(typeof(int), result.Value);
        }

        [Test]
        public void TryParse_ReturnsFalseAndNull_WhenTypeNameIsInvalid()
        {
            var adapter = new TypeAdapter();
            var tokens = new TokenStream(new List<string> { "notatype" });

            var result = adapter.AdaptFromTokens(tokens);

            Assert.IsFalse(result.Success);
        }

        [Test]
        public void TryParse_ConsumesOnlyOneToken()
        {
            var adapter = new TypeAdapter();
            var tokens = new TokenStream(new List<string> { "int", "extra" });

            adapter.AdaptFromTokens(tokens);

            Assert.AreEqual("extra", tokens.Peek());
        }
    }
}
