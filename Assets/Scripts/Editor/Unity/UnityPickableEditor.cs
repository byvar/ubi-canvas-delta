using UnityEditor;
using UbiArt;
using UnityEngine;
using UbiArt.ITF;

[CustomEditor(typeof(UnityPickable))]
public class UnityPickableEditor : Editor {
	private static bool ShowParentBind = false;
	private static bool EditTemplatePath = false;

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
			if (p.pickable is UbiArt.ITF.Frise f) {
				EditTemplatePath = EditorGUILayout.Toggle("Edit FriseConfig path (save & reload)", EditTemplatePath);
				var path = EditorGUILayout.TextField("FriseConfig", (string)(f?.ConfigName?.FullPath));
				if (EditTemplatePath) {
					f.ConfigName = new Path(path);
					f.LUA = f.ConfigName;
				}
			} else if (p.pickable is UbiArt.ITF.Actor a) {
				EditTemplatePath = EditorGUILayout.Toggle("Edit LUA path (save & reload)", EditTemplatePath);
				var path = EditorGUILayout.TextField("LUA", (string)(a?.LUA?.FullPath));
				if (EditTemplatePath) {
					a.LUA = new Path(path);
				}
			}

			if (p.pickable is UbiArt.ITF.Actor act &&
				(Controller.MainContext?.Settings?.Game == Game.RA || Controller.MainContext?.Settings?.Game == Game.RM
				|| !(p.pickable is UbiArt.ITF.Frise))) {
				ShowParentBind = EditorGUILayout.Foldout(ShowParentBind, "parentBind");
				if (ShowParentBind) {
					EditorGUI.indentLevel++;
					if (Controller.MainContext?.Settings?.EngineVersion <= EngineVersion.RO) {
						act.parentBindOrigins.Serialize(s, "parentbind");
					} else {
						act.parentBind.Serialize(s, "parentBind");

						if (act?.parentBind?.read ?? false) {
							if (GUILayout.Button("Recalculate position")) {
								var tree = new PickableTree(Controller.Obj.MainScene.obj);
								var path = Controller.Obj.MainScene.obj.FindPickable(pick => pick == p.pickable);

								if (path != null) {
									var curNode = tree.FollowObjectPath(path.Path, throwIfNotFound: false);
									if (curNode != null) {
										var parentBind = act.parentBind.value;
										var linkedNode = curNode.Parent.GetNodeWithObjectPath(parentBind.parentPath, throwIfNotFound: false);
										bool foundParent = linkedNode?.Pickable != null;
										if (foundParent) {
											var parentObj = linkedNode.Pickable.GetPrecreatedGameObject().GetComponent<UnityPickable>();
											var relativePos = parentObj.transform.InverseTransformPoint(p.transform.position);

											parentBind.offsetPos = new Vec3d(relativePos.x, relativePos.y, -relativePos.z);

											var calculatedAngle = p.transform.eulerAngles.z - parentObj.transform.eulerAngles.z;
											var curAngle = new Angle(parentBind.offsetAngle).EulerAngle;
											var angleDifference = p.NormalizeEulerAngle(calculatedAngle) - p.NormalizeEulerAngle(curAngle);
											if (Mathf.Abs(angleDifference) > 0.00005) {
												parentBind.offsetAngle = new Angle(p.NormalizeEulerAngle(calculatedAngle), degrees: true);
											}
										}
									}
								}
							}
						}

					}
					EditorGUI.indentLevel--;
				}
			}

			if (p.pickable is UbiArt.ITF.Actor act2) {
				if ((Controller.MainContext?.Settings?.Game == Game.RA || Controller.MainContext?.Settings?.Game == Game.RM)) {
					act2.STARTPAUSE = s.Serialize<bool>(act2.STARTPAUSE, name: "STARTPAUSE");
				}
				if (p.pickable.templatePickable is Actor_Template atpl) {
					atpl.STARTPAUSED = s.Serialize<bool>(atpl.STARTPAUSED, name: "(TEMPLATE) STARTPAUSED");
				}
			}

			if (p.pickable.templatePickable != null) {
				if (p.pickable.templatePickable.TAGS != null) {
					p.pickable.templatePickable.TAGS.Serialize(s, "TAGS");
				}
			}
			if (p.pickable is UbiArt.ITF.SubSceneActor ssa) {
				ssa.EMBED_SCENE = s.Serialize<bool>(ssa.EMBED_SCENE, name: "EMBED_SCENE");
				ssa.IS_SINGLE_PIECE = s.Serialize<bool>(ssa.IS_SINGLE_PIECE, name: "IS_SINGLE_PIECE");
				ssa.ZFORCED = s.Serialize<bool>(ssa.ZFORCED, name: "ZFORCED");
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