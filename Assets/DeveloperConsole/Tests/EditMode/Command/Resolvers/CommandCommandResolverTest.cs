using DeveloperConsole.Command;
using NUnit.Framework;

namespace DeveloperConsole.Tests.Command.Resolvers
{
    public class CommandCommandResolverTest
    {

        [Test]
        public void Resolve_ReturnsSuccess_WithExpectedCommand()
        {
            var command = new CommandBuilder()
                .WithName("mock")
                .Build();

            var resolver = new CommandCommandResolver(command);

            var result = resolver.Resolve(null);

            Assert.IsTrue(result.Status == CommandResolutionStatus.Success);
            Assert.AreSame(command, result.Command);
        }
    }
}
