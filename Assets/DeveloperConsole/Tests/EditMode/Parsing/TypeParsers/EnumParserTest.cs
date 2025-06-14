using System.Collections.Generic;
using System.Linq;
using DeveloperConsole.Parsing;
using DeveloperConsole.Parsing.Tokenizing;
using NUnit.Framework;

namespace DeveloperConsole.Tests.Parsing
{
    public class EnumParserTest
    {
        private enum TestEnum
        {
            Dog,
            Cat
        }
        
        [Test]
        public void EnumColorParser_Parseable()
        {
            var parser = new EnumParser<TestEnum>();
            
            List<string> tokens = new() { "dog", "cat", " dog", " 1 ", "DOG", "CAT" };
            TokenStream stream = new(tokens);

            Assert.True(parser.TryParse(stream, out object x));
            Assert.AreEqual(TestEnum.Dog, x);
            Assert.AreEqual(stream.Remaining().Count(), 5);
            
            Assert.True(parser.TryParse(stream, out x));
            Assert.AreEqual(TestEnum.Cat, x);
            Assert.AreEqual(stream.Remaining().Count(), 4);
            
            Assert.True(parser.TryParse(stream, out x));
            Assert.AreEqual(TestEnum.Dog, x);
            Assert.AreEqual(stream.Remaining().Count(), 3);
            
            Assert.True(parser.TryParse(stream, out x));
            Assert.AreEqual(TestEnum.Cat, x);
            Assert.AreEqual(stream.Remaining().Count(), 2);
            
            Assert.True(parser.TryParse(stream, out x));
            Assert.AreEqual(TestEnum.Dog, x);
            Assert.AreEqual(stream.Remaining().Count(), 1);
            
            Assert.True(parser.TryParse(stream, out x));
            Assert.AreEqual(TestEnum.Cat, x);
            Assert.False(stream.HasMore());
        }
        
        [Test]
        public void EnumParser_NotParseable()
        {
            var parser = new EnumParser<TestEnum>();

            List<string> tokens = new() { null, "a", "20", "catt", "adog"};
            TokenStream stream = new(tokens);

            Assert.False(parser.TryParse(stream, out _));
            Assert.AreEqual(stream.Remaining().Count(), 4);
            
            Assert.False(parser.TryParse(stream, out _));
            Assert.AreEqual(stream.Remaining().Count(), 3);
            
            Assert.False(parser.TryParse(stream, out _));
            Assert.AreEqual(stream.Remaining().Count(), 2);
            
            Assert.False(parser.TryParse(stream, out _));
            Assert.AreEqual(stream.Remaining().Count(), 1);
            
            Assert.False(parser.TryParse(stream, out _));
            Assert.False(stream.HasMore());
        }
    }
}