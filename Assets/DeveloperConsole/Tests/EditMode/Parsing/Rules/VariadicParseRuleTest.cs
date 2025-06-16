using System.Collections.Generic;
using DeveloperConsole.Command;
using DeveloperConsole.Parsing.Rules;
using DeveloperConsole.Parsing.Tokenizing;
using NUnit.Framework;
using UnityEngine;

namespace DeveloperConsole.Tests.Parsing.Rules
{
    public class VariadicParseRuleTest
    {
        private IParseRule rule;

        private ArgumentSpecification flag;
        private ArgumentSpecification number;
        
        [SetUp]
        public void Setup()
        {
            rule = new VariadicParseRule();
            
            var flagField = new FieldBuilder()
                .WithName("flag")
                .WithType(typeof(bool))
                .WithAttribute(new SwitchAttribute('o', "desc"))
                .Build();
            
            var numberField = new FieldBuilder()
                .WithName("number")
                .WithType(typeof(float))
                .WithAttribute(new PositionalAttribute(0, "desc"))
                .Build();
            
            flag = new ArgumentSpecification(flagField);
            number = new ArgumentSpecification(numberField);
        }

        [Test]
        public void Filter_ShouldMatch()
        {
            var field = new FieldBuilder()
                .WithName("nums")
                .WithType(typeof(List<int>))
                .WithAttribute(new VariadicAttribute("desc"))
                .Build();
            
            ArgumentSpecification nums = new(field);
            ArgumentSpecification[] allArgs = { nums, flag, number };
            
            var result = rule.Filter("1", allArgs, null);
            
            Assert.IsTrue(result.Length == 1);
            Assert.AreEqual(nums, result[0]);
            Assert.AreEqual(nums.Name, result[0].Name);
        }
        
        [Test]
        public void Filter_ShouldReturnNull_WhenMissingAttribute()
        {
            var field = new FieldBuilder()
                .WithName("nums")
                .WithType(typeof(List<int>))
                .Build();
            
            ArgumentSpecification nums = new(field);
            ArgumentSpecification[] allArgs = { nums, flag, number };
            
            var result = rule.Filter("1", allArgs, null);
            
            Assert.IsNull(result);
        }
        
        [Test]
        public void Filter_ShouldReturnNull_WhenArgIsNotVariadic()
        {
            ArgumentSpecification[] allArgs = { flag, number };
            
            var result = rule.Filter("-f", allArgs, null);
            
            Assert.IsNull(result);
        }
        
        [Test]
        public void Apply_ShouldSucceedReturnTrue_WhenWellFormed()
        {
            var field = new FieldBuilder()
                .WithName("nums")
                .WithType(typeof(List<int>))
                .WithAttribute(new VariadicAttribute("desc"))
                .Build();
            
            ArgumentSpecification nums = new(field);
            ArgumentSpecification[] allArgs = { nums };

            List<string> tokens = new() { "-1", "2" };
            TokenStream stream = new TokenStream(tokens);
            
            var result = rule.Apply(stream, allArgs);
            
            Assert.AreEqual(Status.Success, result.Status);
            CollectionAssert.AreEqual(new List<int> {-1, 2}, (List<int>)result.Values[nums]);
        }
        
        [Test]
        public void Apply_ShouldSucceedReturnTrue_WithMultiTokenTypes()
        {
            var field = new FieldBuilder()
                .WithName("nums")
                .WithType(typeof(List<Vector2>))
                .WithAttribute(new VariadicAttribute("desc"))
                .Build();
            
            ArgumentSpecification nums = new(field);
            ArgumentSpecification[] allArgs = { nums };

            List<string> tokens = new() { "-1", "2", "0", "1" };
            TokenStream stream = new TokenStream(tokens);
            
            var result = rule.Apply(stream, allArgs);
            
            Assert.AreEqual(Status.Success, result.Status);
            CollectionAssert.AreEqual(new List<Vector2> {new(-1, 2), new (0, 1)}, (List<Vector2>)result.Values[nums]);
        }
        
        [Test]
        public void Apply_ShouldFail_WhenBadType()
        {
            var field = new FieldBuilder()
                .WithName("nums")
                .WithType(typeof(List<int>))
                .WithAttribute(new VariadicAttribute("desc"))
                .Build();
            
            ArgumentSpecification nums = new(field);
            ArgumentSpecification[] allArgs = { nums };

            List<string> tokens = new() { "-1", "a" };
            TokenStream stream = new TokenStream(tokens);
            
            var result = rule.Apply(stream, allArgs);
            
            Assert.AreEqual(Status.Fail, result.Status);
            Assert.IsTrue(result.ErrorMessage.Contains("Parsing"));
        }
        
        [Test]
        public void Apply_ShouldFail_WhenMultipleArgsPassed()
        {
            using SilentLogCapture log = new();
            
            var field = new FieldBuilder()
                .WithName("nums")
                .WithType(typeof(List<int>))
                .WithAttribute(new VariadicAttribute("desc"))
                .Build();
            
            ArgumentSpecification nums = new(field);
            ArgumentSpecification[] allArgs = { flag, nums };

            List<string> tokens = new() { "-f", "1", "2" };
            TokenStream stream = new TokenStream(tokens);
            
            var result = rule.Apply(stream, allArgs);
            Assert.AreEqual(Status.Fail, result.Status);
            Assert.AreEqual(1, log.Count(LogType.Error));
            Assert.IsTrue(log.HasLog(LogType.Error, "Too many arguments"));
        }
    }
}