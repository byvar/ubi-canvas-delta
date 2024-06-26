namespace UbiArt.ITF {
	[Games(GameFlags.VH | GameFlags.RA)]
	public partial class RO2_BezierBranchRendererSegment_Template : CSerializable {
		public StringID name;
		public float startRatio;
		public float startOffset;
		public float endRatio = 1f;
		public float endOffset;
		public uint spriteMinIndex;
		public uint spriteMaxIndex;
		public float spritePlayRate;
		public float spritePlayRateSpeedMultiplier;
		public float tileLength = 1f;
		public bool uvAttachToEnd = true;
		public float uvScrollSpeed;
		public float beginLength { get => startLengthOffset; set => startLengthOffset = value; }
		public float endLength { get => endLengthOffset; set => endLengthOffset = value; }
		public float beginWidth { get => startWidth; set => startWidth = value; }
		public float beginAlpha { get => startAlpha; set => startAlpha = value; }
		public float startLengthRatio;
		public float startLengthOffset;
		public float endLengthRatio;
		public float endLengthOffset;
		public float startWidth = 1f;
		public float midWidth = 1f;
		public float endWidth = 1f;
		public float startAlpha;
		public float midAlpha = 1f;
		public float endAlpha;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			name = s.SerializeObject<StringID>(name, name: "name");
			startRatio = s.Serialize<float>(startRatio, name: "startRatio");
			startOffset = s.Serialize<float>(startOffset, name: "startOffset");
			endRatio = s.Serialize<float>(endRatio, name: "endRatio");
			endOffset = s.Serialize<float>(endOffset, name: "endOffset");
			spriteMinIndex = s.Serialize<uint>(spriteMinIndex, name: "spriteMinIndex");
			spriteMaxIndex = s.Serialize<uint>(spriteMaxIndex, name: "spriteMaxIndex");
			spritePlayRate = s.Serialize<float>(spritePlayRate, name: "spritePlayRate");
			spritePlayRateSpeedMultiplier = s.Serialize<float>(spritePlayRateSpeedMultiplier, name: "spritePlayRateSpeedMultiplier");
			tileLength = s.Serialize<float>(tileLength, name: "tileLength");
			uvAttachToEnd = s.Serialize<bool>(uvAttachToEnd, name: "uvAttachToEnd");
			uvScrollSpeed = s.Serialize<float>(uvScrollSpeed, name: "uvScrollSpeed");
			if (s.HasFlags(SerializeFlags.Deprecate)) {
				beginLength = s.Serialize<float>(beginLength, name: "beginLength");
				endLength = s.Serialize<float>(endLength, name: "endLength");
				beginWidth = s.Serialize<float>(beginWidth, name: "beginWidth");
				beginAlpha = s.Serialize<float>(beginAlpha, name: "beginAlpha");
			}
			startLengthRatio = s.Serialize<float>(startLengthRatio, name: "startLengthRatio");
			startLengthOffset = s.Serialize<float>(startLengthOffset, name: "startLengthOffset");
			endLengthRatio = s.Serialize<float>(endLengthRatio, name: "endLengthRatio");
			endLengthOffset = s.Serialize<float>(endLengthOffset, name: "endLengthOffset");
			startWidth = s.Serialize<float>(startWidth, name: "startWidth");
			midWidth = s.Serialize<float>(midWidth, name: "midWidth");
			endWidth = s.Serialize<float>(endWidth, name: "endWidth");
			startAlpha = s.Serialize<float>(startAlpha, name: "startAlpha");
			midAlpha = s.Serialize<float>(midAlpha, name: "midAlpha");
			endAlpha = s.Serialize<float>(endAlpha, name: "endAlpha");
		}
	}
}

