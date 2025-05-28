using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DeveloperConsole
{
    public static class ConsoleInputManager
    {
        public delegate void InputEventHandler(string input);

        public static event InputEventHandler InputSubmitted;
        
        private static List<IConsoleInputSource> _inputMethods = new();

        static ConsoleInputManager()
        {
            ConsoleKernel.OnEventOccured += OnEventOccured;
            AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
        }

        private static void OnBeforeAssemblyReload()
        {
            ConsoleKernel.OnEventOccured -= OnEventOccured;
        }
        
        public static void RegisterInputMethod(IConsoleInputSource inputSource)
        {
            if (_inputMethods.Contains(inputSource))
            {
                // TODO: Warning to console
                return;
            }
            
            _inputMethods.Add(inputSource);
        }

        public static void UnregisterInputMethod(IConsoleInputSource inputSource)
        {
            if (!_inputMethods.Contains(inputSource))
            {
                // TODO: Warning to console
                return;
            }
            
            _inputMethods.Remove(inputSource);
        }

        public static void UnregisterAllInputMethods() => _inputMethods.Clear();

        private static void OnEventOccured(Event current)
        {
            foreach (IConsoleInputSource inputSource in _inputMethods)
            {
                if (inputSource.InputAvailable())
                {
                    InputSubmitted?.Invoke(inputSource.GetInput());
                }
            }
        }
    }
}