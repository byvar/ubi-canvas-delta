
using System;

namespace UbiArt {
	public interface IMemoryData {
		public bool TryGetSize(Type type, out uint size);
		public bool TryGetAlign(Type type, out uint align);
	}
}