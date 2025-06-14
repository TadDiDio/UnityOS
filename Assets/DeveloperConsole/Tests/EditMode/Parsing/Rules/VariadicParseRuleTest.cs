using System.Collections.Generic;
using DeveloperConsole.Command;
using DeveloperConsole.Parsing;
using DeveloperConsole.Parsing.Rules;
using DeveloperConsole.Parsing.Tokenizing;
using NUnit.Framework;
using UnityEngine;

namespace DeveloperConsole.Tests.Parsing.Rules
{
    public class VariadicParseRuleTest
    {
        private VariadicParseRule _rule;
        private ParseContext _context;

        [SetUp]
        public void SetUp()
        {
            _rule = new VariadicParseRule();
            _context = new ParseContext(null);
        }

        [Test]
        public void CanMatch_True_ForListIntField()
        {
            var field = new FieldBuilder()
                .WithName("numbers")
                .WithType(typeof(List<int>))
                .WithAttribute(new VariadicAttribute("desc"))
                .Build();

            var spec = new ArgumentSpecification(field);

            Assert.IsTrue(_rule.CanMatch("1", spec, _context));
        }

        [Test]
        public void CanMatch_False_ForArrayField()
        {
            var field = new FieldBuilder()
                .WithName("numbers")
                .WithType(typeof(int[]))
                .WithAttribute(new VariadicAttribute("desc"))
                .Build();

            var spec = new ArgumentSpecification(field);

            Assert.IsFalse(_rule.CanMatch("1", spec, _context));
        }

        [Test]
        public void TryParse_Succeeds_ForListOfInts()
        {
            var field = new FieldBuilder()
                .WithName("numbers")
                .WithType(typeof(List<int>))
                .WithAttribute(new VariadicAttribute("desc"))
                .Build();

            var spec = new ArgumentSpecification(field);
            var stream = new TokenStream(new List<string> { "1", "2", "3", "4" });

            var success = _rule.TryParse(stream, spec, out var result);

            Assert.IsTrue(success);
            Assert.AreEqual(Status.Success, result.Status);
            CollectionAssert.AreEqual(new List<int> { 1, 2, 3, 4 }, (List<int>)result.Value);
        }

        [Test]
        public void TryParse_Succeeds_ForListOfStrings()
        {
            var field = new FieldBuilder()
                .WithName("names")
                .WithType(typeof(List<string>))
                .WithAttribute(new VariadicAttribute("desc"))
                .Build();

            var spec = new ArgumentSpecification(field);
            var stream = new TokenStream(new List<string> { "Alice", "Bob", "Charlie" });

            var success = _rule.TryParse(stream, spec, out var result);

            Assert.IsTrue(success);
            Assert.AreEqual(Status.Success, result.Status);
            CollectionAssert.AreEqual(new List<string> { "Alice", "Bob", "Charlie" }, (List<string>)result.Value);
        }

        [Test]
        public void TryParse_Succeeds_ForListOfVector2()
        {
            var field = new FieldBuilder()
                .WithName("points")
                .WithType(typeof(List<Vector2>))
                .WithAttribute(new VariadicAttribute("desc"))
                .Build();

            var spec = new ArgumentSpecification(field);
            var stream = new TokenStream(new List<string> { "0", "0", "1", "1", "2", "2" });

            var success = _rule.TryParse(stream, spec, out var result);

            Assert.IsTrue(success);
            Assert.AreEqual(Status.Success, result.Status);
            var points = (List<Vector2>)result.Value;

            Assert.AreEqual(3, points.Count);
            Assert.AreEqual(new Vector2(0, 0), points[0]);
            Assert.AreEqual(new Vector2(1, 1), points[1]);
            Assert.AreEqual(new Vector2(2, 2), points[2]);
        }

        [Test]
        public void TryParse_Fails_IfTokenCannotBeParsed()
        {
            var field = new FieldBuilder()
                .WithName("numbers")
                .WithType(typeof(List<int>))
                .WithAttribute(new VariadicAttribute("desc"))
                .Build();

            var spec = new ArgumentSpecification(field);
            var stream = new TokenStream(new List<string> { "1", "two", "3" });

            var success = _rule.TryParse(stream, spec, out var result);

            Assert.IsFalse(success);
            Assert.AreEqual(Status.Fail, result.Status);
        }
    }
}