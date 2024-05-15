using System;
using System.Collections.Generic;

namespace UbiArt.ITF {
	public partial class CriteriaDesc {
		protected override void OnPostSerialize(CSerializerObject s) {
			base.OnPostSerialize(s);
			if (s.Settings.Game == Game.RJR || s.Settings.Game == Game.RFR
			                                || s.Settings.Game == Game.RO || s.Settings.Game == Game.RL) {
				SetEvaluationEnum();
			} else {
				if (evaluation == Type.Undefined) {
					SetEvaluationEnum();
				} else {
					SetEvaluationStringID();
				}
			}
		}
		protected void SetEvaluationEnum() {
			var sid = eval?.stringID ?? 0xFFFFFFFF;
			evaluation = sid switch {
				0x8b2df96c => Type.LessThan,
				0x15285401 => Type.LessThanOrEquals,
				0x8caece26 => Type.GreaterThan,
				0x4d07b33f => Type.GreaterThanOrEquals,
				0x158d0faf => Type.Equals,
				0xfb9ae83 => Type.NotEquals,
				0x2166158f => Type.And,
				_ => Type.Undefined
			};
		}
		protected void SetEvaluationStringID() {
			eval = evaluation switch {
				Type.LessThan => new StringID("<"),
				Type.LessThanOrEquals => new StringID("<="),
				Type.GreaterThan => new StringID(">"),
				Type.GreaterThanOrEquals => new StringID(">="),
				Type.Equals => new StringID("=="),
				Type.NotEquals => new StringID("!="),
				Type.And => new StringID("&"),
				_ => new StringID()
			};
		}
	}
}
