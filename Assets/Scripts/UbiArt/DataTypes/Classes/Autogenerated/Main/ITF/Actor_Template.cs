namespace UbiArt.ITF {
	[Games(GameFlags.ROVersion | GameFlags.RLVersion | GameFlags.VH | GameFlags.RA)]
	public partial class Actor_Template : TemplatePickable {
		public bool PROCEDURAL;
		public bool STARTPAUSED;
		public bool FORCEISENVIRONMENT;
		public bool FORCEALWAYSACTIVE;
		public uint UPDATEFREQUENCE;
		public CArrayO<Generic<ActorComponent_Template>> COMPONENTS;
		public Vec2d scaleForced;
		public Vec2d scaleMin;
		public Vec2d scaleMax;
		public int xFLIPPED;
		public float zForced;
		public int useZForced;
		public StringID archetype;
		public StringID type;
		public Pickable.UpdateType updatetype;

		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.Settings.Game == Game.RJR || s.Settings.Game == Game.RFR || s.Settings.Game == Game.RO) {
				if (this is FriseConfig) return;
				scaleForced = s.SerializeObject<Vec2d>(scaleForced, name: "scaleForced");
				scaleMin = s.SerializeObject<Vec2d>(scaleMin, name: "scaleMin");
				scaleMax = s.SerializeObject<Vec2d>(scaleMax, name: "scaleMax");
				xFLIPPED = s.Serialize<int>(xFLIPPED, name: "xFLIPPED");
				PROCEDURAL = s.Serialize<bool>(PROCEDURAL, name: "PROCEDURAL");
				STARTPAUSED = s.Serialize<bool>(STARTPAUSED, name: "STARTPAUSED");
				zForced = s.Serialize<float>(zForced, name: "zForced");
				useZForced = s.Serialize<int>(useZForced, name: "useZForced");
				archetype = s.SerializeObject<StringID>(archetype, name: "archetype");
				type = s.SerializeObject<StringID>(type, name: "type");
				updatetype = s.Serialize<Pickable.UpdateType>(updatetype, name: "updatetype");
				if (s.HasFlags(SerializeFlags.Group_Data)) {
					COMPONENTS = s.SerializeObject<CArrayO<Generic<ActorComponent_Template>>>(COMPONENTS, name: "COMPONENTS");
				}
			} else if (s.Settings.Game == Game.RL || s.Settings.Game == Game.COL) {
				if (this is FriseConfig) return;
				PROCEDURAL = s.Serialize<bool>(PROCEDURAL, name: "PROCEDURAL", options: CSerializerObject.Options.BoolAsByte);
				STARTPAUSED = s.Serialize<bool>(STARTPAUSED, name: "STARTPAUSED", options: CSerializerObject.Options.BoolAsByte);
				FORCEISENVIRONMENT = s.Serialize<bool>(FORCEISENVIRONMENT, name: "FORCEISENVIRONMENT", options: CSerializerObject.Options.BoolAsByte);
				COMPONENTS = s.SerializeObject<CArrayO<Generic<ActorComponent_Template>>>(COMPONENTS, name: "COMPONENTS");
			} else if (s.Settings.Game == Game.VH) {
				if (this is FriseConfig) return;
				PROCEDURAL = s.Serialize<bool>(PROCEDURAL, name: "PROCEDURAL");
				STARTPAUSED = s.Serialize<bool>(STARTPAUSED, name: "STARTPAUSED");
				FORCEISENVIRONMENT = s.Serialize<bool>(FORCEISENVIRONMENT, name: "FORCEISENVIRONMENT");
				FORCEALWAYSACTIVE = s.Serialize<bool>(FORCEALWAYSACTIVE, name: "FORCEALWAYSACTIVE");
				COMPONENTS = s.SerializeObject<CArrayO<Generic<ActorComponent_Template>>>(COMPONENTS, name: "COMPONENTS");
			} else {
				PROCEDURAL = s.Serialize<bool>(PROCEDURAL, name: "PROCEDURAL");
				STARTPAUSED = s.Serialize<bool>(STARTPAUSED, name: "STARTPAUSED");
				FORCEISENVIRONMENT = s.Serialize<bool>(FORCEISENVIRONMENT, name: "FORCEISENVIRONMENT");
				FORCEALWAYSACTIVE = s.Serialize<bool>(FORCEALWAYSACTIVE, name: "FORCEALWAYSACTIVE");
				UPDATEFREQUENCE = s.Serialize<uint>(UPDATEFREQUENCE, name: "UPDATEFREQUENCE");
				COMPONENTS = s.SerializeObject<CArrayO<Generic<ActorComponent_Template>>>(COMPONENTS, name: "COMPONENTS");
			}
		}
		public override uint? ClassCRC => 0x1B857BCE;
	}
}

