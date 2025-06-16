using System.Collections.Generic;
using DeveloperConsole.Command;
using DeveloperConsole.Parsing.Rules;
using DeveloperConsole.Parsing.Tokenizing;
using NUnit.Framework;

namespace DeveloperConsole.Tests.Parsing.Rules
{
    public class GroupedShortBoolSwitchRuleTest
    {
        private IParseRule rule;

        private ArgumentSpecification flagA;
        private ArgumentSpecification flagB;
        private ArgumentSpecification randomArg;
        
        [SetUp]
        public void Setup()
        {
            rule = new GroupedShortBoolSwitchRule();
            
            var fieldA = new FieldBuilder()
                .WithName("flagA")
                .WithType(typeof(bool))
                .WithAttribute(new SwitchAttribute('a', "desc"))
                .Build();
            
            var fieldB = new FieldBuilder()
                .WithName("flagB")
                .WithType(typeof(bool))
                .WithAttribute(new SwitchAttribute('b', "desc"))
                .Build();
            
            var random = new FieldBuilder()
                .WithName("random")
                .WithType(typeof(float))
                .WithAttribute(new PositionalAttribute(0, "desc"))
                .Build();
            
            flagA = new ArgumentSpecification(fieldA);
            flagB = new ArgumentSpecification(fieldB);
            randomArg = new ArgumentSpecification(random);
        }

        [Test]
        public void Filter_ShouldMatchTwo()
        {
            ArgumentSpecification[] allArgs = { flagA, flagB };
            
            var result = rule.Filter("-ab", allArgs, null);
            
            Assert.IsTrue(result.Length == 2);
            CollectionAssert.AreEqual(allArgs, result);
        }
        
        [Test]
        public void Filter_ShouldRejectOne()
        {
            ArgumentSpecification[] allArgs = { flagA };
            
            var result = rule.Filter("-a", allArgs, null);
            Assert.IsNull(result);
        }
        
        [Test]
        public void Filter_ShouldReturnNull_WhenOneMissingAttribute()
        {
            var field = new FieldBuilder()
                .WithName("flag")
                .WithType(typeof(bool))
                .Build();
            
            ArgumentSpecification flag = new(field);
            ArgumentSpecification[] allArgs = { flag, flagA, flagB };
            
            var result = rule.Filter("-abf", allArgs, null);
            
            Assert.IsNull(result);
        }
        
        [Test]
        public void Filter_ShouldReturnNull_WhenFlagDoesntStartWithDashes()
        {
            ArgumentSpecification[] allArgs = { flagA, flagB };
            
            var result = rule.Filter("ab", allArgs, null);
            
            Assert.IsNull(result);
        }
        
        [Test]
        public void Filter_ShouldReturnNull_WhenFlagDoesntMatchName()
        {
            ArgumentSpecification[] allArgs = { flagA, flagB };
            
            var result = rule.Filter("-ac", allArgs, null);
            
            Assert.IsNull(result);
        }
        
        [Test]
        public void Filter_ShouldReturnNull_WhenFlagIsOnlyDashes()
        {
            ArgumentSpecification[] allArgs = { flagA, flagB };
            
            var result = rule.Filter("-", allArgs, null);
            
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
            ArgumentSpecification[] allArgs = { flag, flagA, randomArg };
            
            var result = rule.Filter("-af", allArgs, null);
            
            Assert.IsNull(result);
        }
        
        [Test]
        public void Apply_ShouldSucceedReturnTrue()
        {
            ArgumentSpecification[] allArgs = { flagA, flagB };

            List<string> tokens = new() { "-ab" };
            TokenStream stream = new TokenStream(tokens);
            
            var result = rule.Apply(stream, allArgs);
            
            Assert.AreEqual(Status.Success, result.Status);
            Assert.AreEqual(true, result.Values[flagA]);
            Assert.AreEqual(true, result.Values[flagB]);
        }
        
        [Test]
        public void Apply_ShouldFailWhenNotBool()
        {
            var field = new FieldBuilder()
                .WithName("flag")
                .WithType(typeof(int))
                .WithAttribute(new SwitchAttribute('f', "desc"))
                .Build();
            
            ArgumentSpecification flag = new(field);
            ArgumentSpecification[] allArgs = { flag, flagA };
            
            List<string> tokens = new() { "-af" };
            TokenStream stream = new TokenStream(tokens);
            
            var result = rule.Apply(stream, allArgs);
            
            Assert.AreEqual(Status.Fail, result.Status);
            Assert.IsTrue(result.ErrorMessage.Contains("Parsing"));
        }
    }
}