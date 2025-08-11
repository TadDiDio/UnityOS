using System;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using DeveloperConsole.Bindings;
using DeveloperConsole.Command;
using DeveloperConsole.Core.Shell;
using DeveloperConsole.Parsing;
using DeveloperConsole.Parsing.TypeAdapting;
using DeveloperConsole.Windowing;

namespace DeveloperConsole.Core.Kernel
{
    // TODO: Make this not a singleton, inject it only where classes need it, everything else goes through the API.

    /// <summary>
    /// The kernel and center of the entire system. Handles distributing ticks, inputs, and draw calls.
    /// </summary>
    public class Kernel : Singleton<Kernel>, IDisposable
    {
        private DependenciesContainer _dependencies;
        private List<IKernelApplication> _applications = new();
        private readonly Dictionary<Type, object> _proxies = new();
        private readonly Dictionary<Type, object> _serviceMap = new();


        /// <summary>
        /// Creates a new kernel.
        /// </summary>
        /// <param name="config">The injected dependency container.</param>
        public Kernel(DependenciesFactory config)
        {
            _dependencies = config.Create();

            RegisterCoreServices(_dependencies);
            RegisterCoreApplications(_dependencies);
        }

        private void RegisterCoreServices(DependenciesContainer dependencies)
        {
            TryAddService(typeof(IShellApplication), dependencies.Shell);
            TryAddService(typeof(ICommandExecutor), dependencies.CommandExecutor);
            TryAddService(typeof(ICommandRegistry), dependencies.CommandRegistry);
            TryAddService(typeof(ITypeAdapterRegistry), dependencies.TypeAdapterRegistry);
            TryAddService(typeof(ICommandParser), dependencies.CommandParser);
            TryAddService(typeof(IObjectBindingsManager), dependencies.ObjectBindingsManager);
            TryAddService(typeof(IWindowManager), dependencies.WindowManager);
        }

        private void TryAddService(Type type, object service)
        {
            if (!_serviceMap.TryAdd(type, service))
            {
                Log.Warning($"Service for {type.Name} already existed and will not be replaced.");
            }
        }

        private void RegisterCoreApplications(DependenciesContainer dependencies)
        {
            // TODO: Will probably include window manager here.
            RegisterApplication(dependencies.Shell);
        }

        /// <summary>
        /// Registers an application for consistent ticking.
        /// </summary>
        /// <param name="kernelApplication">The application.</param>
        public void RegisterApplication(IKernelApplication kernelApplication)
        {
            if (_applications.Contains(kernelApplication))
            {
                Log.Error($"Attempted to register a duplicate kernel application: {kernelApplication.GetType().Name}");
                return;
            }
            _applications.Add(kernelApplication);
        }


        /// <summary>
        /// Removes an application from being consistently ticked.
        /// </summary>
        /// <param name="kernelApplication">The application.</param>
        public void UnregisterApplication(IKernelApplication kernelApplication)
        {
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

        /// <summary>
        /// Proxies use this to get a live service from the dependency container.
        /// </summary>
        /// <typeparam name="T">The type of service to get.</typeparam>
        /// <returns>The service.</returns>
        public T GetLiveInstance<T>() where T : class
        {
            if (_dependencies == null) return null;

            if (_serviceMap.TryGetValue(typeof(T), out var service)) return (T)service;

            return null;
        }


        /// <summary>
        /// Makes the kernel tick all applications.
        /// </summary>
        public void Tick()
        {
            foreach (var application in _applications) application.Tick();
        }


        /// <summary>
        /// Makes the kernel distribute an input event.
        /// </summary>
        /// <param name="current"></param>
        public void OnInput(Event current)
        {
            _dependencies.WindowManager.OnInput(current);
        }


        /// <summary>
        /// Makes the kernel distribute a draw call.
        /// </summary>
        /// <param name="isSceneView">Whether this is the scene or game view.</param>
        public void OnDraw(bool isSceneView)
        {
            _dependencies.WindowManager.OnGUI(isSceneView);
        }


        /// <summary>
        /// Destroys all applications and services.
        /// </summary>
        public void Dispose()
        {
            foreach (var application in _applications.ToList()) UnregisterApplication(application);
            foreach (var application in _applications) application.Dispose();
            _dependencies = null;
            _proxies.Clear();
        }
    }
}

