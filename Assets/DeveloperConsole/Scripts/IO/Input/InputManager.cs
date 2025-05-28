#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.Collections.Generic;

namespace DeveloperConsole
{
    public static class InputManager
    {
        public delegate void InputEventHandler(string input);
        public static event InputEventHandler InputSubmitted;
        
        public static List<IConsoleInputSource> InputMethods { get; } = new();

        public static void Initialize()
        {
            StaticResetRegistry.Register(Reset);
            
            ConsoleKernel.OnEventOccured += OnEventOccured;
            
#if UNITY_EDITOR
            AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
#endif
        }

        private static void OnBeforeAssemblyReload()
        {
            ConsoleKernel.OnEventOccured -= OnEventOccured;
        }

        private static void Reset()
        {
            ConsoleKernel.OnEventOccured -= OnEventOccured;
            InputMethods.Clear();
            InputSubmitted = null;
        }
        
        public static void RegisterInputMethod(IConsoleInputSource inputSource)
        {
            if (InputMethods.Contains(inputSource))
            {
                // TODO: Warning to console
                return;
            }
            
            InputMethods.Add(inputSource);
        }

        public static void UnregisterInputMethod(IConsoleInputSource inputSource)
        {
            if (!InputMethods.Contains(inputSource))
            {
                // TODO: Warning to console
                return;
            }
            
            InputMethods.Remove(inputSource);
        }

        public static void UnregisterAllInputMethods()
        {
            InputMethods.Clear();
        }

        private static void OnEventOccured(Event current)
        {
            foreach (IConsoleInputSource inputSource in InputMethods)
            {
                if (inputSource.InputAvailable())
                {
                    InputSubmitted?.Invoke(inputSource.GetInput());
                }
            }
        }
    }
}