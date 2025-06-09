using UnityEditor;
using UbiArt;
using UnityEngine;
using System.Linq;

[CustomEditor(typeof(UnityAnimation))]
public class UnityAnimationEditor : Editor {

	public override void OnInspectorGUI() {
		DrawDefaultInspector();

		UnityAnimation ua = target as UnityAnimation;
		if (ua != null && ua.anims != null) {
			var newInd = ua.animIndex;
			string locIdPreview = "Bind Pose";
			if(newInd >= 0 && newInd < (ua.anims?.Length ?? 0))
				locIdPreview = ua.anims[newInd].ToString();

			EditorGUI.indentLevel = 0;
			Rect rect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);
			rect = EditorGUI.PrefixLabel(rect, new GUIContent(name));
			if (EditorGUI.DropdownButton(rect, new GUIContent(locIdPreview), FocusType.Passive)) {
				if (Dropdown == null) {
					Dropdown = new AnimationsDropdown(new UnityEditor.IMGUI.Controls.AdvancedDropdownState(), ua.anims) {
						name = "Animations"
					};
				}
				Dropdown.rect = rect;
				Dropdown.Show(rect);
			}
			if (Dropdown != null && Dropdown.selection != null && Dropdown.rect == rect) {
				newInd = Dropdown.selection.Value;
				Dropdown.selection = null;
			}

			GUILayout.BeginHorizontal();
			GUI.enabled = newInd >= 0; // Allow -1 = bind pose
			if (GUILayout.Button("Previous")) newInd--;
			GUI.enabled = (newInd < ua.anims.Length - 1);
			if (GUILayout.Button("Next")) newInd++;
			GUI.enabled = true;
			GUILayout.EndHorizontal();
			if (ua.animIndex != newInd) {
				ua.animIndex = newInd;
				ua.Init();
			}
		}

	}

	private AnimationsDropdown Dropdown { get; set; }
}