using System;

namespace UbiArt.ITF {
	public partial class BezierBranchComponent_Template : CSerializable {
		public virtual BezierBranchComponent Instantiate(Context context) {
			var tplType = GetType();
			var typeName = tplType.FullName;
			if (typeName.Contains("_Template"))
				typeName = typeName.Replace("_Template", "");

			Type type = Type.GetType(typeName);

			// Check whether the class exists
			if (type == null) throw new Exception($"Could not create instance class of component {tplType.FullName}");

			var c = (BezierBranchComponent)Activator.CreateInstance(type);
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