using System.Collections.Generic;
using System.Linq;
using UbiArt.Animation;

namespace UbiArt.ITF {
	public partial class AnimLightComponent_Template {

		protected override void OnPreSerialize(CSerializerObject s) {
			base.OnPreSerialize(s);
			Reinit(s.Context, s.Settings);
		}

		Settings previousSettings = null;
		protected virtual void Reinit(Context c, Settings settings) {
			if (previousSettings != null) {
				if (previousSettings.EngineVersion <= EngineVersion.RO && settings.EngineVersion >= EngineVersion.RL) {
					foreach (var anim in animSet.animations) {
						anim.name = new Path($"{animationPath?.FullPath ?? ""}{anim.name?.FullPath ?? ""}");
					}
					animSet.animPackage = new AnimResourcePackage() {
						skeleton = animSet.resourceList.FirstOrDefault(r => r.GetExtension() == "skl"),
						animPathAABB = new CListO<AnimPathAABB>(),
						textureBank = new CListO<TextureBankPath>(),
						needPack = true,
						packed = true,
						fromHD = true
					};
					var blend = blendmode2;
					string shaderPath = blend switch {
						GraphicComponent_Template.GFX_BLEND2.ADDALPHA => "world/common/matshader/addalpha.msh",
						GraphicComponent_Template.GFX_BLEND2.ADD => "world/common/matshader/add.msh",
						GraphicComponent_Template.GFX_BLEND2.MUL => "world/common/matshader/mul.msh",
						GraphicComponent_Template.GFX_BLEND2.MUL2X => "world/common/matshader/mul2x.msh",
						//_ => "world/common/matshader/default.msh",
						_ => "world/common/matshader/regularbuffer/backlighted.msh"
					};

					// Create texture bank ID list
					//Dictionary<StringID, Path> texturePaths = new Dictionary<StringID, Path>();
					Dictionary<Path, StringID> texturePaths = new Dictionary<Path, StringID>();
					for (int i = 0; i < animSet.resourceList.Count; i++) {
						if (animSet.resourceList[i].GetExtension() != "anm") continue;
						var anm = animSet.resourceList[i].GetObject<AnimTrack>();
						animSet.animPackage.animPathAABB.Add(new AnimPathAABB() {
							name = animSet.resourceList[i].GetFilenameWithoutExtension(),
							path = animSet.resourceList[i],
							aabb = //animSet.animAABB //anm.LocalAABB
							new AABB() {
								MIN = Vec2d.MinusInfinity,
								MAX = Vec2d.Infinity
							}
						});
						for (int j = 0; j < anm.texturePathKeysOrigins.keysLegends.Count; j++) {
							var texPath = new Path(anm.texturePathsOrigins[j].Item2);
							texturePaths[texPath] = anm.texturePathKeysOrigins.keysLegends[j];
							//texturePaths[anm.texturePathKeysOrigins.keysLegends[j]] = new Path(anm.texturePathsOrigins[j].Item2);
						}
					}
					for (int i = 0; i < animSet.resourceList.Count; i++) {
						var res = animSet.resourceList[i];
						if (res.GetExtension() is ("png" or "tga")) {
							var texPath = res;
							var bankId = texPath.stringID;
							if(texturePaths.ContainsKey(texPath))
								bankId = texturePaths[texPath];
							else if(texturePaths.Count == 1)
								bankId = texturePaths.FirstOrDefault().Value;

							var pbkPathLower = $"{texPath.GetFilenameWithoutExtension(fullPath: true).ToLowerInvariant()}.pbk";
							for (int j = 0; j < animSet.resourceList.Count; j++) {
								if(animSet.resourceList[j].FullPath.ToLowerInvariant() == pbkPathLower) {
									var pbkPath = animSet.resourceList[j];

									animSet.animPackage.textureBank.Add(new TextureBankPath() {
										id = bankId,
										patchBank = pbkPath,
										textureSet = new GFXMaterialTexturePathSet() {
											diffuse = texPath
										},
										materialShader = new Path(shaderPath)
									});
									break;
								}
							}
						}
					}
				}
			}
			previousSettings = settings;
		}
	}
}
