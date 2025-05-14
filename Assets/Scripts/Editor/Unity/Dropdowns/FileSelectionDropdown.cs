﻿
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
			output.AddRange(from file in Directory.EnumerateFiles(directory, extension, SearchOption.AllDirectories) select file.Substring(directory.Length).Replace(System.IO.Path.DirectorySeparatorChar, '/'));
		}
		// Return the output
		return output;
	}

	protected override AdvancedDropdownItem BuildRoot() {
		var root = new AdvancedDropdownItem(name);
		for (int i = 0; i < files.Length; i++) {
			Add(root, files[i], files[i], i);
		}

		return root;
	}

	protected void Add(AdvancedDropdownItem parent, string path, string fullPath, int id) {
		if (path.Contains("/")) {
			// Folder
			string folder = path.Substring(0, path.IndexOf("/"));
			string rest = path.Substring(path.IndexOf("/") + 1);
			AdvancedDropdownItem folderNode = parent.children.FirstOrDefault(c => c.name == folder);
			if (folderNode == null) {
				folderNode = new AdvancedDropdownItem(folder);
				parent.AddChild(folderNode);
			}
			Add(folderNode, rest, fullPath, id);
		} else {
			// File
			var name = displayFullPaths ? fullPath : path;
			name = name?.RemoveCKD();
			parent.AddChild(new AdvancedDropdownItem(name) {
				id = id
			});
		}
	}

	protected override void ItemSelected(AdvancedDropdownItem item) {
		base.ItemSelected(item);
		if (item.children.Count() == 0 && files != null && files.Length > item.id) {
			selection = files[item.id];
			//property.stringValue = selection;
		}
	}
}
