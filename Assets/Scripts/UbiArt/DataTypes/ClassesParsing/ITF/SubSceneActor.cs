﻿using Cysharp.Threading.Tasks;

namespace UbiArt.ITF {
	public partial class SubSceneActor {
		ContainerFile<Scene> sceneFile;

		public static string DefaultPath => "enginedata/actortemplates/subscene.tpl";

		public Scene GetScene() => SCENE?.value ?? SCENE_ORIGINS?.obj;

		protected override void OnPostSerialize(CSerializerObject s) {
			base.OnPostSerialize(s);
			if (IsFirstLoad && !EMBED_SCENE) {
				Loader l = s.Context.Loader;
				l.LoadFile<ContainerFile<Scene>>(RELATIVEPATH, result => {
					sceneFile = result;

					if (sceneFile != null) {
						if (l.Settings.EngineVersion <= EngineVersion.RO) {
							SCENE_ORIGINS = new Generic<Scene>(sceneFile.obj);
							SCENE = new Nullable<Scene>(sceneFile.obj);
						} else {
							SCENE = new Nullable<Scene>(sceneFile.obj);
						}
					}
				});
			}
			if (EMBED_SCENE && (SCENE == null || SCENE.IsNull)) {
				SCENE = new Nullable<Scene>(new Scene());
				SCENE.value.InitContext(s.Context);
			}
			if (s.Context.Settings.EngineVersion <= EngineVersion.RO) {
				SCENE = new Nullable<Scene>(SCENE_ORIGINS?.obj);
			}
		}
	}
}
