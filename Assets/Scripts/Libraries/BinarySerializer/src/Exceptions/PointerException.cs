#nullable enable
using System;
using System.Collections.Generic;

namespace BinarySerializer
{
    public class PointerException : Exception 
    {
        public PointerException(string message, string excludeFromStackTrace) 
        {
            Message = message;
            ExcludeFromStackTrace = excludeFromStackTrace;
        }

        public override string Message { get; }
        private string ExcludeFromStackTrace { get; }

        public override string StackTrace 
        {
            get 
            {
                List<string> stackTrace = new();
                stackTrace.AddRange(base.StackTrace.Split(new[] { Environment.NewLine }, StringSplitOptions.None));
                stackTrace.RemoveAll(x => x.Contains(ExcludeFromStackTrace));
                return String.Join(Environment.NewLine, stackTrace.ToArray());
            }
        }
    }
}