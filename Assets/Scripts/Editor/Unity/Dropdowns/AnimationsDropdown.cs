
using UnityEditor.IMGUI.Controls;
using System.Collections.Generic;
using System.Linq;
using UbiArt;
using UnityEngine;
using UbiArt.Animation;

class AnimationsDropdown : AdvancedDropdown {
	public int? selection;
	public string name;
	public Rect rect;
	public UnityAnimation.UnityAnimationTrack[] animations;
	public bool PlayFullAnimation;

	public AnimationsDropdown(AdvancedDropdownState state) : base(state) {
		minimumSize = new UnityEngine.Vector2(50, 400f);
	}
	public AnimationsDropdown(AdvancedDropdownState state, UnityAnimation.UnityAnimationTrack[] animations) : this(state) {
		this.animations = animations;
	}

	protected override AdvancedDropdownItem BuildRoot() {
		var root = new AdvancedDropdownItem(name);
		root.AddChild(new AdvancedDropdownItem("Bind Pose") {
			id = -1
		});
		if (animations != null) {
			for (int i = 0; i < animations.Length; i++) {
				var a = animations[i];

				var name = a.ToString(PlayFullAnimation);
				root.AddChild(new AdvancedDropdownItem(name) {
					id = a.Index
				});
			}
		}

		return root;
	}

	protected override void ItemSelected(AdvancedDropdownItem item) {
		base.ItemSelected(item);
		if (item.children.Count() == 0) {
			selection = item.id;
		}
	}
}
