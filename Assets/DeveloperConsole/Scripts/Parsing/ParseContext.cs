using System.Collections.Generic;

namespace DeveloperConsole.Parsing
{
    public class ParseContext
    {
        public IParseTarget Target { get; }
        private Dictionary<string, object> _metadata = new();

        public bool TryGetData<T>(string key, out T value)
        {
            value = default;
            
            if (!_metadata.TryGetValue(key, out object obj)) return false;
            if (obj is not T typedResult) return false;
            
            value = typedResult;
            return true;
        }

        public void SetData<T>(string key, T value)
        {
            _metadata[key] = value; 
        }
    }
}