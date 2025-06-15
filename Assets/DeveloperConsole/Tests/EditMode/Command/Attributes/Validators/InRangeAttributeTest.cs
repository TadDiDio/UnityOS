using System;
using System.Linq;
using DeveloperConsole.Command;
using NUnit.Framework;

namespace DeveloperConsole.Tests.Command.Attributes
{
    public class InRangeAttributeTest : ConsoleTest
    {
        private ArgumentSpecification BuildArgWithInRangeAttribute(string name, Type type, float min, float max)
        {
            AttributeBuilderRegistry.Register(new TestInRangeAttributeBuilder());
            var field = new FieldBuilder()
                .WithName(name)
                .WithType(type)
                .WithAttribute(new InRangeAttribute(min, max))
                .Build();

            return new ArgumentSpecification(field);
        }

        private RecordingContext CreateContext(object value, ArgumentSpecification spec)
        {
            return new RecordingContext
            {
                ArgumentValue = value,
                ArgumentSpecification = spec,
                ParseTarget = null // Not needed for validation tests
            };
        }

        [Test]
        public void Validate_Passes_ForFloatWithinRange()
        {
            var spec = BuildArgWithInRangeAttribute("health", typeof(float), 0, 100);
            var attr = spec.Attributes.OfType<InRangeAttribute>().First();

            attr.Record(CreateContext(50f, spec));
            Assert.IsTrue(attr.Validate(spec));
        }

        [Test]
        public void Validate_Passes_ForIntWithinRange()
        {
            var spec = BuildArgWithInRangeAttribute("offset", typeof(int), -10, 10);
            var attr = spec.Attributes.OfType<InRangeAttribute>().First();

            attr.Record(CreateContext(3, spec));
            Assert.IsTrue(attr.Validate(spec));
        }

        [Test]
        public void Validate_Fails_WhenBelowMin()
        {
            var spec = BuildArgWithInRangeAttribute("health", typeof(float), 0, 100);
            var attr = spec.Attributes.OfType<InRangeAttribute>().First();

            attr.Record(CreateContext(-5f, spec));
            Assert.IsFalse(attr.Validate(spec));
            Assert.AreEqual("Value -5 is not in the range [0, 100].", attr.ErrorMessage());
        }

        [Test]
        public void Validate_Fails_WhenAboveMax()
        {
            var spec = BuildArgWithInRangeAttribute("health", typeof(float), 0, 100);
            var attr = spec.Attributes.OfType<InRangeAttribute>().First();

            attr.Record(CreateContext(150f, spec));
            Assert.IsFalse(attr.Validate(spec));
            Assert.AreEqual("Value 150 is not in the range [0, 100].", attr.ErrorMessage());
        }

        [Test]
        public void Validate_Fails_WhenNonNumeric()
        {
            var spec = BuildArgWithInRangeAttribute("bogus", typeof(object), 0, 100);
            var attr = spec.Attributes.OfType<InRangeAttribute>().First();

            attr.Record(CreateContext("not-a-number", spec));
            Assert.IsFalse(attr.Validate(spec));
        }

        [Test]
        public void Validate_Passes_ExactlyAtMin()
        {
            var spec = BuildArgWithInRangeAttribute("edge", typeof(float), 0, 10);
            var attr = spec.Attributes.OfType<InRangeAttribute>().First();

            attr.Record(CreateContext(0f, spec));
            Assert.IsTrue(attr.Validate(spec));
        }

        [Test]
        public void Validate_Passes_ExactlyAtMax()
        {
            var spec = BuildArgWithInRangeAttribute("edge", typeof(float), 0, 10);
            var attr = spec.Attributes.OfType<InRangeAttribute>().First();

            attr.Record(CreateContext(10f, spec));
            Assert.IsTrue(attr.Validate(spec));
        }
    }
}