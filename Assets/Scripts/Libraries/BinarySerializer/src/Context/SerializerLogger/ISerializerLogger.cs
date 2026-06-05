#nullable enable
using System;

namespace BinarySerializer
{
    public interface ISerializerLogger : IDisposable
    {
        bool IsEnabled { get; }
        void Log(object? obj);
    }
}