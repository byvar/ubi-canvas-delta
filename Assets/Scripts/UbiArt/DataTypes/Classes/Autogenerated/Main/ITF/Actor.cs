namespace UbiArt.ITF {
	[Games(GameFlags.All)]
	public partial class Actor : Pickable {
		public Path LUA;
		public Nullable<Bind> parentBind;
		public CArrayO<Generic<ActorComponent>> COMPONENTS;
		public Nullable<ActorBind> parentBindOrigins;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.Settings.Game == Game.RJR || s.Settings.Game == Game.RFR || s.Settings.Game == Game.RO) {
				if (this is Frise) return;
				if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
					LUA = s.SerializeObject<Path>(LUA, name: "LUA");
					xFLIPPED = s.Serialize<bool>(xFLIPPED, name: "xFLIPPED");
					parentBindOrigins = s.SerializeObject<Nullable<ActorBind>>(parentBindOrigins, name: "parentBind");
				}
				if (s.HasFlags(SerializeFlags.Group_Data)) {
					COMPONENTS = s.SerializeObject<CArrayO<Generic<ActorComponent>>>(COMPONENTS, name: "COMPONENTS");
				}
			} else if (s.Settings.Game == Game.RL || s.Settings.Game == Game.VH) {
				if (this is Frise) return;
				if (s.Settings.Platform == GamePlatform.Vita) {
					uint lol = 0;
					lol = s.Serialize<uint>(lol, name: "placeholder");
					lol = s.Serialize<uint>(lol, name: "placeholder");
					lol = s.Serialize<uint>(lol, name: "placeholder");
				}
				if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
					LUA = s.SerializeObject<Path>(LUA, name: "LUA");
					parentBind = s.SerializeObject<Nullable<Bind>>(parentBind, name: "parentBind");
				}
				if (s.HasFlags(SerializeFlags.Group_Checkpoint | SerializeFlags.Group_DataEditable | SerializeFlags.ForcedValues)) {
					COMPONENTS = s.SerializeObject<CArrayO<Generic<ActorComponent>>>(COMPONENTS, name: "COMPONENTS");
				}
			} else if (s.Settings.Game == Game.COL) {
				if (this is Frise) return;
				if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
					LUA = s.SerializeObject<Path>(LUA, name: "LUA");
					parentBind = s.SerializeObject<Nullable<Bind>>(parentBind, name: "parentBind");
				}
				if (s.HasFlags(SerializeFlags.Group_Checkpoint | SerializeFlags.Group_DataEditable | SerializeFlags.ForcedValues
					| SerializeFlags.Flags16 | SerializeFlags.Flags17)) {
					COMPONENTS = s.SerializeObject<CArrayO<Generic<ActorComponent>>>(COMPONENTS, name: "COMPONENTS");
				}
			} else {
				if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
					LUA = s.SerializeObject<Path>(LUA, name: "LUA");
					parentBind = s.SerializeObject<Nullable<Bind>>(parentBind, name: "parentBind");
				}
				if (s.HasFlags(SerializeFlags.Editor)) {
					parentBind = s.SerializeObject<Nullable<Bind>>(parentBind, name: "parentBind");
				}
				if (s.HasFlags(SerializeFlags.Group_Checkpoint | SerializeFlags.Group_DataEditable | SerializeFlags.ForcedValues)) {
					COMPONENTS = s.SerializeObject<CArrayO<Generic<ActorComponent>>>(COMPONENTS, name: "COMPONENTS");
				}
			}
		}
		public override uint? ClassCRC => 0x97CA628B;
	}
}

