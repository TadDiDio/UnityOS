using System.Collections.Generic;
using DeveloperConsole.Command;
using DeveloperConsole.Parsing.Rules;
using DeveloperConsole.Parsing.Tokenizing;
using NUnit.Framework;
using UnityEngine;

namespace DeveloperConsole.Tests.Parsing.Rules
{
    public class BoolLongSwitchParseRuleTest : ConsoleTest
    {
        private IParseRule rule;

        private ArgumentSpecification otherFlag;
        private ArgumentSpecification randomArg;
        
        [SetUp]
        public void Setup()
        {
            rule = new BoolLongSwitchParseRule();
            
            var otherBool = new FieldBuilder()
                .WithName("flag2")
                .WithType(typeof(bool))
                .WithAttribute(new SwitchAttribute('o', "desc"))
                .Build();
            
            var random = new FieldBuilder()
                .WithName("random")
                .WithType(typeof(float))
                .WithAttribute(new PositionalAttribute(0, "desc"))
                .Build();
            
            otherFlag = new ArgumentSpecification(otherBool);
            randomArg = new ArgumentSpecification(random);
        }

        [Test]
        public void Filter_ShouldMatch()
        {
            var field = new FieldBuilder()
                .WithName("flag")
                .WithType(typeof(bool))
                .WithAttribute(new SwitchAttribute('f', "desc"))
                .Build();
            
            ArgumentSpecification flag = new(field);
            ArgumentSpecification[] allArgs = { flag, otherFlag, randomArg };
            
            var result = rule.Filter("--flag", allArgs, null);
            
            Assert.IsTrue(result.Length == 1);
            Assert.AreEqual(flag, result[0]);
            Assert.AreEqual(flag.Name, result[0].Name);
        }
        
        [Test]
        public void Filter_ShouldReturnNull_WhenMissingAttribute()
        {
            var field = new FieldBuilder()
                .WithName("flag")
                .WithType(typeof(bool))
                .Build();
            
            ArgumentSpecification flag = new(field);
            ArgumentSpecification[] allArgs = { flag, otherFlag, randomArg };
            
            var result = rule.Filter("--flag", allArgs, null);
            
            Assert.IsNull(result);
        }
        
        [Test]
        public void Filter_ShouldReturnNull_WhenFlagDoesntStartWithDashes()
        {
            var field = new FieldBuilder()
                .WithName("flag")
                .WithType(typeof(bool))
                .WithAttribute(new SwitchAttribute('f', "desc"))
                .Build();
            
            ArgumentSpecification flag = new(field);
            ArgumentSpecification[] allArgs = { flag, otherFlag, randomArg };
            
            var result = rule.Filter("flag", allArgs, null);
            
            Assert.IsNull(result);
        }
        
        [Test]
        public void Filter_ShouldReturnNull_WhenFlagDoesntMatchName()
        {
            var field = new FieldBuilder()
                .WithName("flag")
                .WithType(typeof(bool))
                .WithAttribute(new SwitchAttribute('f', "desc"))
                .Build();
            
            ArgumentSpecification flag = new(field);
            ArgumentSpecification[] allArgs = { flag, otherFlag, randomArg };
            
            var result = rule.Filter("--flags", allArgs, null);
            
            Assert.IsNull(result);
        }
        
        [Test]
        public void Filter_ShouldReturnNull_WhenFlagIsOnlyDashes()
        {
            var field = new FieldBuilder()
                .WithName("flag")
                .WithType(typeof(bool))
                .WithAttribute(new SwitchAttribute('f', "desc"))
                .Build();
            
            ArgumentSpecification flag = new(field);
            ArgumentSpecification[] allArgs = { flag, otherFlag, randomArg };
            
            var result = rule.Filter("--", allArgs, null);
            
            Assert.IsNull(result);
        }
        
