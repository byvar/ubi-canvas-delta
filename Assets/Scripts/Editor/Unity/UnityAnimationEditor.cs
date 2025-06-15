using UnityEditor;
using UbiArt;
using UnityEngine;
using System.Linq;
using UbiCanvas.Helpers;

[CustomEditor(typeof(UnityAnimation))]
public class UnityAnimationEditor : Editor {

	public override void OnInspectorGUI() {
		DrawDefaultInspector();

		UnityAnimation ua = target as UnityAnimation;
		if (ua != null && ua.anims != null && ua.animsFull != null) {
			var anims = ua.PlayFullAnimation ? ua.animsFull : ua.anims;
			var newInd = ua.animIndex;
			string locIdPreview = "Bind Pose";
			if(newInd >= 0 && newInd < (ua.anims?.Length ?? 0))
				locIdPreview = ua.anims[newInd].ToString(ua.PlayFullAnimation);

			EditorGUI.indentLevel = 0;
			Rect rect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);
			rect = EditorGUI.PrefixLabel(rect, new GUIContent(name));
			if (EditorGUI.DropdownButton(rect, new GUIContent(locIdPreview), FocusType.Passive)) {
				if (Dropdown == null || Dropdown.PlayFullAnimation != ua.PlayFullAnimation) {
					Dropdown = new AnimationsDropdown(new UnityEditor.IMGUI.Controls.AdvancedDropdownState(), anims) {
						name = "Animations",
						PlayFullAnimation = ua.PlayFullAnimation
					};
				}
				Dropdown.rect = rect;
				Dropdown.Show(rect);
			}
			if (Dropdown != null && Dropdown.selection != null && Dropdown.rect == rect) {
				newInd = Dropdown.selection.Value;
				Dropdown.selection = null;
			}

			if (ua.PlayFullAnimation && newInd != -1) {
				var fullInd = anims.FindItemIndex(i => i.Index == newInd);
				if (fullInd == -1)
					fullInd = anims.FindItemIndex(i => i.Path == ua.anims[newInd].Path);
				if (fullInd != -1) {
					newInd = fullInd;
				}
			}

			// Next/previous buttons
			GUILayout.BeginHorizontal();
			GUI.enabled = newInd >= 0; // Allow -1 = bind pose
			if (GUILayout.Button("Previous")) newInd--;
			GUI.enabled = (newInd < anims.Length - 1);
			if (GUILayout.Button("Next")) newInd++;
			GUI.enabled = true;
			GUILayout.EndHorizontal();

			// Update anim index
			if (newInd == -1) {
				if (ua.animIndex != -1) {
					ua.animIndex = -1;
					ua.Init();
				}
			} else {
				var anim = anims[newInd];
				if (ua.animIndex != anim.Index) {
					ua.animIndex = anim.Index;
					ua.Init();
				}
			}
		}

	}

	private AnimationsDropdown Dropdown { get; set; }
}