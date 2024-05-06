using System;

namespace UbiArt.ITF {
	public partial class ActorComponent : CSerializable {
		// Prepare component for conversion or returns a new component
		public virtual ActorComponent Convert(Context context, Actor actor, Settings oldSettings, Settings newSettings) {
			if (oldSettings.EngineVersion <= EngineVersion.RO && newSettings.EngineVersion >= EngineVersion.RL) {
				var type = GetType();
				if (type.Name.StartsWith("Ray_")) {
					var typeName = type.FullName;
					var ro2TypeName = typeName.Replace("Ray_", "RO2_");
					Type ro2Type = Type.GetType(ro2TypeName);
					if (ro2Type != null) {
						var c = (ActorComponent)Activator.CreateInstance(ro2Type);
						c.InitContext(context);
						Merger.MergeGeneric(this, ro2Type, targetObject: c);
						return c;
					}
				}
			}
			return this;
		}
	}
}