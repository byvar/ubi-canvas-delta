using UnityEditor;
using UbiArt;
using UnityEngine;

[CustomEditor(typeof(UnityFriseConfig))]
public class UnityFriseConfigEditor : Editor {
	UnityEngine.Color darkSkinHeaderColor = (UnityEngine.Color)new Color32(62, 62, 62, 255);
	UnityEngine.Color lightSkinHeaderColor = (UnityEngine.Color)new Color32(194, 194, 194, 255);
	protected override void OnHeaderGUI() {
		var rect = EditorGUILayout.GetControlRect(false, 0f);
		rect.height = EditorGUIUtility.singleLineHeight;
		rect.y -= rect.height * 1.4f;
		rect.x = 60;
		rect.xMax -= rect.x * 2f;

		EditorGUI.DrawRect(rect, EditorGUIUtility.isProSkin ? darkSkinHeaderColor : lightSkinHeaderColor);

		string header = "Frise Config (Shared)";
		EditorGUI.LabelField(rect, header, EditorStyles.boldLabel);
	}

	public override void OnInspectorGUI() {
		//DrawDefaultInspector();
		OnHeaderGUI();

		UnityFriseConfig fcg = target as UnityFriseConfig;
		if (fcg != null) {
			if (fcg.friseConfig != null) {
				var s = CSerializerObjectUnityEditor.Serializer(Controller.MainContext);
				s.InitFoldout(fcg.friseConfig);
				fcg.friseConfig.Serialize(s, "FriseConfig");
			}
		}

	}
}