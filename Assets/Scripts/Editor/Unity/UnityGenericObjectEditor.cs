using UnityEditor;
using UbiArt;
using UnityEngine;
using UbiCanvas.Helpers;

[CustomEditor(typeof(UnityGenericObject))]
public class UnityGenericObjectEditor : Editor {
	UnityEngine.Color darkSkinHeaderColor = (UnityEngine.Color)new Color32(62, 62, 62, 255);
	UnityEngine.Color lightSkinHeaderColor = (UnityEngine.Color)new Color32(194, 194, 194, 255);
	protected override void OnHeaderGUI() {
		var rect = EditorGUILayout.GetControlRect(false, 0f);
		rect.height = EditorGUIUtility.singleLineHeight;
		rect.y -= rect.height * 1.4f;
		rect.x = 60;
		rect.xMax -= rect.x * 2f;

		EditorGUI.DrawRect(rect, EditorGUIUtility.isProSkin ? darkSkinHeaderColor : lightSkinHeaderColor);

		UnityGenericObject ugo = target as UnityGenericObject;
		if (ugo != null && ugo.obj != null) {
			string header = ugo.obj.GetType().GetFormattedName();
			EditorGUI.LabelField(rect, header, EditorStyles.boldLabel);
		} else {
			EditorGUI.LabelField(rect, "Unity Generic Object (null)", EditorStyles.boldLabel);
		}
	}

	public override void OnInspectorGUI() {
		//DrawDefaultInspector();
		OnHeaderGUI();

		UnityGenericObject ugo = target as UnityGenericObject;
		if (ugo != null && ugo.obj != null) {
			var s = CSerializerObjectUnityEditor.Serializer(Controller.MainContext);
			s.InitFoldout(ugo.obj);
			ugo.obj.Serialize(s, "UnityGenericObject");
		}

	}
}