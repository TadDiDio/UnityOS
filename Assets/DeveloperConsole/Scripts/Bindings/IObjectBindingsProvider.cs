using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace DeveloperConsole
{
    public interface IObjectBindingsProvider
    {
        public Dictionary<Type, Object> GetAllBindings();
        public bool TryGetBinding(Type type, string name, string tag, out Object obj);
        public Object ResolveBinding(Type objType, string name, string tag);
    }
}