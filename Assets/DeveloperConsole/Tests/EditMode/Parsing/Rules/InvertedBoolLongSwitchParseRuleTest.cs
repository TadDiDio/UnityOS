using System.Collections.Generic;
using DeveloperConsole.Command;
using DeveloperConsole.Parsing;
using DeveloperConsole.Parsing.Rules;
using DeveloperConsole.Parsing.Tokenizing;
using NUnit.Framework;

namespace DeveloperConsole.Tests.Parsing.Rules
{
    public class InvertedBoolLongSwitchParseRuleTest
    {
        private InvertedBoolLongSwitchParseRule rule;

        [SetUp]
        public void SetUp()
        {
            rule = new InvertedBoolLongSwitchParseRule();
        }

        [Test]
        public void CanMatch_ShouldReturnTrue_ForValidInvertedBoolSwitch()
        {
            var field = new FieldBuilder()
                .WithName("verbose")
                .WithType(typeof(bool))
                .WithAttribute(new SwitchAttribute("v", "desc"))
                .Build();

            var spec = new ArgumentSpecification(field);
            var context = new ParseContext();

            Assert.IsTrue(rule.CanMatch("--no-verbose", spec, context));
        }

        [Test]
        public void CanMatch_ShouldReturnFalse_IfNoSwitchAttribute()
        {
            var field = new FieldBuilder()
                .WithName("verbose")
                .WithType(typeof(bool))
                .Build();

            var spec = new ArgumentSpecification(field);
            var context = new ParseContext();

            Assert.IsFalse(rule.CanMatch("--no-verbose", spec, context));
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
            var context = new ParseContext();

            Assert.IsFalse(rule.CanMatch("--verbose", spec, context));
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
            var context = new ParseContext();

            Assert.IsFalse(rule.CanMatch("--no-", spec, context));
        }

        [Test]
        public void CanMatch_ShouldReturnFalse_IfWrongName()
        {
            var field = new FieldBuilder()
                .WithName("v")
                .WithType(typeof(bool))
                .WithAttribute(new SwitchAttribute("flag", "desc"))
                .Build();

            var spec = new ArgumentSpecification(field);
            var context = new ParseContext();

            Assert.IsFalse(rule.CanMatch("--no-flags", spec, context));
        }
        
        [Test]
        public void CanMatch_ShouldReturnFalse_IfFieldTypeIsNotBool()
        {
            var field = new FieldBuilder()
                .WithName("verbose")
                .WithType(typeof(int))
                .WithAttribute(new SwitchAttribute("v", "desc"))
                .Build();

            var spec = new ArgumentSpecification(field);
            var context = new ParseContext();

            Assert.IsFalse(rule.CanMatch("--no-verbose", spec, context));
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
            var stream = new TokenStream(new List<string> { "true" });

            var success = rule.TryParse(stream, spec, out var result);

            Assert.IsTrue(success);
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
            var stream = new TokenStream(new List<string> { "false" });

            var success = rule.TryParse(stream, spec, out var result);

            Assert.IsTrue(success);
            Assert.AreEqual(Status.Success, result.Status);
            Assert.AreEqual(true, result.Value);
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
            var stream = new TokenStream(new List<string> { });

            var success = rule.TryParse(stream, spec, out var result);

            Assert.IsTrue(success);
            Assert.AreEqual(Status.Success, result.Status);
            Assert.AreEqual(false, result.Value);
        }
        
        [Test]
        public void TryParse_ShouldReturnFail_WhenTokenIsInvalid()
        {
            var field = new FieldBuilder()
                .WithName("verbose")
                .WithType(typeof(bool))
                .WithAttribute(new SwitchAttribute("v", "desc"))
                .Build();

            var spec = new ArgumentSpecification(field);
            var stream = new TokenStream(new List<string> { "notabool" });

            var success = rule.TryParse(stream, spec, out var result);

            Assert.IsFalse(success);
            Assert.AreEqual(Status.Fail, result.Status);
        }
    }
}