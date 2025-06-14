using NUnit.Framework;
using System.Collections.Generic;
using DeveloperConsole.Parsing;
using DeveloperConsole.Parsing.Tokenizing;

namespace DeveloperConsole.Tests.Parsing
{
    public class IntParserTest
    {
        [Test]
        public void IntParser_NotParseable()
        {
            var parser = new IntParser();

            List<string> tokens = new() { null, "", "asd", "-123.0", "123.0", "10f", "1.5"};
            TokenStream stream = new(tokens);

            while (stream.HasMore())
            {
                Assert.False(parser.TryParse(stream, out object x));
            }
        }
        
        [Test]
        public void IntParser_Parseable()
        {
            var parser = new IntParser();

            List<string> tokens = new() { "-1", "0", "1", "-123123", "8439278" };
            TokenStream stream = new(tokens);

            int count = 0;
            while (stream.HasMore())
            {
                Assert.True(parser.TryParse(stream, out object x));
                Assert.AreEqual(int.Parse(tokens[count++]), x);
            }
        }
    }
}
