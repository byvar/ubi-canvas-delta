using System;

namespace UbiArt.ITF {
	public partial class ActorComponent_Template : CSerializable {
		// Prepare component for conversion or returns a new component
		public virtual ActorComponent_Template Convert(Context context, Actor_Template actor, Settings oldSettings, Settings newSettings) {
			if (oldSettings.EngineVersion <= EngineVersion.RO && newSettings.EngineVersion >= EngineVersion.RL) {
				var type = GetType();
				if (type.Name.StartsWith("Ray_")) {
					var typeName = type.FullName;
					var ro2TypeName = typeName.Replace("Ray_", "RO2_");
					Type ro2Type = Type.GetType(ro2TypeName);
					if (ro2Type != null) {
						var c = (ActorComponent_Template)Activator.CreateInstance(ro2Type);
						c.InitContext(context);
						Merger.MergeGeneric(this, ro2Type, targetObject: c);
						return c;
					}
				}
			}
			return this;
		}

		public virtual ActorComponent Instantiate(Context context) {
			var tplType = GetType();
			var typeName = tplType.FullName;
			if (typeName.Contains("_Template"))
				typeName = typeName.Replace("_Template", "");

			Type type = Type.GetType(typeName);

			// Check whether the class exists
			if (type == null) throw new Exception($"Could not create instance class of component {tplType.FullName}");

			var c = (ActorComponent)Activator.CreateInstance(type);
			c.InitContext(context);
			return c;
		}

		public virtual string GetInstanceTypeName() {
			var tplType = GetType();
			var typeName = tplType.FullName;
			if (typeName.Contains("_Template"))
				typeName = typeName.Replace("_Template", "");
			return typeName;
		}
	}
}