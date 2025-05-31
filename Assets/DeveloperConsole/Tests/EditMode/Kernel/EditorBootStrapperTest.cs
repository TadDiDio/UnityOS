using NUnit.Framework;

namespace DeveloperConsole.Tests
{
    public class EditorBootstrapperTest
    {
        [OneTimeTearDown]
        public void RestartKernel()
        {
            KernelBootstrapper.Bootstrap();
        }
        
        [Test]
        public void EditorBootstrapper_BootstrapThenKill()
        {
            Assert.DoesNotThrow(KernelBootstrapper.Bootstrap);
            Assert.NotNull(Kernel.Instance);
            Assert.True(Kernel.IsInitialized);
            Assert.DoesNotThrow(KernelBootstrapper.KillSystem);
            Assert.False(Kernel.IsInitialized);
        }
        
        [Test]
        public void EditorBootstrapper_KillThenBootstrap()
        {
            Assert.DoesNotThrow(KernelBootstrapper.KillSystem);
            Assert.False(Kernel.IsInitialized);
            Assert.DoesNotThrow(KernelBootstrapper.Bootstrap);
            Assert.NotNull(Kernel.Instance);
            Assert.True(Kernel.IsInitialized);
        }
        
        [Test]
        public void EditorBootstrapper_MultipleKills()
        {
            Assert.DoesNotThrow(KernelBootstrapper.KillSystem);
            Assert.DoesNotThrow(KernelBootstrapper.KillSystem);
            Assert.DoesNotThrow(KernelBootstrapper.KillSystem);
            Assert.DoesNotThrow(KernelBootstrapper.KillSystem);
            Assert.False(Kernel.IsInitialized);
        }
        
        [Test]
        public void EditorBootstrapper_MultipleBootstraps()
        {
            Assert.DoesNotThrow(KernelBootstrapper.Bootstrap);
            Assert.DoesNotThrow(KernelBootstrapper.Bootstrap);
            Assert.DoesNotThrow(KernelBootstrapper.Bootstrap);
            Assert.DoesNotThrow(KernelBootstrapper.Bootstrap);
            Assert.True(Kernel.IsInitialized);
        }
    }
}