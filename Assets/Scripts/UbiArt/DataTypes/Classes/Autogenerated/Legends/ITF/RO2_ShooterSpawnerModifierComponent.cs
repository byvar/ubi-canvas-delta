namespace UbiArt.ITF {
	[Games(GameFlags.RL)]
	public partial class RO2_ShooterSpawnerModifierComponent : TimedSpawnerModifierComponent {
		public Enum_tweenId tweenId;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			tweenId = s.Serialize<Enum_tweenId>(tweenId, name: "tweenId");
		}
		public enum Enum_tweenId {
			[Serialize("Value__1")] Value__1 = -1,
		}
		public override uint? ClassCRC => 0xBFB882A5;
	}
}

