using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.Globalization;
using System;

[CustomEditor(typeof(TransparencyCaptureBehaviour))]
public class TransparencyCaptureBehaviourEditor : Editor {

    public override async void OnInspectorGUI() {
        DrawDefaultInspector();

        TransparencyCaptureBehaviour pb = (TransparencyCaptureBehaviour)target;

		IsTransparent = EditorGUILayout.Toggle("Transparent", IsTransparent);
		Scale = EditorGUILayout.FloatField("Scale", Scale);

		if (GUILayout.Button("Take screenshot")) {
			System.DateTime dateTime = System.DateTime.Now;
			var defaultName = $"ubicanvas_{dateTime.ToString("yyyy_MM_dd HH_mm_ss")}";
			string savePath = EditorUtility.SaveFilePanel("Output file", Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), defaultName, "png");
			if (!string.IsNullOrWhiteSpace(savePath)) {
				Resolution res = TransparencyCaptureBehaviour.GetCurrentResolution();
				byte[] screenshotBytes = await pb.Capture((int)(res.width * Scale), (int)(res.height * Scale), true);
				UbiCanvas.Helpers.Util.ByteArrayToFile(savePath, screenshotBytes);
				Debug.Log("Screenshot saved.");
			}
		}

	}

	public float Scale = 1f;
	public bool IsTransparent = false;
	

}