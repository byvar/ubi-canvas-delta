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
