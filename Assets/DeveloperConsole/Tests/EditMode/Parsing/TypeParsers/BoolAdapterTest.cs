using System.Collections.Generic;
using DeveloperConsole.Parsing.Tokenizing;
using DeveloperConsole.Parsing.TypeAdapting;
using NUnit.Framework;

namespace DeveloperConsole.Tests.Parsing
{
    public class BoolAdapterTest
    {

        [Test]
        public void BoolParser_NotParseable()
        {
            var adapter = new BoolAdapter();

            List<string> tokens = new() { null, "", "asd", "-123d", "1", "t", "F" };

            foreach (var token in tokens)
            {
                TokenStream tokenStream = new TokenStream(new List<string> { token });
                var result = adapter.AdaptFromTokens(tokenStream);
                Assert.False(result.Success);
            }
        }



        [Test]
        public void BoolParser_Parseable()
        {
            var adapter = new BoolAdapter();

            List<string> tokens = new() { "true", "false", "True", "False", "TRUE", "FALSE", "TrUe", "fAlsE"};
            TokenStream stream = new(tokens);

            int count = 0;
            while (stream.HasMore())
            {
                var result = adapter.AdaptFromTokens(stream);
                Assert.True(result.Success);
                Assert.AreEqual(bool.Parse(tokens[count++]), result.Value);
            }
        }
    }
}
