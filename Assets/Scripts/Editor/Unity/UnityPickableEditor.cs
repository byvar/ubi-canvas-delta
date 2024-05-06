using UnityEditor;
using UbiArt;
using UnityEngine;

[CustomEditor(typeof(UnityPickable))]
public class UnityPickableEditor : Editor {
	private static bool ShowParentBind = false;

	/*public override bool RequiresConstantRepaint() {
		//return base.RequiresConstantRepaint();
		return true;
	}*/

	public override void OnInspectorGUI() {
		DrawDefaultInspector();

		UnityPickable p = target as UnityPickable;
		if (p?.pickable != null) {
			p.UpdateDataFromTransform();
			var s = CSerializerObjectUnityEditor.Serializer(Controller.MainContext);
			bool wasFlipped = p.pickable.xFLIPPED;
			Vec2d prevPos = new Vec2d(p.pickable.POS2D?.x ?? 0, p.pickable.POS2D?.y ?? 0);
			float prevZ = p.pickable.RELATIVEZ;
			Angle prevAngle = new Angle(p.pickable.ANGLE);
			Vec2d prevScale = new Vec2d(p.pickable.SCALE?.x ?? 1f, p.pickable.SCALE?.y ?? 1f);
			Vec2d newPos = s.SerializeObject<Vec2d>(p.pickable.POS2D, name: "POS2D");
			float newZ = s.Serialize<float>(p.pickable.RELATIVEZ, name: "RELATIVEZ");
			Angle newAngle = s.SerializeObject<Angle>(p.pickable.ANGLE, name: "ANGLE");
			Vec2d newScale = s.SerializeObject<Vec2d>(p.pickable.SCALE, name: "SCALE");
			bool isFlipped = s.Serialize<bool>(wasFlipped, name: "xFLIPPED");
			if (wasFlipped != isFlipped || prevScale != newScale || prevPos != newPos || prevZ != newZ || prevAngle != newAngle) {
				p.pickable.ANGLE = newAngle;
				p.pickable.SCALE = newScale;
				p.pickable.POS2D = newPos;
				p.pickable.RELATIVEZ = newZ;
				p.pickable.xFLIPPED = isFlipped;
				p.ResetTransformFromData();
			}
			if (p.pickable.templatePickable != null) {
				if (p.pickable is UbiArt.ITF.Frise f) {
					EditorGUILayout.TextField("FriseConfig", (string)(f?.ConfigName?.FullPath));
				} else if (p.pickable is UbiArt.ITF.Actor a) {
					EditorGUILayout.TextField("LUA", (string)(a?.LUA?.FullPath));
				}

				if (p.pickable is UbiArt.ITF.Actor act &&
					(Controller.MainContext?.Settings?.Game == Game.RA || Controller.MainContext?.Settings?.Game == Game.RM
					|| !(p.pickable is UbiArt.ITF.Frise))) {
					ShowParentBind = EditorGUILayout.Foldout(ShowParentBind, "parentBind");
					if (ShowParentBind) {
						EditorGUI.indentLevel++;
						act.parentBind.Serialize(s, "parentBind");
						EditorGUI.indentLevel--;
					}
				}
				if (p.pickable is UbiArt.ITF.Actor act2 &&
					(Controller.MainContext?.Settings?.Game == Game.RA || Controller.MainContext?.Settings?.Game == Game.RM)) {
					act2.STARTPAUSE = s.Serialize<bool>(act2.STARTPAUSE, name: "STARTPAUSE");
				}
				if (p.pickable.templatePickable.TAGS != null) {
					p.pickable.templatePickable.TAGS.Serialize(s, "TAGS");
				}
			}

			if (GUILayout.Button("Select in game view")) {
				Controller.Obj.selector.Select(p, view: true);
			}
		}
		//if (p.Dirty) {
			//p.Dirty = false;
		Repaint();
		//}
	}
}