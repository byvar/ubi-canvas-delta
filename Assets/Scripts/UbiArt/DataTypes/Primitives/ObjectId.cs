namespace UbiArt {
	public class ObjectId : CSerializable, ICSerializableShortLog {
		public long id;

		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.Settings.Game == Game.COL) {
				id = s.Serialize<int>((int)id);
			} else {
				id = s.Serialize<long>(id);
			}
		}

		public const int Invalid = -1;

		public bool IsNull => id == Invalid;

		public string SerializeLog(CSerializerObject s) => $"ObjectId({id})";
	}
}
