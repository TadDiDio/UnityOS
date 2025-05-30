using System;

namespace DeveloperConsole
{
    public abstract class Singleton<T> where T : class
    {
        private static T _instance;

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
        
        public static bool IsInitialized => _instance != null;

        public static void Initialize(Func<T> factory)
        {
            if (_instance != null)
            {
                throw new InvalidOperationException($"{typeof(T).Name} has already been initialized.");
            }

            _instance = factory() ?? throw new InvalidOperationException($"{typeof(T).Name} factory returned null.");
        }

        public static void Reset()
        {
            _instance = null;
        }
    }
}