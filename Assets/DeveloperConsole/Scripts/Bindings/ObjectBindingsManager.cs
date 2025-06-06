using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DeveloperConsole
{
    public class ObjectBindingsManager : IObjectBindingsProvider
    {
        private Dictionary<Type, Object> _bindings = new();

        public Dictionary<Type, Object> GetAllBindings() => _bindings;

        public bool TryGetBinding(Type type, string name, string tag, out Object obj)
        {
            var success = _bindings.TryGetValue(type, out obj);
            if (obj)
            {
                Debug.Log("Found object");
                return true;
            }
            
            if (success) _bindings.Remove(type);

            obj = ResolveBinding(type, name, tag);
            return obj;
        }
            
        // TODO BUG: Also tags being null messes something up, try bind Player -n Player in console.
        public Object ResolveBinding(Type objType, string name, string tag)
        {
            bool found = _bindings.TryGetValue(objType, out Object current);

            // If the cached value is null, remove it
            if (found && !current)
            {
                _bindings.Remove(objType);
            }
            
            // TODO: I think this only searches active scene, add support for all open scenes.
            var allObjects = Object.FindObjectsByType(objType, FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            if (allObjects == null) return null;

            var result = allObjects
                .OrderBy(obj =>
                {
                    bool nameMatches = !string.IsNullOrEmpty(name) && obj.name == name;
                    bool tagMatches = !string.IsNullOrEmpty(tag) && obj switch
                    {
                        GameObject go => go.CompareTag(tag),
                        Component comp => comp.CompareTag(tag),
                        _ => false
                    };

                    if (nameMatches && tagMatches) return 0;
                    if (tagMatches) return 1;
                    if (nameMatches) return 2;
                    return 3;
                })
                .FirstOrDefault();

            if (result) _bindings[objType] = result;
            return result;
        }
    }
}