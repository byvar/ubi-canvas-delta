namespace UbiArt.ITF {
	[Games(GameFlags.All)]
	public partial class PolyLineEdge : CSerializable {
		public Vec2d POS;
		public float Scale = 1f;
		public bool SwitchTexture;
		public StringID GMatOverride = new StringID();
		public Hole HoleMode;
		public Vec2d Vector = Vec2d.Zero;
		public Vec2d NormalizedVector = Vec2d.Zero;
		public float Length;
		public StringID GameMaterial = new StringID();

		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.Settings.Game == Game.RJR || s.Settings.Game == Game.RFR || s.Settings.Game == Game.RO) {
				if (s.HasFlags(SerializeFlags.Editor | SerializeFlags.Group_DataEditable)) {
					POS = s.SerializeObject<Vec2d>(POS, name: "POS");
				}
				if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
					HoleMode = s.Serialize<Hole>(HoleMode, name: "HoleMode");
					Scale = s.Serialize<float>(Scale, name: "Scale");
					SwitchTexture = s.Serialize<bool>(SwitchTexture, name: "SwitchTexture");
				}
			} else if (s.Settings.Game == Game.VH || s.Settings.Game == Game.RL || s.Settings.Game == Game.COL) {
				POS = s.SerializeObject<Vec2d>(POS, name: "POS");
				Scale = s.Serialize<float>(Scale, name: "Scale");
				SwitchTexture = s.Serialize<bool>(SwitchTexture, name: "SwitchTexture");
				HoleMode = s.Serialize<Hole>(HoleMode, name: "HoleMode");
				if (s.HasFlags(SerializeFlags.DataBin)) {
					Vector = s.SerializeObject<Vec2d>(Vector, name: "Vector");
					NormalizedVector = s.SerializeObject<Vec2d>(NormalizedVector, name: "NormalizedVector");
					Length = s.Serialize<float>(Length, name: "Length");
					GameMaterial = s.SerializeObject<StringID>(GameMaterial, name: "GameMaterial");
				}
			} else {
				POS = s.SerializeObject<Vec2d>(POS, name: "POS");
				Scale = s.Serialize<float>(Scale, name: "Scale");
				SwitchTexture = s.Serialize<bool>(SwitchTexture, name: "SwitchTexture");
				GMatOverride = s.SerializeObject<StringID>(GMatOverride, name: "GMatOverride");
				HoleMode = s.Serialize<Hole>(HoleMode, name: "HoleMode");
				if (s.HasFlags(SerializeFlags.DataBin)) {
					Vector = s.SerializeObject<Vec2d>(Vector, name: "Vector");
					NormalizedVector = s.SerializeObject<Vec2d>(NormalizedVector, name: "NormalizedVector");
					Length = s.Serialize<float>(Length, name: "Length");
					GameMaterial = s.SerializeObject<StringID>(GameMaterial, name: "GameMaterial");
				}
			}
		}
		public enum Hole {
			[Serialize("Hole_None"     )] None = 0,
			[Serialize("Hole_Collision")] Collision = 1,
			[Serialize("Hole_Visual"   )] Visual = 2,
			[Serialize("Hole_Both"     )] Both = 3,
		}
	}
}

