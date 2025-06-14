using System.Collections.Generic;
using DeveloperConsole.Parsing;
using DeveloperConsole.Parsing.Tokenizing;
using NUnit.Framework;

namespace DeveloperConsole.Tests.Parsing
{
    public class StringParserTest
    {
        [Test]
        public void StringParser_Parseable()
        {
            var parser = new StringParser();

            List<string> tokens = new() { null, "-1", "0", "1", "-123123", "8439278", "risaodj", ".323234\"\n//..<P{<#$#@_)"};
            TokenStream stream = new(tokens);

            int count = 0;
            while (stream.HasMore())
            {
                Assert.True(parser.TryParse(stream, out object x));
                Assert.AreEqual(tokens[count++], x);
            }
        }
    }
}