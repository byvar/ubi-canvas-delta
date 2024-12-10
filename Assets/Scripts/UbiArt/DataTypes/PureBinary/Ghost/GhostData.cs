using BinarySerializer;
using System;
using System.Collections.Generic;
using UbiArt.ITF;
using UbiCanvas.Helpers;

namespace UbiArt.Ghost {
	public class GhostData : CSerializable {
		public uint ghostsCount;
		public GhostType ghostType;
		public Ghost[] ghosts;

		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			ghostsCount = s.Serialize<uint>(ghostsCount, name: nameof(ghostsCount));
			ghostType = s.Serialize<GhostType>(ghostType, name: nameof(ghostType));

			if(ghosts == null)
				ghosts = new Ghost[ghostsCount];
			if (ghosts.Length != ghostsCount)
				Array.Resize<Ghost>(ref ghosts, (int)ghostsCount);

			for (int i = 0; i < ghostsCount; i++) {
				switch (ghostType) {
					case GhostType.Anim:
						SerializeGhostFrame<GhostAnim>(s, i);
						break;
					default:
						throw new NotImplementedException();
				}
			}
		}

		public void SerializeGhostFrame<T>(CSerializerObject s, int i) where T : Ghost, new() {
			if (ghosts[i] == null || ghosts[i] is not T) ghosts[i] = new T();
			ghosts[i] = s.SerializeObject<T>(ghosts[i] as T, name: $"{nameof(ghosts)}[{i}]");
		}

		public abstract class Ghost : CSerializable {
			public uint framesCount;
			protected override void SerializeImpl(CSerializerObject s) {
				base.SerializeImpl(s);
				framesCount = s.Serialize<uint>(framesCount, name: nameof(framesCount));
			}
		}
		public class GhostAnim : Ghost {
			public Frame[] frames;
			public CListO<Input> inputs;

			protected override void SerializeImpl(CSerializerObject s) {
				base.SerializeImpl(s);

				inputs = s.SerializeObject<CListO<Input>>(inputs, name: nameof(inputs));

				if (frames == null) frames = new Frame[framesCount];
				if (frames.Length != framesCount)
					Array.Resize<Frame>(ref frames, (int)framesCount);

				for (int i = 0; i < frames.Length; i++) {
					if(frames[i] == null) frames[i] = new Frame();
					frames[i].ghost = this;
					frames[i] = s.SerializeObject<Frame>(frames[i], name: $"{nameof(frames)}[{i}]");
				}
			}

			public class Input : CSerializable {
				public InputDesc.InputType varType;
				public StringID name;
				protected override void SerializeImpl(CSerializerObject s) {
					base.SerializeImpl(s);
					varType = s.Serialize<InputDesc.InputType>(varType, name: nameof(varType));
					name = s.SerializeObject<StringID>(name, name: nameof(name));
				}
			}

			public class Frame : CSerializable {
				public GhostAnim ghost;

				public Flag flags;
				public bool unk0;
				public bool xFLIPPED;
				public StringID animation;
				public float posX;
				public float posY;
				public float posZ;
				public float scaleX;
				public float scaleY;
				public Vec2d scale;
				public uint score;
				public Angle angle;
				public uint inputsCount;

				public long inputsMask1;
				public long inputsMask2;

				public Dictionary<int, uint> inputsUInt = new Dictionary<int, uint>();
				public Dictionary<int, float> inputsFloat = new Dictionary<int, float>();

