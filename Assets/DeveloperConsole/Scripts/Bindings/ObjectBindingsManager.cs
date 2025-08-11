using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DeveloperConsole.Bindings
{
    public class ObjectBindingsManager : IObjectBindingsManager
    {
        private Dictionary<Type, object> _bindings = new();

        public ReadOnlyDictionary<Type, object> GetAllBindings()
        {
            return new ReadOnlyDictionary<Type, object>(_bindings);
        }

        public void BindObject(Type type, object obj)
        {
            Log.Info("Here");
            _bindings[type] = obj;
        }

        public bool TryGetPlainCSharpBinding(Type type, out object obj) => _bindings.TryGetValue(type, out obj);
        public bool TryGetUnityObjectBinding(Type type, string name, string tag, out Object obj)
        {
            var found = Object.FindObjectsByType(type, FindObjectsInactive.Include, FindObjectsSortMode.None);

            obj = found.OrderByDescending(o => HasTag(o, tag))  // Prioritize tags
                .ThenByDescending(o => HasName(o, name))        // Then names
                .ThenByDescending(IsActive)                     // Then active in hierarchy
                .FirstOrDefault();

            return obj;
        }


        private static bool HasTag(Object o, string tag)
        {
            if (string.IsNullOrEmpty(tag)) return false;

            return o switch
            {
                GameObject go => go.CompareTag(tag),
                Component comp => comp.gameObject.CompareTag(tag),
                _ => false
            };
        }

        private static bool HasName(Object o, string name)
        {
            if (string.IsNullOrEmpty(name)) return false;
            return o switch
            {
                GameObject go => go.name == name,
                Component comp => comp.gameObject.name == name,
                _ => false
            };
        }

        private static bool IsActive(Object o)
        {
            return o switch
            {
                GameObject go => go.activeInHierarchy,
                Component comp => comp.gameObject.activeInHierarchy,
                _ => false
            };
        }
    }
}
