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

        private ArgumentSpecification CreateArgumentSpec(string name, Type fieldType, ArgumentAttribute attr)
        {
            var fieldBuilder = new FieldBuilder()
                .WithName(name)
                .WithType(fieldType)
                .WithAttribute(attr);

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
            var fieldInfo = new FieldBuilder()
                .WithName("flag")
                .WithType(typeof(bool))
                .Build();

            var argument = new ArgumentSpecification(fieldInfo);

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
            var switchAttr = new SwitchAttribute("flag", "desc");
            var tokenStream = new TokenStream(new List<string> { "true" });
            var argument = CreateArgumentSpec("flag", typeof(bool), switchAttr);

            var result = _rule.TryParse(tokenStream, argument);

            Assert.IsTrue(result.Status is Status.Success);
            Assert.AreEqual(true, result.Value);
        }
        
        [Test]
        public void TryParse_ReturnsImpliedTrueAndSuccess_WhenTypeParseSucceeds()
        {
            var switchAttr = new SwitchAttribute("flag", "desc");
            var tokenStream = new TokenStream(new List<string> { });
            var argument = CreateArgumentSpec("flag", typeof(bool), switchAttr);

            var result = _rule.TryParse(tokenStream, argument);

            Assert.IsTrue(result.Status is Status.Success);
            Assert.AreEqual(true, result.Value);
        }

        [Test]
        public void TryParse_ReturnsFalseAndSuccess_WhenTypeParseSucceeds()
        {
            var switchAttr = new SwitchAttribute("flag", "desc");
            var tokenStream = new TokenStream(new List<string> { "--flag", "False" });
            var argument = CreateArgumentSpec("flag", typeof(bool), switchAttr);

            var result = _rule.TryParse(tokenStream, argument);

            Assert.IsTrue(result.Status is Status.Success);
            Assert.AreEqual(false, result.Value);
        }
        
        [Test]
        public void TryParse_ReturnsFalseAndSuccess_WhenTypeParseFails()
        {
            var switchAttr = new SwitchAttribute("flag", "desc");
            var tokenStream = new TokenStream(new List<string> { "notabool" });
            var argument = CreateArgumentSpec("flag", typeof(bool), switchAttr);

            var result = _rule.TryParse(tokenStream, argument);

            Assert.AreEqual(Status.Success, result.Status);
            Assert.AreEqual(true, result.Value);
        }
    }
}