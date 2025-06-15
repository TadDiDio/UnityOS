using System.Collections.Generic;
using DeveloperConsole.Command;
using DeveloperConsole.Parsing;
using DeveloperConsole.Parsing.Rules;
using DeveloperConsole.Parsing.Tokenizing;
using NUnit.Framework;
using UnityEngine;

namespace DeveloperConsole.Tests.Parsing.Rules
{
    public class LongSwitchParseRuleTest
    {
        private LongSwitchParseRule rule;

        [SetUp]
        public void SetUp()
        {
            rule = new LongSwitchParseRule();
        }

        [Test]
        public void CanMatch_ReturnsTrue_WhenTokenMatchesLongSwitch()
        {
            var field = new FieldBuilder()
                .WithName("speed")
                .WithType(typeof(float))
                .WithAttribute(new SwitchAttribute("s", "desc"))
                .Build();

            var spec = new ArgumentSpecification(field);
            var context = new ParseContext(null);

            Assert.IsTrue(rule.CanMatch("--speed", spec, context));
        }

        [Test]
        public void CanMatch_ReturnsFalse_WhenSwitchAttributeMissing()
        {
            var field = new FieldBuilder()
                .WithName("speed")
                .WithType(typeof(float))
                .Build();

            var spec = new ArgumentSpecification(field);
            var context = new ParseContext(null);

            Assert.IsFalse(rule.CanMatch("--speed", spec, context));
        }

        [Test]
        public void CanMatch_ReturnsFalse_WhenTokenDoesNotMatchFieldName()
        {
            var field = new FieldBuilder()
                .WithName("height")
                .WithType(typeof(float))
                .WithAttribute(new SwitchAttribute("h", "desc"))
                .Build();

            var spec = new ArgumentSpecification(field);
            var context = new ParseContext(null);

            Assert.IsFalse(rule.CanMatch("--width", spec, context));
        }

        [Test]
        public void CanMatch_ReturnsTrue_WhenTokenMatchesOverrideFieldName()
        {
            var field = new FieldBuilder()
                .WithName("height")
                .WithType(typeof(float))
                .WithAttribute(new SwitchAttribute("h", "desc", "width"))
                .Build();

            var spec = new ArgumentSpecification(field);
            var context = new ParseContext(null);

            Assert.IsTrue(rule.CanMatch("--width", spec, context));
        }
        
        [Test]
        public void TryParse_Succeeds_WithStringValue()
        {
            var field = new FieldBuilder()
                .WithName("name")
                .WithType(typeof(string))
                .WithAttribute(new SwitchAttribute("n", "desc"))
                .Build();

            var spec = new ArgumentSpecification(field);
            var tokens = new TokenStream(new List<string> { "-n", "dragon" });

            var result = rule.TryParse(tokens, spec);

            Assert.AreEqual(Status.Success, result.Status);
            Assert.AreEqual("dragon", result.Value);
        }

        [Test]
        public void TryParse_Succeeds_WithIntValue()
        {
            var field = new FieldBuilder()
                .WithName("count")
                .WithType(typeof(int))
                .WithAttribute(new SwitchAttribute("c", "desc"))
                .Build();

            var spec = new ArgumentSpecification(field);
            var tokens = new TokenStream(new List<string> { "-c", "42" });

            var result = rule.TryParse(tokens, spec);

            Assert.AreEqual(Status.Success, result.Status);
            Assert.AreEqual(42, result.Value);
        }

        [Test]
        public void TryParse_Succeeds_WithFloatValue()
        {
            var field = new FieldBuilder()
                .WithName("scale")
                .WithType(typeof(float))
                .WithAttribute(new SwitchAttribute("s", "desc"))
                .Build();

            var spec = new ArgumentSpecification(field);
            var tokens = new TokenStream(new List<string> { "-s", "3.14" });

            var result = rule.TryParse(tokens, spec);

            Assert.AreEqual(Status.Success, result.Status);
            Assert.AreEqual(3.14f, result.Value);
        }

        [Test]
        public void TryParse_Succeeds_WithVector2Value_FromTwoTokens()
        {
            var field = new FieldBuilder()
                .WithName("position")
                .WithType(typeof(Vector2))
                .WithAttribute(new SwitchAttribute("p", "desc"))
                .Build();

            var spec = new ArgumentSpecification(field);
            var tokens = new TokenStream(new List<string> { "-p", "1.0", "2.5" });

            var result = rule.TryParse(tokens, spec);

            Assert.AreEqual(Status.Success, result.Status);
            Assert.AreEqual(new Vector2(1.0f, 2.5f), result.Value);
        }

        [Test]
        public void TryParse_Fails_WithInvalidTypeInput()
        {
            var field = new FieldBuilder()
                .WithName("score")
                .WithType(typeof(int))
                .WithAttribute(new SwitchAttribute("s", "desc"))
                .Build();

            var spec = new ArgumentSpecification(field);
            var tokens = new TokenStream(new List<string> { "-s", "not-a-number" });

            var result = rule.TryParse(tokens, spec);

            Assert.AreEqual(Status.Fail, result.Status);
        }
    }
}