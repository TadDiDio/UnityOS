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
        private PositionalParseRule rule;

        [SetUp]
        public void SetUp()
        {
            rule = new PositionalParseRule();
        }

        [Test]
        public void CanMatch_True_WhenIndexMatches()
        {
            var field = new FieldBuilder()
                .WithName("name")
                .WithType(typeof(string))
                .WithAttribute(new PositionalAttribute(0, "desc"))
                .Build();

            var spec = new ArgumentSpecification(field);
            var context = new ParseContext();

            Assert.IsTrue(rule.CanMatch("dragon", spec, context));
        }

        [Test]
        public void CanMatch_False_WhenIndexDoesNotMatch()
        {
            var field = new FieldBuilder()
                .WithName("age")
                .WithType(typeof(int))
                .WithAttribute(new PositionalAttribute(1, "desc"))
                .Build();

            var spec = new ArgumentSpecification(field);
            var context = new ParseContext();
            context.SetData(PositionalParseRule.PositionalIndexKey, 0); // mismatch

            Assert.IsFalse(rule.CanMatch("25", spec, context));
        }

        [Test]
        public void CanMatch_IncrementsPositionalIndexOnSuccess()
        {
            var field = new FieldBuilder()
                .WithName("class")
                .WithType(typeof(string))
                .WithAttribute(new PositionalAttribute(0, "desc"))
                .Build();

            var spec = new ArgumentSpecification(field);
            var context = new ParseContext();

            rule.CanMatch("warrior", spec, context);

            Assert.IsTrue(context.TryGetData(PositionalParseRule.PositionalIndexKey, out int newIndex));
            Assert.AreEqual(1, newIndex);
        }

        [Test]
        public void TryParse_Succeeds_WithString()
        {
            var field = new FieldBuilder()
                .WithName("name")
                .WithType(typeof(string))
                .WithAttribute(new PositionalAttribute(0, "desc"))
                .Build();

            var spec = new ArgumentSpecification(field);
            var tokens = new TokenStream(new List<string> { "dragon" });

            var result = rule.TryParse(tokens, spec, out var parseResult);

            Assert.IsTrue(result);
            Assert.AreEqual(Status.Success, parseResult.Status);
            Assert.AreEqual("dragon", parseResult.Value);
        }

        [Test]
        public void TryParse_Succeeds_WithInt()
        {
            var field = new FieldBuilder()
                .WithName("level")
                .WithType(typeof(int))
                .WithAttribute(new PositionalAttribute(0, "desc"))
                .Build();

            var spec = new ArgumentSpecification(field);
            var tokens = new TokenStream(new List<string> { "42" });

            var result = rule.TryParse(tokens, spec, out var parseResult);

            Assert.IsTrue(result);
            Assert.AreEqual(Status.Success, parseResult.Status);
            Assert.AreEqual(42, parseResult.Value);
        }

        [Test]
        public void TryParse_Succeeds_WithVector2()
        {
            var field = new FieldBuilder()
                .WithName("position")
                .WithType(typeof(Vector2))
                .WithAttribute(new PositionalAttribute(0, "desc"))
                .Build();

            var spec = new ArgumentSpecification(field);
            var tokens = new TokenStream(new List<string> { "1.0", "2.0" });

            var result = rule.TryParse(tokens, spec, out var parseResult);

            Assert.IsTrue(result);
            Assert.AreEqual(Status.Success, parseResult.Status);
            Assert.AreEqual(new Vector2(1f, 2f), parseResult.Value);
        }

        [Test]
        public void TryParse_Fails_WithInvalidInput()
        {
            var field = new FieldBuilder()
                .WithName("score")
                .WithType(typeof(int))
                .WithAttribute(new PositionalAttribute(0, "desc"))
                .Build();

            var spec = new ArgumentSpecification(field);
            var tokens = new TokenStream(new List<string> { "not-an-int" });

            var result = rule.TryParse(tokens, spec, out var parseResult);

            Assert.IsFalse(result);
            Assert.AreEqual(Status.Fail, parseResult.Status);
        }

        [Test]
        public void CanMatch_UsesExistingIndex_WhenPresent()
        {
            var field = new FieldBuilder()
                .WithName("target")
                .WithType(typeof(string))
                .WithAttribute(new PositionalAttribute(2, "desc"))
                .Build();

            var spec = new ArgumentSpecification(field);
            var context = new ParseContext();
            context.SetData(PositionalParseRule.PositionalIndexKey, 2);

            Assert.IsTrue(rule.CanMatch("enemy", spec, context));
        }
    }
}