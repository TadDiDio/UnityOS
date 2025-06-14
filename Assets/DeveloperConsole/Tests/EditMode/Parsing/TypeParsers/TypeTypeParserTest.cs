using System.Collections.Generic;
using DeveloperConsole.Parsing;
using DeveloperConsole.Parsing.Tokenizing;
using NUnit.Framework;

namespace DeveloperConsole.Tests.Parsing
{
    public class TypeTypeParserTest : ConsoleTest
    {
        [Test]
        public void TryParse_ReturnsTrueAndCorrectType_WhenTypeNameIsValid()
        {
            var parser = new TypeTypeParser();
            var tokens = new TokenStream(new List<string> { "int" });
            bool result = parser.TryParse(tokens, out var obj);

            Assert.IsTrue(result);
            Assert.AreEqual(typeof(int), obj);
        }

        [Test]
        public void TryParse_ReturnsFalseAndNull_WhenTypeNameIsInvalid()
        {
            var parser = new TypeTypeParser();
            var tokens = new TokenStream(new List<string> { "notatype" });
            bool result = parser.TryParse(tokens, out var obj);

            Assert.IsFalse(result);
            Assert.IsNull(obj);
        }

        [Test]
        public void TryParse_ConsumesOnlyOneToken()
        {
            var parser = new TypeTypeParser();
            var tokens = new TokenStream(new List<string> { "int", "extra" });
        
            parser.TryParse(tokens, out var obj);

            Assert.AreEqual("extra", tokens.Peek());
        }
    }
}