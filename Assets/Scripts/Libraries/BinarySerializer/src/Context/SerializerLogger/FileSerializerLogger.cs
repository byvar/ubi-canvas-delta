using System;
using System.IO;

namespace BinarySerializer
{
    public class FileSerializerLogger : ISerializerLogger
    {
        public FileSerializerLogger(string logFile)
        {
            LogFile = logFile;
        }

        private bool _created;
        public virtual bool IsEnabled => true;

        private StreamWriter _logWriter;
        protected StreamWriter LogWriter => _logWriter ??= GetWriter();

        public string LogFile { get; }

        private StreamWriter GetWriter()
        {
            FileMode mode = _created ? FileMode.Append : FileMode.Create;
            _created = true;
            return new StreamWriter(File.Open(LogFile, mode, FileAccess.Write, FileShare.Read));
        }

        public void Log(object obj) => LogWriter.WriteLine(obj != null ? obj.ToString() : String.Empty);

        public void Dispose()
        {
            _logWriter?.Dispose();
            _logWriter = null;
        }
    }
}