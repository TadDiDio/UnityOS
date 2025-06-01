using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace DeveloperConsole.Tests
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
            var handle1 = Kernel.Instance.Get<ICommandParser>();
            var handle2 = Kernel.Instance.Get<ICommandParser>();
            var handle3 = Kernel.Instance.Get<ICommandParser>();

            // Using proxies is okay because kernel is alive
            Assert.DoesNotThrow(() => handle1.Parse(new TokenStream(new List<string> {"test"})));
            Assert.DoesNotThrow(() => handle2.Parse(new TokenStream(new List<string> {"test"})));
            Assert.DoesNotThrow(() => handle3.Parse(new TokenStream(new List<string> {"test"})));
            
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
            Assert.Throws<InvalidOperationException>(() => Kernel.Instance.Get<ICommandParser>());
        }
        
        [Test]
        public void KernelService_StaleSystemsThrow()
        {
            KernelBootstrapper.Bootstrap();
            
            var handle = Kernel.Instance.Get<ICommandParser>();
            Assert.NotNull(handle);

            KernelBootstrapper.KillSystem();
            
            // Using a stale reference is not allowed and fails loudly
            Assert.Throws<ObjectDisposedException>(() => handle.Parse(new TokenStream(new List<string> {"test"})));
        }
    }
}