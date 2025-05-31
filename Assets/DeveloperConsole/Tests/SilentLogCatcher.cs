using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DeveloperConsole.Tests
{
    public sealed class SilentLogCapture : IDisposable
    {
        public struct CapturedLog
        {
            public LogType Type;
            public string Message;
            public string StackTrace;
        }

        private class CapturingLogHandler : ILogHandler
        {
            private readonly ILogHandler fallback;
            private readonly List<CapturedLog> logs;

            public CapturingLogHandler(ILogHandler fallback, List<CapturedLog> logs)
            {
                this.fallback = fallback;
                this.logs = logs;
            }

            public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
            {
                logs.Add(new CapturedLog
                {
                    Type = logType,
                    Message = string.Format(format, args),
                    StackTrace = Environment.StackTrace
                });

                // Do NOT forward to fallback — suppresses actual output
            }

            public void LogException(Exception exception, UnityEngine.Object context)
            {
                logs.Add(new CapturedLog
                {
                    Type = LogType.Exception,
                    Message = exception.Message,
                    StackTrace = exception.StackTrace
                });

                // Suppress original output
            }
        }

        private readonly ILogHandler _originalHandler;
        private readonly List<CapturedLog> _capturedLogs = new();

        public IReadOnlyList<CapturedLog> Captured => _capturedLogs;

        public SilentLogCapture()
        {
            _originalHandler = Debug.unityLogger.logHandler;
            Debug.unityLogger.logHandler = new CapturingLogHandler(_originalHandler, _capturedLogs);
        }

        public int Count(LogType type)
        {
            return _capturedLogs.Count(log => log.Type == type);
        }

        public bool HasLog(LogType type, string contains)
        {
            foreach (var log in _capturedLogs)
            {
                if (log.Type == type && log.Message.Contains(contains)) return true;
            }

            return false;
        }
        
        public void Dispose()
        {
            Debug.unityLogger.logHandler = _originalHandler;
        }
    }
}