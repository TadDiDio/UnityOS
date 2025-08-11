using System;
using System.Collections.ObjectModel;
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
        public ReadOnlyDictionary<Type, object> GetAllBindings();


        /// <summary>
        /// Binds an object for future commands to use.
        /// </summary>
        /// <param name="type">The type to bind for.</param>
        /// <param name="obj">The object instance to bind to.</param>
        public void BindObject(Type type, object obj);


        /// <summary>
        /// Tries to get a Unity object by searching the scene with the matching parameters.
        /// </summary>
        /// <param name="type">The type of object to get.</param>
        /// <param name="name">The name to find if there isn't an object already bound. Ignored if empty.</param>
        /// <param name="tag">The tag to find if there isn't an object already bound. Ignored if empty.</param>
        /// <param name="obj">The bound object.</param>
        /// <returns>True if a bound object was found in the cache or scene.</returns>
        public bool TryGetUnityObjectBinding(Type type, string name, string tag, out Object obj);


        /// <summary>
        /// Tries to get a plain C# object from the cache.
        /// </summary>
        /// <param name="type">The type to get.</param>
        /// <param name="obj">The bound object.</param>
        /// <returns>True if a cached object exists.</returns>
        public bool TryGetPlainCSharpBinding(Type type, out object obj);
    }
}
