namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_DanceStoneComponent_Template : ActorComponent_Template {
		public int dancerShouldFlip;
		public Ray_DanceStoneComponent_Template.Tempo upTempo;
		public Ray_DanceStoneComponent_Template.Tempo downTempo;
		public float syncRatio;
		public float syncOffset;
		public uint numTemposToUnlock;
		public float distXSnap;
		public float distYSnap;
		public uint numLumNormalReward;
		public uint numLumPowerReward;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			dancerShouldFlip = s.Serialize<int>(dancerShouldFlip, name: "dancerShouldFlip");
			upTempo = s.SerializeObject<Ray_DanceStoneComponent_Template.Tempo>(upTempo, name: "upTempo");
			downTempo = s.SerializeObject<Ray_DanceStoneComponent_Template.Tempo>(downTempo, name: "downTempo");
			syncRatio = s.Serialize<float>(syncRatio, name: "syncRatio");
			syncOffset = s.Serialize<float>(syncOffset, name: "syncOffset");
			numTemposToUnlock = s.Serialize<uint>(numTemposToUnlock, name: "numTemposToUnlock");
			distXSnap = s.Serialize<float>(distXSnap, name: "distXSnap");
			distYSnap = s.Serialize<float>(distYSnap, name: "distYSnap");
			numLumNormalReward = s.Serialize<uint>(numLumNormalReward, name: "numLumNormalReward");
			numLumPowerReward = s.Serialize<uint>(numLumPowerReward, name: "numLumPowerReward");
		}
		public override uint? ClassCRC => 0xA0D4FA77;

		[Games(GameFlags.RO)]
		public class Tempo : CSerializable {
			public float min;
			public float max;
			protected override void SerializeImpl(CSerializerObject s) {
				base.SerializeImpl(s);
				min = s.Serialize<float>(min, name: "min");
				max = s.Serialize<float>(max, name: "max");
			}
		}
	}
}

