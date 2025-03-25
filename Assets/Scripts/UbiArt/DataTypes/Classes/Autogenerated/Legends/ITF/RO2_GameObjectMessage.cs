namespace UbiArt.ITF {
	[Games(GameFlags.RL)]
	public partial class RO2_GameObjectMessage : CSerializable {
		public uint m_randomCount;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			m_randomCount = s.Serialize<uint>(m_randomCount, name: "m_randomCount");
		}
		public override uint? ClassCRC => 0x753F1CC8;
	}
}

