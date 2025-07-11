using System;
using System.Reflection;
using DeveloperConsole.Command;
using DeveloperConsole.Core.Shell;
using NUnit.Framework;

namespace DeveloperConsole.Tests.Command.Resolvers
{
    public class TextCommandResolverTest : ConsoleTest
    {
        private ShellSession session;

        [SetUp]
        public void Setup()
        {
            session = new ShellSession();
        }

        [Test]
        public void Resolve_ShouldFail_OnEmptyInput()
        {
            var resolver = new TextCommandResolver("");

            var result = resolver.Resolve(session);

            Assert.AreEqual(Status.Fail, result.Status);
            Assert.AreEqual("", result.ErrorMessage);
        }

        [Test]
        public void Resolve_ShouldFail_WhenCommandNotFound()
        {
            var resolver = new TextCommandResolver("notarealcommand");

            var result = resolver.Resolve(session);

            Assert.AreEqual(Status.Fail, result.Status);
            Assert.IsTrue(result.ErrorMessage.Contains("Could not find a command"));
        }

        [Test]
        public void Resolve_ShouldFail_WhenParseFails()
        {
            Type mock = new CommandBuilder()
                .WithName("mock")
                .BuildType();
            
            ConsoleAPI.Commands.RegisterCommand(mock);
            
            var resolver = new TextCommandResolver("mock unexpected_token");
            
            var result = resolver.Resolve(session);

            Assert.AreEqual(Status.Fail, result.Status);
            Assert.IsTrue(result.ErrorMessage.Contains("unexpected token"));
        }

        [Test]
        public void Resolve_ShouldSucceed_WhenCommandParsesCorrectly()
        {
            Type mock = new CommandBuilder()
                .WithName("greet")
                .WithPositional("name", typeof(string))
                .BuildType();
            
            ConsoleAPI.Commands.RegisterCommand(mock);

            var resolver = new TextCommandResolver("greet Alice");

            var result = resolver.Resolve(session);

            Assert.AreEqual(Status.Success, result.Status);
            Assert.IsNotNull(result.Command);
            Assert.AreEqual("greet", result.Command.GetType().GetCustomAttribute<CommandAttribute>().Name);
        }
    }
}