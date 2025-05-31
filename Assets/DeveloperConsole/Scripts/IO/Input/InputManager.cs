using System;
using UnityEngine;
using System.Collections.Generic;

namespace DeveloperConsole
{
    public class InputManager : IInputManager
    {
        public event Action<string> InputSubmitted;
        public List<IInputSource> InputSources { get; } = new();
        
        public void RegisterInputSource(IInputSource inputSource)
        {
            if (InputSources.Contains(inputSource))
            {
                // TODO: Warning to console
                return;
            }
            
            InputSources.Add(inputSource);
        }

        public void UnregisterInputSource(IInputSource inputSource)
        {
            if (!InputSources.Contains(inputSource))
            {
                // TODO: Warning to console
                return;
            }
            
            InputSources.Remove(inputSource);
        }

        public void UnregisterAllInputSources() => InputSources.Clear();

        public void OnEventOccured(Event current)
        {
            foreach (IInputSource inputSource in InputSources)
            {
                if (inputSource.InputAvailable())
                {
                    InputSubmitted?.Invoke(inputSource.GetInput());
                }
            }
        }
    }
}