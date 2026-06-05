using UnityEditor;
using UbiArt;
using UnityEngine;
using UbiCanvas.Helpers;

[CustomEditor(typeof(UnitySceneConfig))]
public class UnitySceneConfigEditor : Editor {
	UnityEngine.Color darkSkinHeaderColor = (UnityEngine.Color)new Color32(62, 62, 62, 255);
	UnityEngine.Color lightSkinHeaderColor = (UnityEngine.Color)new Color32(194, 194, 194, 255);
	protected override void OnHeaderGUI() {
		var rect = EditorGUILayout.GetControlRect(false, 0f);
		rect.height = EditorGUIUtility.singleLineHeight;
		rect.y -= rect.height * 1.4f;
		rect.x = 60;
		rect.xMax -= rect.x * 2f;

		EditorGUI.DrawRect(rect, EditorGUIUtility.isProSkin ? darkSkinHeaderColor : lightSkinHeaderColor);

		UnitySceneConfig sc = target as UnitySceneConfig;
		if (sc != null && sc.config != null) {
			string header = sc.config.GetType().GetFormattedName();
			EditorGUI.LabelField(rect, header, EditorStyles.boldLabel);
		} else {

			EditorGUI.LabelField(rect, "Scene Config", EditorStyles.boldLabel);
		}
	}

	public override void OnInspectorGUI() {
		//DrawDefaultInspector();
		OnHeaderGUI();

		UnitySceneConfig sc = target as UnitySceneConfig;
		if (sc != null && sc.config != null) {
			var s = CSerializerObjectUnityEditor.Serializer(Controller.MainContext);
			s.InitFoldout(sc.config);
			sc.config.Serialize(s, "SceneConfig");
		}

	}
}