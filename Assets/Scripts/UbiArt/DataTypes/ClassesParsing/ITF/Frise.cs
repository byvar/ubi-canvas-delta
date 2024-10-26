﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace UbiArt.ITF {
	public partial class Frise {
		public GenericFile<FriseConfig> config;
		public FriseOrigins.FriseConfigOrigins configOrigins;
		public GenericFile<GFXMaterialShader_Template> shader;

		protected override void OnPostSerialize(CSerializerObject s) {
			base.OnPostSerialize(s);
			if (IsFirstLoad) {
				Loader l = s.Context.Loader;

				if (s.Settings.EngineVersion > EngineVersion.RO) {
					l.LoadFile<GenericFile<GFXMaterialShader_Template>>(MatShader, result => shader = result);

					l.LoadFile<GenericFile<FriseConfig>>(ConfigName, result => {
						config = result;
						if (config != null)
							templatePickable = config.obj;
					});
				} else {
					l.LoadFile<FriseOrigins.FriseConfigOrigins>(ConfigName, result => {
						configOrigins = result;
					});
				}
			}
		}

		protected override void Reinit(Context c, Settings settings) {
			if (previousSettings != null) {
				if (previousSettings.Game != settings.Game) {
					if ((previousSettings.Game == Game.RA || previousSettings.Game == Game.RM) &&
						!(settings.Game == Game.RA || settings.Game == Game.RM)) {
						// Check components
						if (COMPONENTS != null && COMPONENTS.Count > 0) {
							c.SystemLogger?.LogWarning($"Frise with components: {USERFRIENDLY}");
						}
						// Check parentBind
						if (parentBind != null && !parentBind.IsNull) {
							c.SystemLogger?.LogWarning($"Frise with parentBind: {USERFRIENDLY}");
						}
					}
					if (previousSettings.EngineVersion == EngineVersion.RO && settings.EngineVersion == EngineVersion.RL) {
						PointsList = new PolyPointList() {
							LocalPoints = LOCAL_POINTS,
							Loop = LOOP
						};
						PointsList.RecomputeData();
						PreComputedForCook = false;
						meshBuildData = new Nullable<MeshBuildData>(new MeshBuildData());
						collisionData = new Nullable<CollisionData>(new CollisionData());
						meshStaticData = new Nullable<MeshStaticData>(new MeshStaticData());
						meshAnimData = new Nullable<MeshAnimData>(new MeshAnimData());
						meshFluidData = new Nullable<MeshFluidData>(new MeshFluidData());
						meshOverlayData = new Nullable<MeshOverlayData>(new MeshOverlayData());
						if (UseTemplatePrimitiveParams) { // Hack to see if frise has been modified or not
							UseTemplatePrimitiveParams = false;
							useStaticFog = true;
						}
					}
				}
			}
			base.Reinit(c, settings);
		}

		public void InitForNewFrise() {
			LOCAL_POINTS = new CListO<PolyLineEdge>();
			PointsList = new PolyPointList() {
				LocalPoints = LOCAL_POINTS,
				Loop = LOOP
			};
			//PointsList.RecomputeData();
			PreComputedForCook = false;
			meshBuildData = new Nullable<MeshBuildData>(new MeshBuildData());
			collisionData = new Nullable<CollisionData>(new CollisionData());
			meshStaticData = new Nullable<MeshStaticData>(new MeshStaticData());
			meshAnimData = new Nullable<MeshAnimData>(new MeshAnimData());
			meshFluidData = new Nullable<MeshFluidData>(new MeshFluidData());
			meshOverlayData = new Nullable<MeshOverlayData>(new MeshOverlayData());
			UseTemplatePrimitiveParams = true;
			//useStaticFog = true;
		}

		protected void RecomputeLineData() {
			PointsList?.CheckLoop();
			PointsList?.RecomputeData();
		}

		public void Recompute() {
			RecomputeLineData();
			if(config?.obj == null) return;
			var friseConfig = config?.obj;

			// Init datas
			RecomputeData recomputeData = new RecomputeData(this);

			var method = (FriseMethode)friseConfig.methode;
			// TODO
			switch (method) {
				case FriseMethode.Generic:
					break;
			}
		}

		private void BuildFrieze_InGeneric(RecomputeData recomputeData) {
			recomputeData.CopyEdgesFromPolyline();
			// TODO:
		}

		public enum FriseMethode : uint {
			 Roundness = 0,
			 Generic,
			 Extremity,
			 Archi,
			 Fluid,
			 Pipe,
			 ExtremitySimple,
			 String,
			 Atlas,
			 Overlay,
			 Frame,
			 ArchiSimple,
			 PipePatchSimple,
			 Extremity13,
			 Bezier3D,
		}
	}
}
