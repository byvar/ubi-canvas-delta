using System.Collections.Generic;
using System.Linq;
using System.IO;
using Ionic.Zlib;
using System.Threading.Tasks;
using UbiCanvas.Helpers;
using System;

namespace UbiArt.Ghost {
	public class GhostFile : ICSerializable {
		public GhostFileHeader header = new GhostFileHeader();
		//public byte[] data;
		public GhostData data;

		public void Serialize(CSerializerObject s, string name) {
			header = s.SerializeObject<GhostFileHeader>(header, name: nameof(header));
			s.Goto(GhostFileHeader.HeaderSize);
			var size = (s.Length - s.CurrentPosition);
			if (header.isCompressed) {
				s.DoCompressed(() => {
					//data = s.SerializeBytes(data, header.uncompressedSize);
					data = s.SerializeObject<GhostData>(data, name: nameof(data));
				}, compressedSize: size, decompressedSize: header.uncompressedSize);
			} else {
				//data = s.SerializeBytes(data, size);
				data = s.SerializeObject<GhostData>(data, name: nameof(data));
			}
		}
	}
}
