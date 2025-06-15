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

            List<string> tokens = new() { null, "", "asd", "-123d", "1", "t", "F" };

            foreach (var token in tokens)
            {
                TokenStream tokenStream = new TokenStream(new List<string> { token });
                var result = parser.TryParseStream(tokenStream);
                Assert.False(result.Success);
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
                var result = parser.TryParseStream(stream);
                Assert.True(result.Success);
                Assert.AreEqual(bool.Parse(tokens[count++]), result.Value);
            }
        }
    }
}