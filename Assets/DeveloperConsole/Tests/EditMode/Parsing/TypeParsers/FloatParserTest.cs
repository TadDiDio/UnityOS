using System.Collections.Generic;
using NUnit.Framework;

namespace DeveloperConsole.Tests
{
    public class FloatParserTest
    {
        [Test]
        public void FloatParser_NotParseable()
        {
            var parser = new FloatParser();
            Assert.AreEqual(parser.TokenCount(), 1);

            List<string> tokens = new() { null, "", "asd", "-123d", "123f", "10d"};
            TokenStream stream = new(tokens);

            while (stream.HasMore())
            {
                Assert.False(parser.TryParse(stream, out object x));
            }
        }
    
        [Test]
        public void FloatParser_Parseable()
        {
            var parser = new FloatParser();

            List<string> tokens = new() { "-1", "0", "1", "-123123", "8439278", "324.432", ".004324", "-.0432"};
            TokenStream stream = new(tokens);

            int count = 0;
            while (stream.HasMore())
            {
                Assert.True(parser.TryParse(stream, out object x));
                Assert.AreEqual(float.Parse(tokens[count++]), x);
            }
        }
    }
}