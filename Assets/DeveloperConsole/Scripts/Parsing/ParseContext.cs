using System.Collections.Generic;

namespace DeveloperConsole.Parsing
{
    /// <summary>
    /// Meta data about the parse context. Can be used to store state between rules
    /// or between iterations of the same rule.
    /// </summary>
    public class ParseContext
    {
        /// <summary>
        /// The finalized parse target.
        /// </summary>
        public CommandParseTarget Target { get; }
        
        private Dictionary<string, object> _metadata = new();

        
        /// <summary>
        /// Creates a new parse context.
        /// </summary>
        /// <param name="target">The parse target.</param>
        public ParseContext(CommandParseTarget target)
        {
            Target = target;
        }
        
        
        /// <summary>
        /// Try to get data by the key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The data.</param>
        /// <typeparam name="T">The data's type.</typeparam>
        /// <returns>True if found.</returns>
        public bool TryGetData<T>(string key, out T value)
        {
            value = default;
            
            if (!_metadata.TryGetValue(key, out object obj)) return false;
            if (obj is not T typedResult) return false;
            
            value = typedResult;
            return true;
        }

        
        /// <summary>
        /// Stores or overrides the data at key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The data.</param>
        /// <typeparam name="T">The data's type.</typeparam>
        public void SetData<T>(string key, T value)
        {
            _metadata[key] = value; 
        }
    }
}