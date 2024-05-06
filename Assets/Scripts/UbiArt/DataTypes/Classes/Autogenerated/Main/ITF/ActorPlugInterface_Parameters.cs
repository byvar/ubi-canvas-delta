namespace UbiArt.ITF {
	[Games(GameFlags.VH | GameFlags.RA)]
	public partial class ActorPlugInterface_Parameters : CSerializable {
		public CRefList<PlugConfig> configList;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			configList = s.SerializeObject<CRefList<PlugConfig>>(configList, name: "configList");
		}
	}
}

