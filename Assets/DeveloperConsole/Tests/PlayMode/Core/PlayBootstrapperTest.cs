using System.Collections;
using DeveloperConsole.Core.Kernel;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace DeveloperConsole.Tests.Core
{
    public class PlayBootstrapperTest
    {
        [OneTimeTearDown]
        public void RestartKernel()
        {
            KernelBootstrapper.Bootstrap();
        }
        
        [UnityTest]
        public IEnumerator PlayBootstrapper_BootstrapThenKill()
        {
            Assert.DoesNotThrow(KernelBootstrapper.Bootstrap);
            Assert.NotNull(Kernel.Instance);
            Assert.True(Kernel.IsInitialized);
            Assert.DoesNotThrow(KernelBootstrapper.KillSystem);
            yield return null;
            Assert.False(Kernel.IsInitialized);
        }

        [UnityTest]
        public IEnumerator PlayBootstrapper_KillThenBootstrap()
        {
            Assert.DoesNotThrow(KernelBootstrapper.KillSystem);
            Assert.False(Kernel.IsInitialized);
            Assert.DoesNotThrow(KernelBootstrapper.Bootstrap);
            Assert.NotNull(Kernel.Instance);
            Assert.True(Kernel.IsInitialized);
            yield return null;
        }
        
        [UnityTest]
        public IEnumerator PlayBootstrapper_MultipleKills()
        {
            Assert.DoesNotThrow(KernelBootstrapper.KillSystem);
            Assert.DoesNotThrow(KernelBootstrapper.KillSystem);
            Assert.DoesNotThrow(KernelBootstrapper.KillSystem);
            Assert.DoesNotThrow(KernelBootstrapper.KillSystem);
            Assert.False(Kernel.IsInitialized);
            yield return null;
        }
        
        [UnityTest]
        public IEnumerator PlayBootstrapper_MultipleBootstraps()
        {
            Assert.DoesNotThrow(KernelBootstrapper.Bootstrap);
            Assert.DoesNotThrow(KernelBootstrapper.Bootstrap);
            Assert.DoesNotThrow(KernelBootstrapper.Bootstrap);
            Assert.DoesNotThrow(KernelBootstrapper.Bootstrap);
            Assert.True(Kernel.IsInitialized);
            yield return null;
        }
    }
}