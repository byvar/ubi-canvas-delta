using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace UbiArt {
	public class ConversionSettings {
		public Context OldContext { get; set; }

		public Settings OldSettings { get; set; }

		public WwiseConversionSettings WwiseConversionSettings { get; set; } = new WwiseConversionSettings();

		public Action<Context, ConversionSettings, TextureCooked> TextureConversion { get; set; }


		#region Path Conversion
		public List<IPathConversionRule> PathConversionRules { get; set; } = new List<IPathConversionRule>();
		public bool LockPaths { get; set; } = false;
		public HashSet<Path> LockedPaths { get; set; } = new HashSet<Path>(new IdentityComparer<Path>());

		private sealed class IdentityComparer<T> : IEqualityComparer<T>
			where T : class {
			public bool Equals(T x, T y) => ReferenceEquals(x, y);
			public int GetHashCode(T obj) => RuntimeHelpers.GetHashCode(obj);
		}

		public void ConvertPath(Path p) {
			if(LockedPaths != null && LockedPaths.Contains(p)) return;
			if (PathConversionRules != null) {
				foreach (var rule in PathConversionRules) {
					rule.Apply(p);
				}
			}
			if (OldSettings?.EngineVersion == EngineVersion.RO) {
				if (!p.IsNull && string.IsNullOrEmpty(p.FullPath)) p.FullPath = "";
			}
			if (LockPaths) LockedPaths.Add(p);
		}
		#endregion
	}
}
