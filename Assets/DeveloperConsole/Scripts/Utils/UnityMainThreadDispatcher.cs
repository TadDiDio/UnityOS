using System;
using System.Collections.Generic;

namespace DeveloperConsole
{
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

        public void Update()
        {
            while (true)
            {
                Action action = null;

                lock (_lock)
                {
                    if (_actions.Count > 0) action = _actions.Dequeue();
                }

                if (action == null) break;

                action.Invoke();
            }
        }
    }
}
