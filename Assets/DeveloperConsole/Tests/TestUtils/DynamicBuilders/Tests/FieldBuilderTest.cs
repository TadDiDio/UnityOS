using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DeveloperConsole.Command;
using NUnit.Framework;
using UnityEngine;

namespace DeveloperConsole.Tests.TestUtils
{
    public class FieldBuilderTest
    {
        [Test]
        public void FieldBuilder_DefaultNameAndType()
        {
            var field = new FieldBuilder().Build();

            Assert.NotNull(field);
            Assert.AreEqual("test", field.Name);
            Assert.AreEqual(typeof(int), field.FieldType);
        }

        [Test]
        public void FieldBuilder_CustomNameAndType()
        {
            var field = new FieldBuilder()
                .WithName("flag")
                .WithType(typeof(bool))
                .Build();

            Assert.NotNull(field);
            Assert.AreEqual("flag", field.Name);
            Assert.AreEqual(typeof(bool), field.FieldType);
        }

        [Test]
        public void FieldBuilder_SwitchAttributeOnField()
        {
            var attr = new SwitchAttribute("f", "Enable the flag");

            var field = new FieldBuilder()
                .WithName("flag")
                .WithType(typeof(bool))
                .WithAttribute(attr)
                .Build();

            var actualAttr = field.GetCustomAttribute<SwitchAttribute>();
            Assert.NotNull(actualAttr);
            Assert.AreEqual("f", actualAttr.ShortName);
            Assert.AreEqual(CommandMetaProcessor.Description("Enable the flag"), actualAttr.Description);
        }

        [Test]
        public void FieldBuilder_MultipleAttributes()
        {
            var attr1 = new SwitchAttribute("x", "desc1");
            var attr2 = new SwitchAttribute("y", "desc2");

            var field = new FieldBuilder()
                .WithName("multi")
                .WithType(typeof(string))
                .WithAttribute(attr1)
                .WithAttribute(attr2) // same type, but should both attach
                .Build();

            var attributes = field.GetCustomAttributes<SwitchAttribute>();
            Assert.That(attributes.Count(), Is.EqualTo(2));
        }
        
        [Test]
        public void FieldBuilder_Positional()
        {
            var field = new FieldBuilder()
                .WithName("position")
                .WithType(typeof(int))
                .WithAttribute(new PositionalAttribute(0, "desc"))
                .Build();

            var attr = field.GetCustomAttribute<PositionalAttribute>();
            Assert.That(attr, Is.Not.Null);
            Assert.That(attr.Index, Is.EqualTo(0));
        }

        [Test]
        public void FieldBuilder_Variadic()
        {
            var field = new FieldBuilder()
                .WithName("args")
                .WithType(typeof(List<string>))
                .WithAttribute(new VariadicAttribute("desc", isCommandPath: true))
                .Build();

            var attr = field.GetCustomAttribute<VariadicAttribute>();
            Assert.That(attr, Is.Not.Null);
            Assert.That(attr.IsCommandPath, Is.True);
        }

        [Test]
        public void FieldBuilder_ReturnsNullIfBadAttribute()
        {
            using SilentLogCapture log = new();
            
            var badAttr = new CustomFakeAttribute();

            var builder = new FieldBuilder().WithAttribute(badAttr);
            Assert.IsNull(builder);
            Assert.AreEqual(log.Count(LogType.Error), 1);
            Assert.True(log.HasLog(LogType.Error, "could not be built"));
        }

        private class CustomFakeAttribute : ArgumentAttribute
        {
            public CustomFakeAttribute() : base("bad", "desc") { }
        }
    }
}