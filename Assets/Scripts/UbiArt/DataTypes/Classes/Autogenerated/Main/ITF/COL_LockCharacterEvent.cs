namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_LockCharacterEvent : Event {
		public bool _lock;
		public StringID characterID;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			_lock = s.Serialize<bool>(_lock, name: "lock", options: CSerializerObject.Options.BoolAsByte);
			characterID = s.SerializeObject<StringID>(characterID, name: "characterID");
		}
		public override uint? ClassCRC => 0x81DA3C2A;
	}
}

