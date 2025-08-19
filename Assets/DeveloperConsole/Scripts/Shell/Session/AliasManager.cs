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
        /// Tries getting an alias for the key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="alias">The alias if there is one.</param>
        /// <returns>True if there was an alias.</returns>
        public bool TryGetAlias(string key, out string alias)
        {
            return _aliasTable.TryGetValue(key, out alias);
        }
    }
}
