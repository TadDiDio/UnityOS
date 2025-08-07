using NUnit.Framework;

namespace DeveloperConsole.Tests
{
    public abstract class ConsoleTest
    {
        [OneTimeSetUp]
        public void RestartKernelOnSetup()
        {
            ConsoleBootstrapper.Bootstrap();
        }
        
        [OneTimeTearDown]
        public void RestartKernelOnTearDown()
        {
            ConsoleBootstrapper.Bootstrap();
        }
    }
}