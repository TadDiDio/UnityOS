using System;

namespace DeveloperConsole
{
    /// <summary>
    /// A utility singleton class.
    /// </summary>
    /// <typeparam name="T">The type to make a singleton.</typeparam>
    public abstract class Singleton<T> where T : class
    {
        private static T _instance;

        
        /// <summary>
        /// Tells if the singleton is initialized.
        /// </summary>
        public static bool IsInitialized => _instance != null;
        
        
        /// <summary>
        /// Gets a reference to the singleton instance.
        /// </summary>
        /// <exception cref="InvalidOperationException">Throws if accessed before the singleton is initialized.</exception>
        public static T Instance
        {
            get
            {
                if (_instance == null)
                    throw new InvalidOperationException($"{typeof(T).Name} has not been initialized. Call " +
                                                        $"{nameof(Initialize)} first.");
                return _instance;
            }
        }

   
        /// <summary>
        /// Initializes the singleton/
        /// </summary>
        /// <param name="factory">The factory to make the new instance.</param>
        /// <exception cref="InvalidOperationException">Throws if the singleton is already initialized.</exception>
        public static void Initialize(Func<T> factory)
        {
            if (_instance != null)
            {
                throw new InvalidOperationException($"{typeof(T).Name} has already been initialized.");
            }

            _instance = factory() ?? throw new InvalidOperationException($"{typeof(T).Name} factory returned null.");
        }

        
        /// <summary>
        /// Destroyes the singleton.
        /// </summary>
        public static void Reset()
        {
            _instance = null;
        }
    }
}