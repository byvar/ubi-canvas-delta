using static UbiArt.ITF.Frise;

namespace UbiArt.ITF {
	public partial class FriseConfig {
		GenericFile<GameMaterial_Template> loaded_gameMaterial;
		GenericFile<GameMaterial_Template> loaded_gameMaterialExtremityStart;
		GenericFile<GameMaterial_Template> loaded_gameMaterialExtremityStop;

		protected override void OnPostSerialize(CSerializerObject s) {
			base.OnPostSerialize(s);
			if (IsFirstLoad) {
				Loader l = s.Context.Loader;

				l.LoadFile<GenericFile<GameMaterial_Template>>(gameMaterial, result => loaded_gameMaterial = result);
				l.LoadFile<GenericFile<GameMaterial_Template>>(gameMaterialExtremityStart, result => loaded_gameMaterialExtremityStart = result);
				l.LoadFile<GenericFile<GameMaterial_Template>>(gameMaterialExtremityStop, result => loaded_gameMaterialExtremityStop = result);

			}
		}

		public Frise InstantiateFrise(Path templatePath) {
			var basename = templatePath?.GetFilenameWithoutExtension(removeCooked: true);
			Frise frz = new Frise() {
				POS2D = Vec2d.Zero,
				USERFRIENDLY = basename,
				LUA = templatePath,
				ConfigName = templatePath,
				config = new GenericFile<FriseConfig>(this),
				templatePickable = this,
				PointsList = new PolyPointList() {
					LocalPoints = new CListO<PolyLineEdge>()
				},
				meshBuildData = new Nullable<MeshBuildData>(new MeshBuildData()),
				collisionData = new Nullable<CollisionData>(new CollisionData()),
				meshStaticData = new Nullable<MeshStaticData>(new MeshStaticData()),
				meshAnimData = new Nullable<MeshAnimData>(new MeshAnimData()),
				meshFluidData = new Nullable<MeshFluidData>(new MeshFluidData()),
				meshOverlayData = new Nullable<MeshOverlayData>(new MeshOverlayData()),
			};
			frz.InitContext(UbiArtContext);
			return (Frise)frz.Clone("frz");
		}
	}
}
