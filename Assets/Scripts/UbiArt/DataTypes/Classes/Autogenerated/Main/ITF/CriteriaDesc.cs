namespace UbiArt.ITF {
	[Games(GameFlags.RJR | GameFlags.RFR | GameFlags.VH | GameFlags.RA)]
	public partial class CriteriaDesc : CSerializable {
		public StringID name;
		public Type evaluation;
		public StringID eval;
		public float value;
		public bool isOR;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.Settings.Game == Game.RJR || s.Settings.Game == Game.RFR
			                                || s.Settings.Game == Game.RO || s.Settings.Game == Game.RL) {
				name = s.SerializeObject<StringID>(name, name: "name");
				eval = s.SerializeObject<StringID>(eval, name: "eval");
				value = s.Serialize<float>(value, name: "value");
				isOR = s.Serialize<bool>(isOR, name: "isOR");
			} else {
				name = s.SerializeObject<StringID>(name, name: "name");
				evaluation = s.Serialize<Type>(evaluation, name: "evaluation");
				if (evaluation == Type.Undefined) {
					eval = s.SerializeObject<StringID>(eval, name: "eval"); // StringID is a choice based on the strings of the enum (except Undefined). Check RA's CriteriaDesc::SerializeImpl
				}
				value = s.Serialize<float>(value, name: "value");
				isOR = s.Serialize<bool>(isOR, name: "isOR");
			}
		}
		public enum Type {
			[Serialize("Undefined"          )] Undefined = 0,
			[Serialize("LessThan"           )] LessThan = 1,
			[Serialize("LessThanOrEquals"   )] LessThanOrEquals = 2,
			[Serialize("GreaterThan"        )] GreaterThan = 3,
			[Serialize("GreaterThanOrEquals")] GreaterThanOrEquals = 4,
			[Serialize("Equals"             )] Equals = 5,
			[Serialize("NotEquals"          )] NotEquals = 6,
			[Serialize("And"                )] And = 7,
		}
	}
}