        [Test]
        public void Filter_ShouldReturnNull_WhenFieldIsNotBool()
        {
            var field = new FieldBuilder()
                .WithName("flag")
                .WithType(typeof(int))
                .WithAttribute(new SwitchAttribute('f', "desc"))
                .Build();
            
            ArgumentSpecification flag = new(field);
            ArgumentSpecification[] allArgs = { flag, otherFlag, randomArg };
            
            var result = rule.Filter("--flag", allArgs, null);
            
            Assert.IsNull(result);
        }
        
        [Test]
        public void Filter_ShouldReturnNull_WhenArgIsNotSwitch()
        {
            ArgumentSpecification[] allArgs = { otherFlag, randomArg };
            
            var result = rule.Filter("--flag", allArgs, null);
            
            Assert.IsNull(result);
        }
        
        [Test]
        public void Apply_ShouldSucceedReturnTrue_WhenBoolIsTrue()
        {
            var field = new FieldBuilder()
                .WithName("flag")
                .WithType(typeof(bool))
                .WithAttribute(new SwitchAttribute('f', "desc"))
                .Build();
            
            ArgumentSpecification flag = new(field);
            ArgumentSpecification[] allArgs = { flag };

            List<string> tokens = new() { "--flag", "true" };
            TokenStream stream = new TokenStream(tokens);
            
            var result = rule.Apply(stream, allArgs);
            
            Assert.AreEqual(Status.Success, result.Status);
            Assert.AreEqual(true, result.Values[flag]);
        }
        
        [Test]
        public void Apply_ShouldSucceedReturnFalse_WhenBoolIsFalse()
        {
            var field = new FieldBuilder()
                .WithName("flag")
                .WithType(typeof(bool))
                .WithAttribute(new SwitchAttribute('f', "desc"))
                .Build();
            
            ArgumentSpecification flag = new(field);
            ArgumentSpecification[] allArgs = { flag };

            List<string> tokens = new() { "--flag", "false" };
            TokenStream stream = new TokenStream(tokens);
            
            var result = rule.Apply(stream, allArgs);
            
            Assert.AreEqual(Status.Success, result.Status);
            Assert.AreEqual(false, result.Values[flag]);
        }
        
        [Test]
        public void Apply_ShouldSucceedReturnTrue_WhenBoolIsImplied()
        {
            var field = new FieldBuilder()
                .WithName("flag")
                .WithType(typeof(bool))
                .WithAttribute(new SwitchAttribute('f', "desc"))
                .Build();
            
            ArgumentSpecification flag = new(field);
            ArgumentSpecification[] allArgs = { flag };

            List<string> tokens = new() { "--flag" };
            TokenStream stream = new TokenStream(tokens);
            
            var result = rule.Apply(stream, allArgs);
            
            Assert.AreEqual(Status.Success, result.Status);
            Assert.AreEqual(true, result.Values[flag]);
        }
        
        [Test]
        public void Apply_ShouldSucceedReturnTrue_WhenBoolIsImpliedWithAnotherToken()
        {
            var field = new FieldBuilder()
                .WithName("flag")
                .WithType(typeof(bool))
                .WithAttribute(new SwitchAttribute('f', "desc"))
                .Build();
            
            ArgumentSpecification flag = new(field);
            ArgumentSpecification[] allArgs = { flag };

            List<string> tokens = new() { "--flag", "token" };
            TokenStream stream = new TokenStream(tokens);
            
            var result = rule.Apply(stream, allArgs);
            
            Assert.AreEqual(Status.Success, result.Status);
            Assert.AreEqual(true, result.Values[flag]);
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
            ArgumentSpecification[] allArgs = { flag, otherFlag };

            List<string> tokens = new() { "--flag" };
            TokenStream stream = new TokenStream(tokens);
            
            var result = rule.Apply(stream, allArgs);
            Assert.AreEqual(Status.Fail, result.Status);
            Assert.AreEqual(1, log.Count(LogType.Error));
            Assert.IsTrue(log.HasLog(LogType.Error, "Too many arguments"));
        }
    }
}