using System.Collections.Generic;
using DeveloperConsole.Parsing.Tokenizing;

namespace DeveloperConsole.Core.Shell
{
    public class AliasManager
    {
        private Dictionary<string, string> _aliasTable = new();

        /// <summary>
        /// Adds an alias.
        /// </summary>
        /// <param name="key">The alias to add.</param>
        /// <param name="value">The value of the alias.</param>
        public void AddAlias(string key, string value)
        {
            _aliasTable[key] = value;
        }


        /// <summary>
        /// Removes an alias.
        /// </summary>
        /// <param name="key">The alias to remove.</param>
        public void RemoveAlias(string key)
        {
            _aliasTable.Remove(key);
        }


        /// <summary>
        /// Gets all aliases.
        /// </summary>
        /// <returns>The alias table.</returns>
        public Dictionary<string, string> GetAliases() => _aliasTable;


        /// <summary>
        /// Replaces a token with its alias.
        /// </summary>
        /// <param name="token">The token to replace.</param>
        /// <param name="replaced">Tells if an alias was applied.</param>
        /// <returns>The tokenized alias or the token if it doesn't have one.</returns>
        public List<string> GetAlias(string token, out bool replaced)
        {
            if (_aliasTable.TryGetValue(token, out var value))
            {
                var result = ConsoleAPI.Parsing.Tokenize(value);

                replaced = result.Status is not TokenizationStatus.Empty;
                return replaced ? result.Tokens : new List<string> { token };
            }

            replaced = false;
            return new List<string> { token };
        }
    }
}
