﻿using UnityEngine;
using UbiArt;
using UbiArt.ITF;

public class UnityTextureGraphicComponent : MonoBehaviour {
	public TextureGraphicComponent tgc;
	float prevZ = -99999;

	private void Update() {
		if (tgc != null && tgc.tex_renderer != null) {
			tgc?.FillMaterialParams(tgc?.tex_renderer);

			float z = transform.position.z;
			if (z != prevZ) {
				prevZ = z;
				ZListManager zman = Controller.Obj.zListManager;
				zman.zDict[tgc.tex_renderer] = z;
				//zman.zDict[tgc.tex_mat] = z;
			}
		}
	}
}
