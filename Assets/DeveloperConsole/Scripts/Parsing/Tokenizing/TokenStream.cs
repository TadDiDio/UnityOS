using System.Collections.Generic;
using System.Linq;

namespace DeveloperConsole.Parsing.Tokenizing
{
    public class TokenStream
    {
        private readonly List<string> _tokens;
        private int _position;
        
        public TokenStream(List<string> tokens) => _tokens = tokens;
        public bool HasMore() => _position < _tokens.Count;
        public int Count() => _tokens.Count - _position;
        public string Peek() => HasMore() ? _tokens[_position] : null;
        public string Next() => HasMore() ? _tokens[_position++] : null;
        public IEnumerable<string> Remaining() => _tokens.Skip(_position);
        public IEnumerable<string> Read(int count)
        {
            var result = _tokens.Skip(_position).Take(count).ToList();
            _position += result.Count;
            return result;
        }
    }
}