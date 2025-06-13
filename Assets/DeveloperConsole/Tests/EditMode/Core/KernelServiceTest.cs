using System;
using DeveloperConsole.Core;
using DeveloperConsole.Parsing;
using NUnit.Framework;

namespace DeveloperConsole.Tests.Core
{
    public class KernelServiceTest
    {
        #region TEST TYPES
        private class TestClass { }
        public interface ITestInterface { }
        #endregion 
        
        [Test]
        public void KernelService_GetServices()
        {
            KernelBootstrapper.Bootstrap();
            
            Assert.NotNull(Kernel.Instance);
            
            // Valid services - should always return proxy but throw during GetLiveInstance().
            var handle1 = Kernel.Instance.Get<IParser>();
            var handle2 = Kernel.Instance.Get<IParser>();
            var handle3 = Kernel.Instance.Get<IParser>();

            // Using proxies is okay because kernel is alive
            // TODO: 
            // Assert.DoesNotThrow(() => handle1.Parse(new TokenStream(new List<string> {"test"})));
            // Assert.DoesNotThrow(() => handle2.Parse(new TokenStream(new List<string> {"test"})));
            // Assert.DoesNotThrow(() => handle3.Parse(new TokenStream(new List<string> {"test"})));
            
            Assert.NotNull(handle1);
            Assert.NotNull(handle2);
            Assert.NotNull(handle3);
            Assert.AreSame(handle1, handle2);
            Assert.AreSame(handle2, handle3);
            
            // Invalid queries
            Assert.Throws<ArgumentException>(() => Kernel.Instance.Get<TestClass>());
            Assert.Throws<ArgumentException>(() => Kernel.Instance.Get<ITestInterface>());
        }
        
        [Test]
        public void KernelService_KernelKilled()
        {
            KernelBootstrapper.KillSystem();
            Assert.Throws<InvalidOperationException>(() => Kernel.Instance.Get<IParser>());
        }
        
        [Test]
        public void KernelService_StaleSystemsThrow()
        {
            KernelBootstrapper.Bootstrap();
            
            var handle = Kernel.Instance.Get<IParser>();
            Assert.NotNull(handle);

            KernelBootstrapper.KillSystem();
            
            // Using a stale reference is not allowed and fails loudly
            // TODO:
            //Assert.Throws<ObjectDisposedException>(() => handle.Parse(new TokenStream(new List<string> {"test"})));
        }
    }
}