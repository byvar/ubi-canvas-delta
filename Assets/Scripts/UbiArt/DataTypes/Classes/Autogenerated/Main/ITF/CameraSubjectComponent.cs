namespace UbiArt.ITF {
	[Games(GameFlags.All)]
	public partial class CameraSubjectComponent : ActorComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xA08CA422;
	}
}

