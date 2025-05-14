using UbiArt;
using UbiCanvas.Helpers;
using UnityEditor;
using UnityEngine;
using MemoryStream = System.IO.MemoryStream;

public class UnityWindowSerializableEditor : UnityWindow
{
	protected override bool AllowScroll => false; // Handle scroll manually due to GUILayout being used

	private Vector2 _scrollPosition;

	// Set these when creating the window!
	public Path Path { get; set; }
	public Context Context => _context;
	private Context _context;
	public void SetContext(Context context) {
		if (_context != null) {
			_context.Dispose();
		}
		_context = context;
	}

	private void OnEnable()
	{
		titleContent = EditorGUIUtility.IconContent("TextAsset Icon");
		titleContent.text = "Serializer";
	}

	protected override void UpdateEditorFields()
	{
		base.UpdateEditorFields();

		if (Path?.Object != null) {
			// Increase label width due to it being cut off otherwise
			EditorGUIUtility.labelWidth = 300;

			EditorGUI.BeginDisabledGroup(true);
			EditorGUILayout.TextField("File", Path.FullPath);
			EditorGUI.EndDisabledGroup();

			if (GUILayout.Button("Save to file")) {
				var cookedPath = Path.CookedPath(Context);
				var cookedName = cookedPath.filename;
				string filePath = EditorUtility.SaveFilePanel("Output file", "", cookedName, cookedPath.GetExtension());

				if (!string.IsNullOrWhiteSpace(filePath)) {
					using (Context) {
						byte[] serializedData = null;
						using (MemoryStream stream = new MemoryStream()) {
							using (Writer writer = new Writer(stream, Context.Settings.IsLittleEndian)) {
								CSerializerObject w = Context.Loader.CreateSerializerForPath(Path.UncookedPath(Context), writer);
								object toWrite = w.Serialize(Path.Object, Path.Object.GetType(), name: Path.filename);
								serializedData = stream.ToArray();
							}
						}
						Util.ByteArrayToFile(filePath, serializedData);
					}
				}

			}

			_scrollPosition = GUILayout.BeginScrollView(_scrollPosition, false, true);
			Path.Object.Serialize(CSerializerObjectUnityEditor.Serializer(Context), Path.filename);
			GUILayout.EndScrollView();
		}
	}
}