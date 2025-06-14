using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.Reflection;
using DeveloperConsole.Command;
using DeveloperConsole.Parsing.Rules;
using DeveloperConsole.Parsing.Tokenizing;

namespace DeveloperConsole.Tests.Parsing.Rules
{
    public class BoolLongSwitchParseRuleTest : ConsoleTest
    {
        private BoolLongSwitchParseRule _rule;

        [SetUp]
        public void Setup()
        {
            _rule = new BoolLongSwitchParseRule();
        }

        private ArgumentSpecification CreateArgumentSpec(string name, Type fieldType, ArgumentAttribute attr = null)
        {
            var fieldBuilder = new FieldBuilder()
                .WithName(name)
                .WithType(fieldType);

            if (attr != null)
                fieldBuilder.WithAttribute(attr);

            FieldInfo fieldInfo = fieldBuilder.Build();

            return new ArgumentSpecification(fieldInfo);
        }

        [Test]
        public void CanMatch_ReturnsTrue_WhenTokenMatchesAndIsBoolSwitch()
        {
            var switchAttr = new SwitchAttribute("flag", "desc");
            var argument = CreateArgumentSpec("flag", typeof(bool), switchAttr);

            bool result = _rule.CanMatch("--flag", argument, null);
            Assert.IsTrue(result);
        }

        [Test]
        public void CanMatch_ReturnsFalse_WhenNoSwitchAttribute()
        {
            var argument = CreateArgumentSpec("flag", typeof(bool));

            bool result = _rule.CanMatch("--flag", argument, null);
            Assert.IsFalse(result);
        }

        [Test]
        public void CanMatch_ReturnsFalse_WhenTokenDoesNotStartWithDoubleDash()
        {
            var switchAttr = new SwitchAttribute("flag", "desc");
            var argument = CreateArgumentSpec("flag", typeof(bool), switchAttr);

            bool result = _rule.CanMatch("-flag", argument, null);
            Assert.IsFalse(result);
        }
        
        [Test]
        public void CanMatch_ReturnsFalse_WhenTokenDoesNotStartWithAnyDash()
        {
            var switchAttr = new SwitchAttribute("flag", "desc");
            var argument = CreateArgumentSpec("flag", typeof(bool), switchAttr);

            bool result = _rule.CanMatch("flag", argument, null);
            Assert.IsFalse(result);
        }

        [Test]
        public void CanMatch_ReturnsFalse_WhenTokenNameDoesNotMatchArgumentName()
        {
            var switchAttr = new SwitchAttribute("flag", "desc");
            var argument = CreateArgumentSpec("flag", typeof(bool), switchAttr);

            bool result = _rule.CanMatch("--notflag", argument, null);
            Assert.IsFalse(result);
        }

        [Test]
        public void CanMatch_ReturnsFalse_WhenFieldTypeIsNotBool()
        {
            var switchAttr = new SwitchAttribute("flag", "desc");
            var argument = CreateArgumentSpec("flag", typeof(int), switchAttr);

            bool result = _rule.CanMatch("--flag", argument, null);
            Assert.IsFalse(result);
        }

        [Test]
        public void CanMatch_ReturnsFalse_WhenTokenIsShort()
        {
            var switchAttr = new SwitchAttribute("flag", "desc");
            var argument = CreateArgumentSpec("flag", typeof(int), switchAttr);

            bool result = _rule.CanMatch("--", argument, null);
            Assert.IsFalse(result);
        }
        
        [Test]
        public void TryParse_ReturnsTrueAndSuccess_WhenTypeParseSucceeds()
        {
            // Assume TokenStream ctor takes string token
            var tokenStream = new TokenStream(new List<string> { "true" });
            var argument = CreateArgumentSpec("flag", typeof(bool));

            bool result = _rule.TryParse(tokenStream, argument, out var parseResult);

            Assert.IsTrue(result);
            Assert.AreEqual(Status.Success, parseResult.Status);
            Assert.AreEqual(true, parseResult.Value);
        }
        
        [Test]
        public void TryParse_ReturnsImpliedTrueAndSuccess_WhenTypeParseSucceeds()
        {
            // Assume TokenStream ctor takes string token
            var tokenStream = new TokenStream(new List<string> { });
            var argument = CreateArgumentSpec("flag", typeof(bool));

            bool result = _rule.TryParse(tokenStream, argument, out var parseResult);

            Assert.IsTrue(result);
            Assert.AreEqual(Status.Success, parseResult.Status);
            Assert.AreEqual(true, parseResult.Value);
        }

        [Test]
        public void TryParse_ReturnsFalseAndSuccess_WhenTypeParseSucceeds()
        {
            // Assume TokenStream ctor takes string token
            var tokenStream = new TokenStream(new List<string> { "False" });
            var argument = CreateArgumentSpec("flag", typeof(bool));

            bool result = _rule.TryParse(tokenStream, argument, out var parseResult);

            Assert.IsTrue(result);
            Assert.AreEqual(Status.Success, parseResult.Status);
            Assert.AreEqual(false, parseResult.Value);
        }
        
        [Test]
        public void TryParse_ReturnsFalseAndFailure_WhenTypeParseFails()
        {
            var tokenStream = new TokenStream(new List<string> { "notabool" });
            var argument = CreateArgumentSpec("flag", typeof(bool));

            bool result = _rule.TryParse(tokenStream, argument, out var parseResult);

            Assert.IsFalse(result);
            Assert.AreEqual(Status.Fail, parseResult.Status);
        }
    }
}