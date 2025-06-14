using System.Collections.Generic;
using DeveloperConsole.Parsing;
using DeveloperConsole.Parsing.Tokenizing;
using NUnit.Framework;

namespace DeveloperConsole.Tests.Parsing
{
    public class BoolParserTest
    {

        [Test]
        public void BoolParser_NotParseable()
        {
            var parser = new BoolParser();

            List<string> tokens = new() { null, "", "asd", "-123d", "asdw", "10d", "t", "f", "T", "F"};

            foreach (var token in tokens)
            {
                Assert.False(parser.TryParse(new TokenStream(new List<string> {token}), out object x));
            }
        }
    
        [Test]
        public void BoolParser_Parseable()
        {
            var parser = new BoolParser();

            List<string> tokens = new() { "true", "false", "True", "False", "TRUE", "FALSE", "TrUe", "fAlsE"};
            TokenStream stream = new(tokens);

            int count = 0;
            while (stream.HasMore())
            {
                Assert.True(parser.TryParse(stream, out object x));
                Assert.AreEqual(bool.Parse(tokens[count++]), x);
            }
        }
    }
}