namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class ResourceGroup : Resource {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x9F50B55F;
	}
}

