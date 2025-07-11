using System.Collections.Generic;

namespace DeveloperConsole.Core.Shell
{
    public class PersistentSessionState
    {
        // TODO: Set these via config. Makes sure to save updates to file so they are not overriden when file is read
        public int MaxOutputBufferSize = 250;
        public int MaxCommandHistorySize = 250;
        
        public List<string> OutputBuffer = new();
        public List<string> CommandHistory = new();
    }
}