using DeveloperConsole.Core.Kernel;
using NUnit.Framework;

namespace DeveloperConsole.Tests.Core
{
    public class EditorBootstrapperTest
    {
        [OneTimeTearDown]
        public void RestartKernel()
        {
            ConsoleBootstrapper.Bootstrap();
        }
        
        [Test]
        public void EditorBootstrapper_BootstrapThenKill()
        {
            Assert.DoesNotThrow(ConsoleBootstrapper.Bootstrap);
            Assert.NotNull(Kernel.Instance);
            Assert.True(Kernel.IsInitialized);
            Assert.DoesNotThrow(ConsoleBootstrapper.Kill);
            Assert.False(Kernel.IsInitialized);
        }
        
        [Test]
        public void EditorBootstrapper_KillThenBootstrap()
        {
            Assert.DoesNotThrow(ConsoleBootstrapper.Kill);
            Assert.False(Kernel.IsInitialized);
            Assert.DoesNotThrow(ConsoleBootstrapper.Bootstrap);
            Assert.NotNull(Kernel.Instance);
            Assert.True(Kernel.IsInitialized);
        }
        
        [Test]
        public void EditorBootstrapper_MultipleKills()
        {
            Assert.DoesNotThrow(ConsoleBootstrapper.Kill);
            Assert.DoesNotThrow(ConsoleBootstrapper.Kill);
            Assert.DoesNotThrow(ConsoleBootstrapper.Kill);
            Assert.DoesNotThrow(ConsoleBootstrapper.Kill);
            Assert.False(Kernel.IsInitialized);
        }
        
        [Test]
        public void EditorBootstrapper_MultipleBootstraps()
        {
            Assert.DoesNotThrow(ConsoleBootstrapper.Bootstrap);
            Assert.DoesNotThrow(ConsoleBootstrapper.Bootstrap);
            Assert.DoesNotThrow(ConsoleBootstrapper.Bootstrap);
            Assert.DoesNotThrow(ConsoleBootstrapper.Bootstrap);
            Assert.True(Kernel.IsInitialized);
        }
    }
}