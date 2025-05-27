using System.Collections.Generic;
using NUnit.Framework;

namespace DeveloperConsole.Tests
{
    public class TokenStreamTest
    {
        [Test]
        public void TokenStream_HasMore()
        {
            var tokens = new List<string> { };
            var stream = new TokenStream(tokens);
            Assert.False(stream.HasMore());

            tokens.Add("hello");
            stream = new TokenStream(tokens);
            Assert.True(stream.HasMore());

            tokens.Add("world");
            stream = new TokenStream(tokens);
            Assert.True(stream.HasMore());

            tokens = new() { "Hello", "world" };
            stream = new TokenStream(tokens);
            stream.Next();
            Assert.True(stream.HasMore());
            stream.Next();
            Assert.False(stream.HasMore());
        }

        [Test]
        public void TokenStream_Next()
        {
            var tokens = new List<string> { };
            var stream = new TokenStream(tokens);

            Assert.IsNull(stream.Next());

            tokens.Add("hello");
            stream = new TokenStream(tokens);
            Assert.AreEqual("hello", stream.Next());
            Assert.IsNull(stream.Next());

            tokens = new List<string> { "hello", "world", "this is a string" };
            stream = new TokenStream(tokens);
            Assert.AreEqual("hello", stream.Next());
            Assert.AreEqual("world", stream.Next());
            Assert.AreEqual("this is a string", stream.Next());
            Assert.IsNull(stream.Next());
        }

        [Test]
        public void TokenStream_Peek()
        {
            var tokens = new List<string> { };
            var stream = new TokenStream(tokens);

            Assert.IsNull(stream.Peek());

            tokens.Add("hello");
            stream = new TokenStream(tokens);
            Assert.AreEqual("hello", stream.Peek());

            tokens = new List<string> { "hello", "world", "this is a string" };
            stream = new TokenStream(tokens);
            Assert.AreEqual("hello", stream.Peek());
            stream.Next();
            Assert.AreEqual("world", stream.Peek());
            stream.Next();
            Assert.AreEqual("this is a string", stream.Peek());
            stream.Next();
            Assert.IsNull(stream.Peek());
        }

        [Test]
        public void TokenStream_Read()
        {
            var tokens = new List<string> { };
            var stream = new TokenStream(tokens);

            Assert.IsNotNull(stream.Read(0));
            Assert.IsNotNull(stream.Read(1));
            Assert.IsNotNull(stream.Read(10));

            tokens = new List<string> { "hello", "world", "this is a string" };
            stream = new TokenStream(tokens);

            Assert.AreEqual(new List<string> { "hello", "world" }, stream.Read(2));
            Assert.AreEqual(new List<string> { "this is a string" }, stream.Read(4));
            Assert.AreEqual(new List<string>(), stream.Read(10));
        }

        [Test]
        public void TokenStream_Remaining()
        {
            var tokens = new List<string> { };
            var stream = new TokenStream(tokens);

            Assert.IsNotNull(stream.Remaining());

            tokens = new List<string> { "hello", "world", "this is a string" };
            stream = new TokenStream(tokens);

            Assert.AreEqual(new List<string> { "hello", "world", "this is a string" }, stream.Remaining());

            stream.Next();
            Assert.AreEqual(new List<string> { "world", "this is a string" }, stream.Remaining());
            stream.Read(1);
            Assert.AreEqual(new List<string> { "this is a string" }, stream.Remaining());
            stream.Read(10);
            Assert.AreEqual(new List<string>(), stream.Remaining());
        }
    }
}