using UnityEditor;
using UbiArt;
using UnityEngine;
using UbiCanvas.Helpers;

[CustomEditor(typeof(UnityActorComponent))]
public class UnityActorComponentEditor : Editor {
	UnityEngine.Color darkSkinHeaderColor = (UnityEngine.Color)new Color32(62, 62, 62, 255);
	UnityEngine.Color lightSkinHeaderColor = (UnityEngine.Color)new Color32(194, 194, 194, 255);
	protected override void OnHeaderGUI() {
		var rect = EditorGUILayout.GetControlRect(false, 0f);
		rect.height = EditorGUIUtility.singleLineHeight;
		rect.y -= rect.height * 1.4f;
		rect.x = 60;
		rect.xMax -= rect.x * 2f;

		EditorGUI.DrawRect(rect, EditorGUIUtility.isProSkin ? darkSkinHeaderColor : lightSkinHeaderColor);

		UnityActorComponent uac = target as UnityActorComponent;
		if (uac != null && uac.component != null) {
			string header = uac.component.GetType().GetFormattedName();
			EditorGUI.LabelField(rect, header, EditorStyles.boldLabel);
		} else {

			EditorGUI.LabelField(rect, "ActorComponent", EditorStyles.boldLabel);
		}
	}

	public override void OnInspectorGUI() {
		//DrawDefaultInspector();
		OnHeaderGUI();

		UnityActorComponent uac = target as UnityActorComponent;
		if (uac != null && uac.component != null) {
			var s = CSerializerObjectUnityEditor.Serializer(Controller.MainContext);
			s.InitFoldout(uac.component);
			uac.component.Serialize(s, "Component");
		}

	}
}