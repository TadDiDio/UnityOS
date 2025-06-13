using System;

namespace DeveloperConsole.Core
{    
    public interface IKernelApplication : IDisposable
    {
        public void Tick();
    }
}