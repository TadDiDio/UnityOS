namespace DeveloperConsole
{
    using System;
    using System.Collections.Generic;

    public class UnityMainThreadDispatcher : Singleton<UnityMainThreadDispatcher>
    {
        private readonly Queue<Action> _actions = new();
        private readonly object _lock = new();

        public void Enqueue(Action action)
        {
            if (action == null) return;
            lock (_lock)
            {
                _actions.Enqueue(action);
            }
        }

        // Must be called each frame from your bootstrapper or some Update loop
        public void Update()
        {
            while (true)
            {
                Action action = null;
                lock (_lock)
                {
                    if (_actions.Count > 0)
                        action = _actions.Dequeue();
                }

                if (action == null)
                    break;

                action.Invoke();
            }
        }
    }
}
