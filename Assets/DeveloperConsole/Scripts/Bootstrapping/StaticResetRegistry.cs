using System;
using System.Collections.Generic;
using UnityEngine;

namespace DeveloperConsole
{
    public static class StaticResetRegistry
    {
        private static readonly List<Action> _resetActions = new();

        public static void Register(Action resetAction)
        {
            if (!_resetActions.Contains(resetAction))
                _resetActions.Add(resetAction);
        }

        public static void ResetAll()
        {
            foreach (var action in _resetActions)
            {
                try
                {
                    action.Invoke();
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Static reset failed: {ex}");
                }
            }
        }

        public static void Clear()
        {
            _resetActions.Clear();
        }
    }
}