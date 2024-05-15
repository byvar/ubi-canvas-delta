using System;
using UnityEngine;
using ISystemLogger = UbiArt.ISystemLogger;

namespace UbiCanvas {
	public class UnitySystemLogger : ISystemLogger
	{
		public UbiArt.LogLevel MinLevel { get; set; } = UbiArt.LogLevel.Trace;

		public UnitySystemLogger() { }
		public UnitySystemLogger(UbiArt.LogLevel minLevel) {
			MinLevel = minLevel;
		}
		public void Log(UbiArt.LogLevel logLevel, object log, params object[] args) {
			if(logLevel < MinLevel) return;
			switch (logLevel) {
				case UbiArt.LogLevel.Error:
					Debug.LogError(String.Format(log?.ToString() ?? String.Empty, args));
					break;
				case UbiArt.LogLevel.Warning:
					Debug.LogWarning(String.Format(log?.ToString() ?? String.Empty, args));
					break;
				case UbiArt.LogLevel.Info:
					Debug.Log(String.Format(log?.ToString() ?? String.Empty, args));
					break;
			}
		}
	}
}