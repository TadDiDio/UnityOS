using NUnit.Framework;

namespace DeveloperConsole.Tests
{
    public class ConsoleResetTest
    {
        [SetUp]
        public void ClearConsole()
        {
            StaticResetRegistry.ResetAll();
        }

        [OneTimeTearDown]
        public void Bootstrap()
        {
            StaticResetRegistry.ResetAll();
            ConsoleBootstrapper.Bootstrap();
        }
    }
}