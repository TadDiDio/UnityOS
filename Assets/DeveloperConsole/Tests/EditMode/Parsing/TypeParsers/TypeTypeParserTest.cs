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
            
            var result = parser.TryParseStream(tokens);

            Assert.IsTrue(result.Success);
            Assert.AreEqual(typeof(int), result.Value);
        }

        [Test]
        public void TryParse_ReturnsFalseAndNull_WhenTypeNameIsInvalid()
        {
            var parser = new TypeTypeParser();
            var tokens = new TokenStream(new List<string> { "notatype" });
            
            var result = parser.TryParseStream(tokens);

            Assert.IsFalse(result.Success);
        }

        [Test]
        public void TryParse_ConsumesOnlyOneToken()
        {
            var parser = new TypeTypeParser();
            var tokens = new TokenStream(new List<string> { "int", "extra" });
        
            parser.TryParseStream(tokens);

            Assert.AreEqual("extra", tokens.Peek());
        }
    }
}