				protected override void SerializeImpl(CSerializerObject s) {
					base.SerializeImpl(s);
					flags = s.Serialize<Flag>(flags, name: nameof(flags));
					//if (flags.HasFlag(Flag.Flags0)) unk0 = s.Serialize<bool>(unk0, name: nameof(unk0));
					if (flags.HasFlag(Flag.XFLIPPED)) xFLIPPED = s.Serialize<bool>(xFLIPPED, name: nameof(xFLIPPED));
					if (flags.HasFlag(Flag.Animation)) animation = s.SerializeObject<StringID>(animation, name: nameof(animation));
					if (flags.HasFlag(Flag.PositionX)) posX = s.Serialize<float>(posX, name: nameof(posX));
					if (flags.HasFlag(Flag.PositionY)) posY = s.Serialize<float>(posY, name: nameof(posY));
					if (flags.HasFlag(Flag.PositionZ)) posZ = s.Serialize<float>(posZ, name: nameof(posZ));
					if (flags.HasFlag(Flag.ScaleX)) scaleX = s.Serialize<float>(scaleX, name: nameof(scaleX));
					if (flags.HasFlag(Flag.ScaleY)) scaleY = s.Serialize<float>(scaleY, name: nameof(scaleY));
					//if (flags.HasFlag(Flag.Scale)) scale = s.SerializeObject<Vec2d>(scale, name: nameof(scale));
					if (flags.HasFlag(Flag.Score)) score = s.Serialize<uint>(score, name: nameof(score));
					if (flags.HasFlag(Flag.Angle)) angle = s.SerializeObject<Angle>(angle, name: nameof(angle));
					if (flags.HasFlag(Flag.InputsCount)) inputsCount = s.Serialize<uint>(inputsCount, name: nameof(inputsCount));

					inputsMask1 = s.Serialize<long>(inputsMask1, name: nameof(inputsMask1));
					inputsMask2 = s.Serialize<long>(inputsMask2, name: nameof(inputsMask2));

					if (ghost?.inputs != null) {
						for (int i = 0; i < ghost.inputs.Count; i++) {
							if (HasInput((uint)i)) {
								var input = ghost.inputs[i];
								if (input.varType == InputDesc.InputType.F32) {
									float value = inputsFloat.TryGetItem(i, 0);
									value = s.Serialize<float>(value, name: $"inputs[{input.name.ToString(s.Context)}]");
									inputsFloat[i] = value;
								} else {
									uint value = inputsUInt.TryGetItem(i, (uint)0);
									value = s.Serialize<uint>(value, name: $"inputs[{input.name.ToString(s.Context)}]");
									inputsUInt[i] = value;
								}
							}
						}
					}
				}

				public bool HasInput(uint index) {
					if (index < 64) {
						return BitHelpers.ExtractBits64(inputsMask1, 1, (int)(index)) == 1;
					} else if (index < 128) {
						return BitHelpers.ExtractBits64(inputsMask2, 1, (int)(index - 64)) == 1;
					} else return false;
				}

				[Flags]
				public enum Flag : uint {
					None = 0,
					Flags0 = 1 << 0,
					PositionX = 1 << 1,
					PositionY = 1 << 2,
					PositionZ = 1 << 3,
					Score = 1 << 4,
					Angle = 1 << 5,
					XFLIPPED = 1 << 6,
					ScaleX = 1 << 7,
					ScaleY = 1 << 8,
					Animation = 1 << 9,
					InputsCount = 1 << 10,
					Flags11 = 1 << 11,
					Flags12 = 1 << 12,
					Flags13 = 1 << 13,
					Flags14 = 1 << 14,
					Flags15 = 1 << 15,
					Flags16 = 1 << 16,
					Flags17 = 1 << 17,
					Flags18 = 1 << 18,
					Flags19 = 1 << 19,
					Flags20 = 1 << 20,
					Flags21 = 1 << 21,
					Flags22 = 1 << 22,
					Flags23 = 1 << 23,
					Flags24 = 1 << 24,
					Flags25 = 1 << 25,
					Flags26 = 1 << 26,
					Flags27 = 1 << 27,
					Flags28 = 1 << 28,
					Flags29 = 1 << 29,
					Flags30 = 1 << 30,
					Flags31 = (uint)1 << 31,
				}


				[Flags]
				public enum Flag_RLC : uint {
					None = 0,
					Flags0 = 1 << 0,
					PositionX = 1 << 1,
					PositionY = 1 << 2,
					PositionZ = 1 << 3,
					Flags4 = 1 << 4,
					Angle = 1 << 5,
					XFLIPPED = 1 << 6,
					Scale = 1 << 7,
					Animation = 1 << 8,
					InputsCount = 1 << 9,
					Flags10 = 1 << 11,
					Flags11 = 1 << 11,
					Flags12 = 1 << 12,
					Flags13 = 1 << 13,
					Flags14 = 1 << 14,
					Flags15 = 1 << 15,
					Flags16 = 1 << 16,
					Flags17 = 1 << 17,
					Flags18 = 1 << 18,
					Flags19 = 1 << 19,
					Flags20 = 1 << 20,
					Flags21 = 1 << 21,
					Flags22 = 1 << 22,
					Flags23 = 1 << 23,
					Flags24 = 1 << 24,
					Flags25 = 1 << 25,
					Flags26 = 1 << 26,
					Flags27 = 1 << 27,
					Flags28 = 1 << 28,
					Flags29 = 1 << 29,
					Flags30 = 1 << 30,
					Flags31 = (uint)1 << 31,
				}
			}
		}

		public enum GhostType : uint {
			None = 0,
			Anim = 1,
			Pad = 2,
			MobilePad = 3
		}
	}
}
