#nullable enable
using System;
using System.IO;
using UnityEngine;

namespace DeveloperConsole.IO
{
    /// <summary>
    /// Allows files to be read into the system.
    /// </summary>
    public class FileSource : IInputSource
    {
        public event Action<IInput> InputSubmitted = null!;
        private StreamReader _reader;

        
        /// <summary>
        /// Creates a new file source.
        /// </summary>
        /// <param name="filePathInAssets">The path to the file from assets.</param>
        public FileSource(string filePathInAssets)
        {
            _reader = new StreamReader(Path.Combine(Application.dataPath, filePathInAssets));
        }

        
        /// <summary>
        /// Reads and inputs a single line.
        /// </summary>
        public void ReadLine()
        {
            string? line = _reader.ReadLine();
            if (line != null) InputSubmitted?.Invoke(new TextInput(null, line));
            else _reader.Dispose();
        }

        
        /// <summary>
        /// Reads multiple lines. 
        /// </summary>
        /// <param name="count">The number of lines to read. All remaining if -1.</param>
        public void ReadLines(int count = -1)
        {
            int counter = 0;

            string? line;
            while ((line = _reader.ReadLine()) != null && counter++ != count)
            {
                InputSubmitted?.Invoke(new TextInput(null, line));
            }

            if (line == null)
            {
                _reader.Dispose();
            }
        }
    }
}