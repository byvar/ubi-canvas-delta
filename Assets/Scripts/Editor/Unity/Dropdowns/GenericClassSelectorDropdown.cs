﻿
using UnityEditor.IMGUI.Controls;
using System.Collections.Generic;
using System.Linq;
using UbiArt;
using UnityEngine;
using System;
using UbiCanvas.Helpers;

class GenericClassSelectorDropdown : AdvancedDropdown {
	public uint? selection = null;
	public string name;
	public Rect rect;
	public Type type;
	public Context context;

	public GenericClassSelectorDropdown(Context context, AdvancedDropdownState state) : base(state) {
		this.context = context;
		minimumSize = new UnityEngine.Vector2(50, 400f);
	}

	protected override AdvancedDropdownItem BuildRoot() {
		var root = new AdvancedDropdownItem(name);
		root.AddChild(new AdvancedDropdownItem("None") {
			id = -1
		});
		
		List<KeyValuePair<uint, Type>> types = new List<KeyValuePair<uint, Type>>();
		foreach (var c in ObjectFactory.classes) {
			if (type.IsAssignableFrom(c.Value)) {
				var attr = (GamesAttribute)Attribute.GetCustomAttribute(c.Value, typeof(GamesAttribute));
				if (attr != null) {
					var settings = context?.Settings;
					if (settings != null) {
						if (!attr.HasGame(settings.Game) || !attr.HasPlatform(settings.Platform)) {
							// This generic class is not supported by the current mode. Don't show it
							continue;
						}
					}
				}
				types.Add(c);
			}
		}
		int CompareTypeName(KeyValuePair<uint, Type> a, KeyValuePair<uint, Type> b) {
			return a.Value.GetFormattedName().CompareTo(b.Value.GetFormattedName());
		}
		types.Sort(CompareTypeName);

		foreach (var t in types) {
			root.AddChild(new AdvancedDropdownItem(t.Value.GetFormattedName()) {
				id = (int)t.Key
			});
		}

		return root;
	}

	protected override void ItemSelected(AdvancedDropdownItem item) {
		base.ItemSelected(item);
		if (item.children.Count() == 0) {
			selection = (uint)item.id;
		}
	}
}
