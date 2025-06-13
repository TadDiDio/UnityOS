using System;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using DeveloperConsole.Bindings;
using DeveloperConsole.Command;
using DeveloperConsole.Parsing;
using DeveloperConsole.Windowing;

namespace DeveloperConsole.Core
{
    // TODO: Make this not a singleton, inject it only where classes need it, everything else goes through the API.
    public class Kernel : Singleton<Kernel>, IDisposable
    {
        private ConsoleRuntimeDependencies _dependencies;
        private List<IKernelApplication> _applications = new();
        private readonly Dictionary<Type, object> _proxies = new();
        private readonly Dictionary<Type, object> _serviceMap = new();

        public Kernel(RuntimeDependenciesFactory config)
        {
            _dependencies = config.Create();
            
            if (!_serviceMap.TryAdd(typeof(IWindowManager), _dependencies.WindowManager) ||
                !_serviceMap.TryAdd(typeof(ITypeParserRegistryProvider), _dependencies.TypeParserRegistry) ||
                !_serviceMap.TryAdd(typeof(ICommandRegistry), _dependencies.CommandRegistry) ||
                !_serviceMap.TryAdd(typeof(IParser), _dependencies.Parser) ||
                !_serviceMap.TryAdd(typeof(IObjectBindingsManager), _dependencies.ObjectBindingsManager))
            {
                Log.Warning("Service already existed and will not be replaced.");
            }
        }
        public void RegisterApplication(IKernelApplication kernelApplication)
        {
            if (_applications.Contains(kernelApplication))
            {
                // TODO: Console error
                return; 
            }
            _applications.Add(kernelApplication);
        }
        public void UnregisterApplication(IKernelApplication kernelApplication)
        {
            if (!_applications.Contains(kernelApplication))
            {
                // TODO: Console error
                return; 
            }
            _applications.Remove(kernelApplication);
        }

        // Client-facing: wraps services in dynamically created interfaces to avoid reference caching problems.
        // This ensures that when the kernel dies, so do all its systems even if clients cache references.
        /// <summary>
        /// Gets a proxy to a kernel service.
        /// </summary>
        /// <typeparam name="T">The type to get. T must be an interface.</typeparam>
        /// <returns>The proxy</returns>
        /// <exception cref="ObjectDisposedException">Accessing after the kernel is killed throws exception.</exception>
        public T Get<T>() where T : class
        {
            if (_dependencies == null) throw new ObjectDisposedException(nameof(Kernel), "Kernel has been disposed.");
            if (!_serviceMap.ContainsKey(typeof(T))) throw new ArgumentException($"Type {typeof(T)} is not a service.");
            if (_proxies.TryGetValue(typeof(T), out var existing)) return (T)existing;

            // Create a transparent proxy for T backed by kernel
            var proxy = DispatchProxy.Create<T, KernelServiceProxy<T>>();
            ((KernelServiceProxy<T>)(object)proxy).Setup(this);

            _proxies.Add(typeof(T), proxy);
            return proxy;
        }

        // Called by proxies to get the live service instance from _deps
        public T GetLiveInstance<T>() where T : class
        {
            if (_dependencies == null) return null;

            if (_serviceMap.TryGetValue(typeof(T), out var service)) return (T)service;

            return null;
        }

        public void Tick()
        {
            foreach (var application in _applications) application.Tick();
        }

        public void OnInput(Event current)
        {
            // TODO: Send this only to input consumers
            _dependencies.WindowManager.OnInput(current);
        }
        public void OnDraw(int screenWidth, int screenHeight, bool sceneView)
        {
            int padding = 10; // TODO: Move padding elsewhere
            Rect screenRect = new(padding, padding, screenWidth - 2 * padding, screenHeight - 2 * padding);
            _dependencies.WindowManager.OnGUI(screenRect);
        }
        
        public void Dispose()
        {
            foreach (var application in _applications) application.Dispose();
            _dependencies = null;
            _proxies.Clear();
        }
    }
}

