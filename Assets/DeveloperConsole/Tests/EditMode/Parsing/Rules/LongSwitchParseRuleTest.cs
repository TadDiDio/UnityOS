using System.Collections.Generic;
using DeveloperConsole.Command;
using DeveloperConsole.Parsing.Rules;
using DeveloperConsole.Parsing.Tokenizing;
using NUnit.Framework;
using UnityEngine;

namespace DeveloperConsole.Tests.EditMode.Parsing.Rules
{
    public class LongSwitchParseRuleTest
    {
        private IParseRule rule;

        private ArgumentSpecification stringArg;
        private ArgumentSpecification floatArg;
        
        [SetUp]
        public void Setup()
        {
            rule = new LongSwitchParseRule();
            
            var stringField = new FieldBuilder()
                .WithName("message")
                .WithType(typeof(string))
                .WithAttribute(new PositionalAttribute(0, "desc"))
                .Build();
            
            var floatField = new FieldBuilder()
                .WithName("number")
                .WithType(typeof(float))
                .WithAttribute(new SwitchAttribute('n', "desc"))
                .Build();
            
            stringArg = new ArgumentSpecification(stringField);
            floatArg = new ArgumentSpecification(floatField);
        }

        [Test]
        public void Filter_ShouldMatch()
        {
            ArgumentSpecification[] allArgs = { stringArg, floatArg };
            
            var result = rule.Filter("--number", allArgs, null);
            
            Assert.IsTrue(result.Length == 1);
            Assert.AreEqual(floatArg, result[0]);
            Assert.AreEqual(floatArg.Name, result[0].Name);
        }
        
        [Test]
        public void Filter_ShouldReturnNull_WhenMissingAttribute()
        {
            var field = new FieldBuilder()
                .WithName("flag")
                .WithType(typeof(bool))
                .Build();
            
            ArgumentSpecification flag = new(field);
            ArgumentSpecification[] allArgs = { flag, stringArg, floatArg };
            
            var result = rule.Filter("--flag", allArgs, null);
            
            Assert.IsNull(result);
        }
        
        [Test]
        public void Filter_ShouldReturnNull_WhenFlagDoesntStartWithDashes()
        {
            ArgumentSpecification[] allArgs = { stringArg, floatArg };
            
            var result = rule.Filter("number", allArgs, null);
            
            Assert.IsNull(result);
        }
        
        [Test]
        public void Filter_ShouldReturnNull_WhenFlagDoesntMatchName()
        {
            ArgumentSpecification[] allArgs = { stringArg, floatArg };
            
            var result = rule.Filter("--numbers", allArgs, null);
            
            Assert.IsNull(result);
        }
        
        [Test]
        public void Filter_ShouldReturnNull_WhenFlagIsOnlyDashes()
        {
            ArgumentSpecification[] allArgs = { stringArg, floatArg };
            
            var result = rule.Filter("--", allArgs, null);
            
            Assert.IsNull(result);
        }
        
        [Test]
        public void Filter_ShouldReturnNull_WhenArgIsNotSwitch()
        {
            ArgumentSpecification[] allArgs = { stringArg, floatArg };
            
            var result = rule.Filter("--message", allArgs, null);
            
            Assert.IsNull(result);
        }
        
        [Test]
        public void Apply_ShouldSucceed_WhenNumberIsValid()
        {
            ArgumentSpecification[] allArgs = { floatArg };
            List<string> tokens = new() { "--number", "-12" };
            TokenStream stream = new TokenStream(tokens);
            
            var result = rule.Apply(stream, allArgs);
            
            Assert.AreEqual(Status.Success, result.Status);
            Assert.AreEqual(-12, result.Values[floatArg]);
        }
        
        [Test]
        public void Apply_ShouldSucceed_WithMultipleTokenArg()
        {
            var field = new FieldBuilder()
                .WithName("position")
                .WithType(typeof(Vector2))
                .WithAttribute(new SwitchAttribute('p', "desc"))
                .Build();
            
            ArgumentSpecification position = new(field);
            ArgumentSpecification[] allArgs = { position };
            
            List<string> tokens = new() { "--position", "-1", "0" };
            TokenStream stream = new TokenStream(tokens);
            
            var result = rule.Apply(stream, allArgs);
            
            Assert.AreEqual(Status.Success, result.Status);
            Assert.AreEqual(new Vector2(-1, 0), result.Values[position]);
        }
        
        
        [Test]
        public void Apply_ShouldFail_WhenMultipleArgsPassed()
        {
            using SilentLogCapture log = new();
            
            var field = new FieldBuilder()
                .WithName("flag")
                .WithType(typeof(bool))
                .WithAttribute(new SwitchAttribute('f', "desc"))
                .Build();
            
            ArgumentSpecification flag = new(field);
            ArgumentSpecification[] allArgs = { flag, stringArg };

            List<string> tokens = new() { "--flag" };
            TokenStream stream = new TokenStream(tokens);
            
            var result = rule.Apply(stream, allArgs);
            Assert.AreEqual(Status.Fail, result.Status);
            Assert.AreEqual(1, log.Count(LogType.Error));
            Assert.IsTrue(log.HasLog(LogType.Error, "Too many arguments"));
        }
    }
}