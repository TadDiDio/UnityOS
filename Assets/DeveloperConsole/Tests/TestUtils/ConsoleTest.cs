using NUnit.Framework;

namespace DeveloperConsole.Tests
{
    public abstract class ConsoleTest
    {
        [OneTimeSetUp]
        public void RestartKernelOnSetup()
        {
            KernelBootstrapper.Bootstrap();
        }
        
        [OneTimeTearDown]
        public void RestartKernelOnTearDown()
        {
            KernelBootstrapper.Bootstrap();
        }
    }
}