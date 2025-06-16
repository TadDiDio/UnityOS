using System.Collections.Generic;
using DeveloperConsole.Command;
using DeveloperConsole.Parsing;
using DeveloperConsole.Parsing.Rules;
using DeveloperConsole.Parsing.Tokenizing;
using NUnit.Framework;
using UnityEngine;

namespace DeveloperConsole.Tests.Parsing.Rules
{
    public class PositionalParseRuleTest
    {
        private IParseRule rule;

        private ArgumentSpecification firstArg;
        private ArgumentSpecification secondArg;

        [SetUp]
        public void Setup()
        {
            rule = new PositionalParseRule();

            var field1 = new FieldBuilder()
                .WithName("message")
                .WithType(typeof(string))
                .WithAttribute(new PositionalAttribute(0, "desc"))
                .Build();

            var field2 = new FieldBuilder()
                .WithName("count")
                .WithType(typeof(int))
                .WithAttribute(new PositionalAttribute(1, "desc"))
                .Build();

            firstArg = new ArgumentSpecification(field1);
            secondArg = new ArgumentSpecification(field2);
        }

        [Test]
        public void Filter_ShouldMatch_FirstPositional()
        {
            var context = new ParseContext(null);
            ArgumentSpecification[] args = { firstArg, secondArg };

            var result = rule.Filter("hello", args, context);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(firstArg, result[0]);
        }

        [Test]
        public void Filter_ShouldMatch_SecondPositional_AfterContextIncrement()
        {
            var context = new ParseContext(null);
            context.SetData(PositionalParseRule.PositionalIndexKey, 1);
            ArgumentSpecification[] args = { firstArg, secondArg };

            var result = rule.Filter("123", args, context);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(secondArg, result[0]);
        }

        [Test]
        public void Filter_ShouldReturnNull_WhenNoMatchingIndex()
        {
            var context = new ParseContext(null);
            context.SetData(PositionalParseRule.PositionalIndexKey, 2);
            ArgumentSpecification[] args = { firstArg, secondArg };

            var result = rule.Filter("anything", args, context);

            Assert.IsNull(result);
        }

        [Test]
        public void Filter_ShouldReturnNull_WhenNoAttribute()
        {
            var field = new FieldBuilder()
                .WithName("message")
                .WithType(typeof(int))
                .WithAttribute(new VariadicAttribute("desc"))
                .Build();

            ArgumentSpecification variadic = new(field);
            
            ArgumentSpecification[] args = { variadic };
            var result = rule.Filter("anything", args, new ParseContext(null));

            Assert.IsNull(result);
        }
        
        [Test]
        public void Apply_ShouldParse_StringSuccessfully()
        {
            ArgumentSpecification[] args = { firstArg };
            var tokens = new List<string> { "hello world" };
            var stream = new TokenStream(tokens);

            var result = rule.Apply(stream, args);

            Assert.AreEqual(Status.Success, result.Status);
            Assert.AreEqual("hello world", result.Values[firstArg]);
        }

        [Test]
        public void Apply_ShouldParse_IntSuccessfully()
        {
            ArgumentSpecification[] args = { secondArg };
            var tokens = new List<string> { "42" };
            var stream = new TokenStream(tokens);

            var result = rule.Apply(stream, args);

            Assert.AreEqual(Status.Success, result.Status);
            Assert.AreEqual(42, result.Values[secondArg]);
        }

        [Test]
        public void Apply_ShouldFail_InvalidInt()
        {
            ArgumentSpecification[] args = { secondArg };
            var tokens = new List<string> { "notanumber" };
            var stream = new TokenStream(tokens);

            var result = rule.Apply(stream, args);

            Assert.AreEqual(Status.Fail, result.Status);
            Assert.IsTrue(result.ErrorMessage.Contains("Parsing"));
        }

        [Test]
        public void Apply_ShouldFail_WhenMultipleArgsPassed()
        {
            using SilentLogCapture log = new();

            ArgumentSpecification[] args = { firstArg, secondArg };
            var tokens = new List<string> { "value" };
            var stream = new TokenStream(tokens);

            var result = rule.Apply(stream, args);

            Assert.AreEqual(Status.Fail, result.Status);
            Assert.IsTrue(log.HasLog(LogType.Error, "Too many arguments"));
        }
    }
}