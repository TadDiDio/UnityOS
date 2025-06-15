using System.Reflection;
using DeveloperConsole.Command;
using NUnit.Framework;

namespace DeveloperConsole.Tests.Command.Attributes
{
    public class RequiredAttributeTest : ConsoleTest
    {
        [OneTimeSetUp]
        public void Register()
        {
            AttributeBuilderRegistry.Register(new TestRequiredAttributeBuilder());
        }
        
        [Test]
        public void Validate_ReturnsFalse_WhenNotSet()
        {
            var attr = new RequiredAttribute();

            Assert.IsFalse(attr.Validate(null));
        }

        [Test]
        public void Validate_ReturnsTrue_AfterRecordCalled()
        {
            var attr = new RequiredAttribute();

            FieldInfo fieldInfo = new FieldBuilder()
                .WithName("testField")
                .WithType(typeof(bool))
                .Build();

            var argumentSpec = new ArgumentSpecification(fieldInfo);

            var context = new RecordingContext
            {
                ArgumentSpecification = argumentSpec,
                ArgumentValue = false,
                ParseTarget = null
            };

            attr.Record(context);

            Assert.IsTrue(attr.Validate(argumentSpec));
        }

        [Test]
        public void ErrorMessage_IncludesArgumentName()
        {
            var attr = new RequiredAttribute();

            FieldInfo fieldInfo = new FieldBuilder()
                .WithName("testField")
                .WithType(typeof(bool))
                .WithAttribute(attr)
                .Build();

            var argumentSpec = new ArgumentSpecification(fieldInfo);
    
            string expectedMessage = "Argument 'testField' must be explicitly set.";
            Assert.IsFalse(attr.Validate(argumentSpec));
            Assert.AreEqual(expectedMessage, attr.ErrorMessage());
        }
    }
}