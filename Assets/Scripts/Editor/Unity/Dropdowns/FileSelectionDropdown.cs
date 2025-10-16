
using UnityEditor.IMGUI.Controls;
using System.Collections.Generic;
using System.Linq;
using UbiArt;
using System.IO;
using UnityEngine;
using UbiCanvas.Helpers;

class FileSelectionDropdown : AdvancedDropdown {
	public string selection = null;
	public string directory;
	public string[] extensions;
	public Mode mode;
	public bool displayFullPaths;
	public string[] files;
	public string name;
	//public SerializedProperty property;
	public bool allowDirectorySelection = false;

	public FileSelectionDropdown(AdvancedDropdownState state) : base(state) {
		minimumSize = new UnityEngine.Vector2(50, 400f);
	}
	public FileSelectionDropdown(AdvancedDropdownState state, string directory, params string[] extensions) : this(state) {
		this.directory = directory;
		this.extensions = extensions;
		files = FindFiles().ToArray();
	}

	private List<string> FindFiles() {
		// Create the output
		var output = new List<string>();

		// If the directory does not exist, return the empty list
		if (!Directory.Exists(directory))
			return output;

		// Add the found files containing the correct file extension
		foreach (var extension in extensions) {
			output.AddRange(
				from file in Directory.EnumerateFiles(directory, extension, SearchOption.AllDirectories)
				select file.Substring(directory.Length).Replace(System.IO.Path.DirectorySeparatorChar, '/')
			);
		}
		// Return the output
		return output;
	}

	protected override AdvancedDropdownItem BuildRoot() {
		var root = new FileDropdownItem(name) {
			path = "",
			type = FileDropdownItem.NodeType.Folder
		};
		// Add a child for the directory itself, ifneedbe
		if (allowDirectorySelection) {
			string selectDisplay = $"/*.*";
			var selectNode = new FileDropdownItem(selectDisplay) {
				path = "",
				type = FileDropdownItem.NodeType.FolderSelect,
				fileIndex = -1
			};
			root.AddChild(selectNode);
		}
		for (int i = 0; i < files.Length; i++) {
			Add(root, files[i], files[i], i, "");
		}

		return root;
	}

	protected void Add(FileDropdownItem parent, string path, string fullPath, int id, string currentRelativePath) {
		if (path.Contains("/")) {
			// Folder
			string folder = path.Substring(0, path.IndexOf("/"));
			string rest = path.Substring(path.IndexOf("/") + 1);
			
			string curPath = string.IsNullOrEmpty(currentRelativePath) ? folder : $"{currentRelativePath}/{folder}";

			var folderNode = parent.children.OfType<FileDropdownItem>().FirstOrDefault(c => c.name == folder && c.type == FileDropdownItem.NodeType.Folder);
			if (folderNode == null) {
				// Folder doesn't exist yet
				string displayName = displayFullPaths ? curPath : folder;

				folderNode = new FileDropdownItem(displayName) {
					path = curPath,
					type = FileDropdownItem.NodeType.Folder
				};

				parent.AddChild(folderNode);

				// Add a child for the directory itself, ifneedbe
				if (allowDirectorySelection) {
					string selectDisplay = $"{folderNode.path}/*.*";
					var selectNode = new FileDropdownItem(selectDisplay) {
						path = folderNode.path,
						type = FileDropdownItem.NodeType.FolderSelect,
						fileIndex = -1
					};
					folderNode.AddChild(selectNode);
				}
			}

			Add(folderNode, rest, fullPath, id, curPath);
		} else {
			// File
			string displayName = displayFullPaths ? fullPath : path;
			displayName = displayName?.RemoveCKD();

			var fileNode = new FileDropdownItem(displayName) {
				path = fullPath,
				type = FileDropdownItem.NodeType.File,
				fileIndex = id,
			};

			parent.AddChild(fileNode);
		}
	}

	protected override void ItemSelected(AdvancedDropdownItem item) {
		base.ItemSelected(item);

		var fItem = item as FileDropdownItem;
		if (fItem == null)
			return;

		switch (fItem.type) {
			case FileDropdownItem.NodeType.File:
				if (files != null && fItem.fileIndex >= 0 && fItem.fileIndex < files.Length) {
					selection = files[fItem.fileIndex];
					return;
				}
				break;
			case FileDropdownItem.NodeType.FolderSelect:
				if(allowDirectorySelection) {
					selection = $"{fItem.path}/*.*";//fItem.path;
					return;
				}
				break;
		}
		// Otherwise ignore selection
	}

	protected class FileDropdownItem : AdvancedDropdownItem {
		public string path;
		public NodeType type;
		public int fileIndex = -1;

		public FileDropdownItem(string displayName) : base(displayName) { }

		public enum NodeType {
			File,
			Folder,
			FolderSelect
		}
	}
}
