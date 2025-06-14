using System.Collections.Generic;
using DeveloperConsole.Command;
using DeveloperConsole.Parsing;
using DeveloperConsole.Parsing.Rules;
using DeveloperConsole.Parsing.Tokenizing;
using NUnit.Framework;

namespace DeveloperConsole.Tests.Parsing.Rules
{
    public class BoolShortSwitchParseRuleTest
    {
        
        private BoolShortSwitchParseRule _rule;

        [SetUp]
        public void Setup()
        {
            _rule = new BoolShortSwitchParseRule();
        }

        [Test]
        public void CanMatch_ShouldReturnTrue_ForValidShortBoolSwitch()
        {
            var field = new FieldBuilder()
                .WithName("flag")
                .WithType(typeof(bool))
                .WithAttribute(new SwitchAttribute("f", "desc"))
                .Build();

            var spec = new ArgumentSpecification(field);
            var context = new ParseContext();

            Assert.IsTrue(_rule.CanMatch("-f", spec, context));
        }

        [Test]
        public void CanMatch_ShouldReturnFalse_IfNoSwitchAttribute()
        {
            var field = new FieldBuilder()
                .WithName("flag")
                .WithType(typeof(bool))
                .Build();

            var spec = new ArgumentSpecification(field);
            var context = new ParseContext();

            Assert.IsFalse(_rule.CanMatch("-f", spec, context));
        }

        [Test]
        public void CanMatch_ShouldReturnFalse_IfTokenDoesNotStartWithSingleDash()
        {
            var field = new FieldBuilder()
                .WithName("flag")
                .WithType(typeof(bool))
                .WithAttribute(new SwitchAttribute("f", "desc"))
                .Build();

            var spec = new ArgumentSpecification(field);
            var context = new ParseContext();

            // False because name after one dash doesn't match
            Assert.IsFalse(_rule.CanMatch("--f", spec, context));
        }
        
        [Test]
        public void CanMatch_ShouldReturnFalse_IfTokenDoesNotStartWithAnyDash()
        {
            var field = new FieldBuilder()
                .WithName("flag")
                .WithType(typeof(bool))
                .WithAttribute(new SwitchAttribute("f", "desc"))
                .Build();

            var spec = new ArgumentSpecification(field);
            var context = new ParseContext();

            // False because name after one dash doesn't match
            Assert.IsFalse(_rule.CanMatch("f", spec, context));
        }

        [Test]
        public void CanMatch_ShouldReturnFalse_IfTokenTooShort()
        {
            var field = new FieldBuilder()
                .WithName("f")
                .WithType(typeof(bool))
                .WithAttribute(new SwitchAttribute("f", "desc"))
                .Build();

            var spec = new ArgumentSpecification(field);
            var context = new ParseContext();

            Assert.IsFalse(_rule.CanMatch("-", spec, context));
        }

        [Test]
        public void CanMatch_ShouldReturnFalse_IfFieldTypeIsNotBool()
        {
            var field = new FieldBuilder()
                .WithName("flag")
                .WithType(typeof(int))
                .WithAttribute(new SwitchAttribute("f", "desc"))
                .Build();

            var spec = new ArgumentSpecification(field);
            var context = new ParseContext();

            Assert.IsFalse(_rule.CanMatch("-f", spec, context));
        }

        [Test]
        public void CanMatch_ShouldReturnFalse_IfFieldNameIsWrong()
        {
            var field = new FieldBuilder()
                .WithName("flag")
                .WithType(typeof(int))
                .WithAttribute(new SwitchAttribute("f", "desc"))
                .Build();

            var spec = new ArgumentSpecification(field);
            var context = new ParseContext();

            Assert.IsFalse(_rule.CanMatch("-a", spec, context));
        }
        
        [Test]
        public void CanMatch_ShouldReturnTrue_WithLongerName()
        {
            var field = new FieldBuilder()
                .WithName("flag")
                .WithType(typeof(bool))
                .WithAttribute(new SwitchAttribute("f2", "desc"))
                .Build();

            var spec = new ArgumentSpecification(field);
            var context = new ParseContext();

            Assert.IsTrue(_rule.CanMatch("-f2", spec, context));
        }
        
        [Test]
        public void TryParse_ShouldReturnTrueSuccess_WhenValidBoolToken()
        {
            var field = new FieldBuilder()
                .WithName("flag")
                .WithType(typeof(bool))
                .WithAttribute(new SwitchAttribute("f", "desc"))
                .Build();

            var spec = new ArgumentSpecification(field);
            var stream = new TokenStream(new List<string> { "true" });

            var success = _rule.TryParse(stream, spec, out var result);

            Assert.IsTrue(success);
            Assert.AreEqual(Status.Success, result.Status);
            Assert.AreEqual(true, result.Value);
        }

        [Test]
        public void TryParse_ShouldReturnTrueSuccess_WhenImpliedBoolToken()
        {
            var field = new FieldBuilder()
                .WithName("flag")
                .WithType(typeof(bool))
                .WithAttribute(new SwitchAttribute("f", "desc"))
                .Build();

            var spec = new ArgumentSpecification(field);
            var stream = new TokenStream(new List<string> { });

            var success = _rule.TryParse(stream, spec, out var result);

            Assert.IsTrue(success);
            Assert.AreEqual(Status.Success, result.Status);
            Assert.AreEqual(true, result.Value);
        }
        
        [Test]
        public void TryParse_ShouldReturnFalseSuccess_WhenFalseBoolToken()
        {
            var field = new FieldBuilder()
                .WithName("flag")
                .WithType(typeof(bool))
                .WithAttribute(new SwitchAttribute("f", "desc"))
                .Build();

            var spec = new ArgumentSpecification(field);
            var stream = new TokenStream(new List<string> { "FaLsE" });

            var success = _rule.TryParse(stream, spec, out var result);

            Assert.IsTrue(success);
            Assert.AreEqual(Status.Success, result.Status);
            Assert.AreEqual(false, result.Value);
        }
        
        [Test]
        public void TryParse_ShouldReturnFail_WhenInvalidBoolToken()
        {
            var field = new FieldBuilder()
                .WithName("flag")
                .WithType(typeof(bool))
                .WithAttribute(new SwitchAttribute("f", "desc"))
                .Build();

            var spec = new ArgumentSpecification(field);
            var stream = new TokenStream(new List<string> { "notabool" });

            var success = _rule.TryParse(stream, spec, out var result);

            Assert.IsFalse(success);
            Assert.AreEqual(Status.Fail, result.Status);
        }
        
        [Test]
        public void TryParse_ShouldReturnFail_WhenInvalidNumberBoolToken()
        {
            var field = new FieldBuilder()
                .WithName("flag")
                .WithType(typeof(bool))
                .WithAttribute(new SwitchAttribute("f", "desc"))
                .Build();

            var spec = new ArgumentSpecification(field);
            var stream = new TokenStream(new List<string> { "1" });

            var success = _rule.TryParse(stream, spec, out var result);

            Assert.IsFalse(success);
            Assert.AreEqual(Status.Fail, result.Status);
        }
    }
}