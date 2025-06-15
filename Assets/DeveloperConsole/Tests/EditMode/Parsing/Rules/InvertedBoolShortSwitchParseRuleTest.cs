using NUnit.Framework;
using System.Collections.Generic;
using DeveloperConsole.Command;
using DeveloperConsole.Parsing;
using DeveloperConsole.Parsing.Rules;
using DeveloperConsole.Parsing.Tokenizing;

namespace DeveloperConsole.Tests.Parsing.Rules
{
    public class InvertedBoolShortSwitchParseRuleTest
    {
        private InvertedBoolShortSwitchParseRule rule;

        [SetUp]
        public void SetUp()
        {
            rule = new InvertedBoolShortSwitchParseRule();
        }

        [Test]
        public void CanMatch_ShouldReturnTrue_ForValidInvertedShortSwitch()
        {
            var field = new FieldBuilder()
                .WithName("verbose")
                .WithType(typeof(bool))
                .WithAttribute(new SwitchAttribute("v", "desc"))
                .Build();

            var spec = new ArgumentSpecification(field);
            var context = new ParseContext(null);

            Assert.IsTrue(rule.CanMatch("-no-v", spec, context));
        }

        [Test]
        public void CanMatch_ShouldReturnFalse_IfNoSwitchAttribute()
        {
            var field = new FieldBuilder()
                .WithName("verbose")
                .WithType(typeof(bool))
                .Build();

            var spec = new ArgumentSpecification(field);
            var context = new ParseContext(null);

            Assert.IsFalse(rule.CanMatch("-no-v", spec, context));
        }

        [Test]
        public void CanMatch_ShouldReturnFalse_IfNotStartingWithNoPrefix()
        {
            var field = new FieldBuilder()
                .WithName("verbose")
                .WithType(typeof(bool))
                .WithAttribute(new SwitchAttribute("v", "desc"))
                .Build();

            var spec = new ArgumentSpecification(field);
            var context = new ParseContext(null);

            Assert.IsFalse(rule.CanMatch("-v", spec, context));
        }

        [Test]
        public void CanMatch_ShouldReturnFalse_IfTokenTooShort()
        {
            var field = new FieldBuilder()
                .WithName("v")
                .WithType(typeof(bool))
                .WithAttribute(new SwitchAttribute("v", "desc"))
                .Build();

            var spec = new ArgumentSpecification(field);
            var context = new ParseContext(null);

            Assert.IsFalse(rule.CanMatch("-no-", spec, context));
        }

        [Test]
        public void CanMatch_ShouldReturnFalse_IfNameDoesNotMatch()
        {
            var field = new FieldBuilder()
                .WithName("v")
                .WithType(typeof(bool))
                .WithAttribute(new SwitchAttribute("v", "desc"))
                .Build();

            var spec = new ArgumentSpecification(field);
            var context = new ParseContext(null);

            Assert.IsFalse(rule.CanMatch("-no-a", spec, context));
        }
        
        [Test]
        public void CanMatch_ShouldReturnFalse_IfFieldTypeIsNotBool()
        {
            var field = new FieldBuilder()
                .WithName("v")
                .WithType(typeof(string))
                .WithAttribute(new SwitchAttribute("v", "desc"))
                .Build();

            var spec = new ArgumentSpecification(field);
            var context = new ParseContext(null);

            Assert.IsFalse(rule.CanMatch("-no-v", spec, context));
        }

        [Test]
        public void TryParse_ShouldReturnSuccess_WithInvertedTrue()
        {
            var field = new FieldBuilder()
                .WithName("verbose")
                .WithType(typeof(bool))
                .WithAttribute(new SwitchAttribute("v", "desc"))
                .Build();

            var spec = new ArgumentSpecification(field);
            var stream = new TokenStream(new List<string> { "-v", "true" });

            var result = rule.TryParse(stream, spec);
            Assert.AreEqual(Status.Success, result.Status);
            Assert.AreEqual(false, result.Value);
        }

        [Test]
        public void TryParse_ShouldReturnSuccess_WithInvertedImpliedTrue()
        {
            var field = new FieldBuilder()
                .WithName("verbose")
                .WithType(typeof(bool))
                .WithAttribute(new SwitchAttribute("v", "desc"))
                .Build();

            var spec = new ArgumentSpecification(field);
            var stream = new TokenStream(new List<string> { "-v" });

            var result = rule.TryParse(stream, spec);

            Assert.AreEqual(Status.Success, result.Status);
            Assert.AreEqual(false, result.Value);
        }
        
        [Test]
        public void TryParse_ShouldReturnSuccess_WithInvertedFalse()
        {
            var field = new FieldBuilder()
                .WithName("verbose")
                .WithType(typeof(bool))
                .WithAttribute(new SwitchAttribute("v", "desc"))
                .Build();

            var spec = new ArgumentSpecification(field);
            var stream = new TokenStream(new List<string> { "-v", "false" });

            var result = rule.TryParse(stream, spec);

            Assert.AreEqual(Status.Success, result.Status);
            Assert.AreEqual(true, result.Value);
        }

        [Test]
        public void TryParse_ShouldReturnFalse_WhenInvalidBoolToken()
        {
            var field = new FieldBuilder()
                .WithName("verbose")
                .WithType(typeof(bool))
                .WithAttribute(new SwitchAttribute("v", "desc"))
                .Build();

            var spec = new ArgumentSpecification(field);
            var stream = new TokenStream(new List<string> { "-v", "invalid" });

            var result = rule.TryParse(stream, spec);

            Assert.AreEqual(Status.Success, result.Status);
            Assert.AreEqual(false, result.Value);
        }
    }
}