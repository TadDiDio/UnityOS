using System.Collections.Generic;
using DeveloperConsole.Parsing.Tokenizing;
using DeveloperConsole.Parsing.TypeAdapting;
using NUnit.Framework;

namespace DeveloperConsole.Tests.Parsing
{
    public class StringAdapterTest
    {
        [Test]
        public void StringParser_Parseable()
        {
            var adapter = new StringAdapter();

            List<string> tokens = new() { null, "-1", "0", "1", "-123123", "8439278", "risaodj", ".323234\"\n//..<P{<#$#@_)"};
            TokenStream stream = new(tokens);

            int count = 0;
            while (stream.HasMore())
            {
                var result = adapter.AdaptFromTokens(stream);
                Assert.True(result.Success);
                Assert.AreEqual(tokens[count++], result.Value);
            }
        }
    }
}
