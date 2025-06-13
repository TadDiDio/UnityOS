using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace DeveloperConsole.Bindings
{
    /// <summary>
    /// Interface or object binding managers.
    /// </summary>
    public interface IObjectBindingsManager
    {
        /// <summary>
        /// Returns all current bindings.
        /// </summary>
        /// <returns>All current bindings.</returns>
        public Dictionary<Type, Object> GetAllBindings();
        
        
        /// <summary>
        /// Tries to get an object by checking if there is a binding, then searching for one if not.
        /// </summary>
        /// <param name="type">The type of object to get.</param>
        /// <param name="name">The name to find if there isn't an object already bound. Ignored if empty.</param>
        /// <param name="tag">The tag to find if there isn't an object already bound. Ignored if empty.</param>
        /// <param name="obj">The bound object.</param>
        /// <returns>True if a bound object was found in the cache or scene.</returns>
        public bool TryGetBinding(Type type, string name, string tag, out Object obj);
        
        
        /// <summary>
        /// Binds an object for commands to reference.
        /// </summary>
        /// <param name="objType">The type to bind.</param>
        /// <param name="name">The name to search for. Ignored if empty.</param>
        /// <param name="tag">The tag to search for. Ignored if empty.</param>
        /// <returns>The bound object or null if one wasn't found.</returns>
        public Object ResolveBinding(Type objType, string name, string tag);
    }
}