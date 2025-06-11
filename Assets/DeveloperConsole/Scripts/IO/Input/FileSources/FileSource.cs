#nullable enable
using System;
using System.IO;
using UnityEngine;

namespace DeveloperConsole
{
    public class FileSource : IInputSource
    {
        public event Action<string?> InputSubmitted = null!;
        private StreamReader _reader;

        public FileSource(string filePathInAssets)
        {
            _reader = new StreamReader(Path.Combine(Application.dataPath, filePathInAssets));
        }

        public void ReadLine()
        {
            string? line = _reader.ReadLine();
            if (line != null) InputSubmitted?.Invoke(line);
            else _reader.Dispose();
        }

        public void ReadLines(int count = -1)
        {
            int counter = 0;

            string? line;
            while ((line = _reader.ReadLine()) != null && counter++ != count)
            {
                InputSubmitted?.Invoke(line);
            }

            if (line == null)
            {
                _reader.Dispose();
            }
        }
    }
}