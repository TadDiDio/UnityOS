using System.Collections.Generic;
using UnityEngine;

namespace DeveloperConsole
{
    public class TerminalInputBuffer
    {
        public string CurrentBuffer = "";

        private int _index;
        private List<string> _history = new();

        public void PushHistory()
        {
            bool isEmpty = string.IsNullOrEmpty(CurrentBuffer);
            bool isIdenticalEntry = _history.Count > 0 && _history[^1].Equals(CurrentBuffer);

            if (!isEmpty && !isIdenticalEntry)
            {
                _history.Add(CurrentBuffer);
            }

            _index = _history.Count;
            CurrentBuffer = "";
        }

        public void LessRecent()
        {
            if (_history.Count == 0) return;

            if (!IndexingHistory && !string.IsNullOrEmpty(CurrentBuffer))
            {
                PushHistory();
                _index--;
            }

            _index = Mathf.Max(0, _index - 1);
            CurrentBuffer = _history[_index];
        }

        public void MoreRecent()
        {
            if (_history.Count == 0) return;
            if (!IndexingHistory) return;

            _index = Mathf.Min(_history.Count - 1, _index + 1);
            CurrentBuffer = _history[_index];
        }

        private bool IndexingHistory => _index < _history.Count;
    }
}
