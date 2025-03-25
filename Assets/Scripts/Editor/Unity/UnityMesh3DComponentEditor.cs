using UnityEditor;
using UbiArt;
using UnityEngine;
using System.Linq;
using UbiCanvas.Helpers.FBX;

[CustomEditor(typeof(UnityMesh3DComponent))]
public class UnityMesh3DComponentEditor : Editor {

	public override void OnInspectorGUI() {
		DrawDefaultInspector();

		UnityMesh3DComponent um = target as UnityMesh3DComponent;
		if (um != null && um.m3d != null) {
			if (GUILayout.Button("Export as ASCII FBX")) {
				var m3d = um.m3d;
				string folderPath = EditorUtility.SaveFolderPanel("Output folder", "", "");

				if (!string.IsNullOrWhiteSpace(folderPath)) {
					FBXExporter.ExportFBX(m3d, m3d.tpl, folderPath);
				}
			}
		}

	}
}