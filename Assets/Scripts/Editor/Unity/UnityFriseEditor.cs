using UnityEditor;
using UbiArt;
using UnityEngine;

[CustomEditor(typeof(UnityFrise))]
public class UnityFriseEditor : Editor {
	/*
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
		if (uac != null && uac.component != null && !uac.component.IsNull) {
			string header = uac.component.obj.GetType().Name;
			EditorGUI.LabelField(rect, header, EditorStyles.boldLabel);
		} else {

			EditorGUI.LabelField(rect, "ActorComponent", EditorStyles.boldLabel);
		}
	}*/

	public override void OnInspectorGUI() {
		//DrawDefaultInspector();
		//OnHeaderGUI();

		UnityFrise frise = target as UnityFrise;
		if (frise != null && frise.frise != null) {
			var s = CSerializerObjectUnityEditor.Serializer(Controller.MainContext);
			s.InitFoldout(frise.frise);
			frise.frise.Serialize(s, "Frise");
		}

	}
}