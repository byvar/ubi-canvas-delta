using System;
using System.Text;
using UbiArt.FileFormat;

namespace UbiArt {
	public class CSerializerLoadInPlace : CSerializerObjectBinary {
		public CSerializerLoadInPlace(Context context, IBinaryLowLevelSerializer serializer) : base(context, serializer) {}
		public CSerializerLoadInPlace(Context context, BinaryFile file, BinaryMode mode) : base(context, file, mode) {}

		public override SerializerProperties Properties => base.Properties | SerializerProperties.LIP; // 0x19

		public uint SerializerMemCount { get; set; }
		public uint ExpectedMemCount { get; set; }
		public long? SerializerMemCountPosition { get; set; }

		#region Object Size
		private void AlignMemCount(uint align) {
			var rest = (SerializerMemCount % align);
			if(rest > 0) SerializerMemCount += align - rest;
		}

		protected override void IncreaseMemCountForString(string s, Encoding encoding) {
			if (string.IsNullOrEmpty(s)) return;
			var length = encoding.GetByteCount(s);
			if (length == 0) return;
			AlignMemCount(1);
			SerializerMemCount += (uint)length + 1;
			//Log("SerializerMemCount: {0}. Increased by {1} for string {2}", SerializerMemCount, length + 1, s);		}
		}
		public override void InitSerializer() {
			base.InitSerializer();

			SerializerMemCountPosition = CurrentPosition;
			SerializerMemCount = Serialize<uint>(SerializerMemCount, name: "SerializerMemCount");
			ExpectedMemCount = SerializerMemCount;
			SerializerMemCount = 0; // Reset it here
		}
		public override void SerializeObjectSize(Type type) {
			base.SerializeObjectSize(type);
			uint sizeOf = 0;

			if(Mode == BinaryMode.Read)
				sizeOf = Serialize<uint>(sizeOf, name: "sizeof");

			if (Settings.MemoryData != null) {
				if (Settings.MemoryData.TryGetSize(type, out var size)) {
					if (Mode == BinaryMode.Read) {
						if (sizeOf != size) {
							Context?.SystemLogger?.LogWarning($"{CurrentPointer}: Sizeof difference for {type.FullName}: Serialized {sizeOf} - Expected {size}");
						}
					} else {
						sizeOf = size;
					}
				} else {
					if (Mode == BinaryMode.Read) {
						var found = Context?.Loader?.FoundSizeOf;
						if (found != null && !found.ContainsKey(type)) found.Add(type, sizeOf);
					} else {
						var missing = Context?.Loader?.MissingSizeOf;
						if (missing != null && !missing.Contains(type)) missing.Add(type);
					}
				}
			}

			if (Mode == BinaryMode.Write)
				sizeOf = Serialize<uint>(sizeOf, name: "sizeof");
		}
		public override void IncreaseMemCount(Type type, uint count = 1, uint? customSize = null) {
			base.IncreaseMemCount(type, count: count, customSize: customSize);
			
			if(count <= 0) return;

			uint align = 4;
			if (Settings.MemoryData != null) {
				if (Settings.MemoryData.TryGetAlign(type, out var storedAlign)) {
					align = storedAlign;
				}
			}

			AlignMemCount(align);

			uint sizeOf = customSize ?? GetMemorySize(type);

			SerializerMemCount += sizeOf * count;
			//Log("SerializerMemCount: {0}. Increased by {1} * {2}", SerializerMemCount, sizeOf, count);
		}


		public override void CloseSerializer() {
			base.CloseSerializer();
			if (SerializerMemCountPosition.HasValue) {
				if (Mode == BinaryMode.Read) {
					if (SerializerMemCount != ExpectedMemCount) {
						Context?.SystemLogger?.LogWarning($"{CurrentPointer}: Wrong SerializerMemCount {SerializerMemCount}. Expected {ExpectedMemCount}. Difference: {(long)ExpectedMemCount - SerializerMemCount}");
					} else if (SerializerMemCount != 0) {
						//Context?.SystemLogger?.LogInfo($"{CurrentPointer}: Correctly calculated SerializerMemCount {SerializerMemCount}.");
					}
				} else {
					var pos = CurrentPosition;
					Goto(SerializerMemCountPosition.Value);
					SerializerMemCount = Serialize<uint>(SerializerMemCount, name: "SerializerMemCount");
					Goto(pos);
				}
				SerializerMemCountPosition = null;
			}
		}
		#endregion
	}
}
