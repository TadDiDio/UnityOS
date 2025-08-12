using System.Collections.Generic;

namespace DeveloperConsole.Persistence
{
    public class PersistentHistoryContainer
    {
        public List<string> History;

        public PersistentHistoryContainer(List<string> history)
        {
            History = history;
        }
    }
}
