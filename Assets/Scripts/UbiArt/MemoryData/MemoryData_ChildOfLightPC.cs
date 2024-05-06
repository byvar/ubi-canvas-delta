using System;
using System.Collections.Generic;

namespace UbiArt {
	public class MemoryData_ChildOfLightPC : IMemoryData {
		#region Data
		public static readonly Dictionary<Type, uint> EditorSizes = new Dictionary<Type, uint> {
			// Main
			[typeof(StringID)] = 8,
			[typeof(Path)] = 148, // 140 + StringID
			[typeof(LocalizedPath)] = 152, // Path + LocId
			[typeof(LocalisationId)] = 4,
			[typeof(ColorInteger)] = 4,
			[typeof(string)] = 20,
			[typeof(Vec2d)] = 8,
			[typeof(Vec3d)] = 12,
			[typeof(Vec4d)] = 16,
			[typeof(Color)] = 16,
			[typeof(Angle)] = 4,
			[typeof(AngleAmount)] = 4,
			[typeof(ObjectId)] = 8,
			[typeof(ObjectRef)] = 4,
			[typeof(PathRef)] = 4,
			[typeof(Platform)] = 4,
			[typeof(SoundGUID)] = 4,
			[typeof(Volume)] = 4,

			// From game data
			[typeof(ITF.FriseConfig)] = 1200,
			[typeof(ITF.CollisionFrieze)] = 44,
			[typeof(ITF.FillConfig)] = 40,
			[typeof(ITF.FriseTextureConfig)] = 1852,
			[typeof(ITF.GFXMaterialSerializable)] = 1660,
			[typeof(ITF.GFXMaterialTexturePathSet)] = 900,
			[typeof(ITF.GFXMaterialSerializableParam)] = 24,
			[typeof(ITF.CollisionTexture)] = 32,
			[typeof(ITF.VertexAnim)] = 44,
			[typeof(ITF.FluidConfig)] = 276,
			[typeof(ITF.FluidFriseLayer)] = 268,
			[typeof(ITF.GFXPrimitiveParam)] = 80,
			[typeof(ITF.Actor_Template)] = 188,
			[typeof(ITF.FogBoxComponent_Template)] = 16,
			[typeof(ITF.FXControllerComponent_Template)] = 140,
			[typeof(ITF.FXControl)] = 128,
			[typeof(ITF.FxBankComponent_Template)] = 1900,
			[typeof(ITF.AABB)] = 16,
			[typeof(ITF.FxDescriptor_Template)] = 5260,
			[typeof(ITF.ITF_ParticleGenerator_Template)] = 3288,
			[typeof(ITF.ParticleGeneratorParameters)] = 3012,
			[typeof(ITF.ParPhase)] = 92,
			[typeof(ITF.ParLifeTimeCurve)] = 72,
			[typeof(ITF.Spline)] = 36,
			[typeof(ITF.EmitLifeTimeCurve)] = 72,
			[typeof(ITF.ProceduralInputData)] = 36,
			[typeof(ITF.GFXMaterialShader_Template)] = 548,
			[typeof(ITF.GFX_MaterialParams)] = 104,
			[typeof(ITF.COL_GFXMaterialShader_Layer_Template)] = 60,
			[typeof(ITF.COL_GameMaterial_Template)] = 244,
			[typeof(ITF.ClearColorComponent_Template)] = 16,
			[typeof(ITF.TweenComponent_Template)] = 120,
			[typeof(ITF.LinkComponent_Template)] = 56,
			[typeof(ITF.SoundComponent_Template)] = 112,
			[typeof(ITF.TextureGraphicComponent_Template)] = 3520,
			[typeof(ITF.COL_PhysBodyComponent_Template)] = 24,
			[typeof(ITF.PhysShapeCircle)] = 8,
			[typeof(ITF.COL_PendulumComponent_Template)] = 32,
			[typeof(ITF.LinkCurveComponent_Template)] = 3524,
			[typeof(ITF.SoundDescriptor_Template)] = 96,
			[typeof(ITF.WwiseInputDesc)] = 28,
			[typeof(ITF.AnimLightComponent_Template)] = 2352,
			[typeof(ITF.SubAnimSet_Template)] = 416,
			[typeof(ITF.SubAnim_Template)] = 188,
			[typeof(ITF.AnimResourcePackage)] = 240,
			[typeof(ITF.TextureBankPath)] = 1108,
			[typeof(ITF.AnimPathAABB)] = 124,
			[typeof(ITF.COL_PointOfInterestComponent_Template)] = 20,
			[typeof(ITF.PhysShapePolygon)] = 96,
		};

		public static readonly Dictionary<Type, uint> RetailSizes = new Dictionary<Type, uint>() {
			// Main
			[typeof(StringID)] = 4,
			[typeof(Path)] = 144, // 140 + StringID
			[typeof(LocalizedPath)] = 148, // Path + LocId
			[typeof(LocalisationId)] = 4,
			[typeof(ColorInteger)] = 4,
			[typeof(string)] = 20,
			[typeof(Vec2d)] = 8,
			[typeof(Vec3d)] = 12,
			[typeof(Vec4d)] = 16,
			[typeof(Color)] = 16,
			[typeof(Angle)] = 4,
			[typeof(AngleAmount)] = 4,
			[typeof(ObjectId)] = 8,
			[typeof(ObjectRef)] = 4,
			[typeof(PathRef)] = 4,
			[typeof(Platform)] = 4,
			[typeof(SoundGUID)] = 4,
			[typeof(Volume)] = 4,

			// From IDA
		};

		public static readonly Dictionary<Type, uint> RetailAlign = new Dictionary<Type, uint>() {
			// ObjectFactory
			[typeof(ITF.Scene)] = 16,


			// Not ObjectFactory
			[typeof(ITF.Trajectory)] = 16,
			[typeof(ITF.Message)] = 8,
			[typeof(ITF.UIScrollbar_Template.Style)] = 1,
			[typeof(ITF.Frise.MeshStaticData)] = 16,
			[typeof(ITF.Frise.MeshAnimData)] = 16,
			[typeof(ITF.Frise.MeshOverlayData)] = 16,
		};
		#endregion

		public const bool UseEditorSizes = true;

		public bool TryGetSize(Type type, out uint size) {
			uint? sizeOf = null;
			size = 0;
			if (Type.GetTypeCode(type) != TypeCode.Object) {
				sizeOf = 4;
				switch (Type.GetTypeCode(type)) {
					case TypeCode.Boolean: sizeOf = 4; break;
					case TypeCode.Byte: sizeOf = 1; break;
					case TypeCode.Char: sizeOf = 1; break;
					case TypeCode.String: sizeOf = 20; break; // sizeof(String8) // IN RL
					case TypeCode.Double: sizeOf = 8; break;
					case TypeCode.UInt16: sizeOf = 2; break;
					case TypeCode.UInt64: sizeOf = 8; break;
					case TypeCode.Int16: sizeOf = 2; break;
					case TypeCode.Int64: sizeOf = 8; break;
				}
			} else if (typeof(IObjectContainer).IsAssignableFrom(type)) {
				sizeOf = 4;
				if (type.IsGenericType) {
					var baseType = type.GetGenericTypeDefinition();
					// Could probably be written better
					if (baseType == typeof(CList<>) || baseType == typeof(CArray<>)
						|| baseType == typeof(CListO<>) || baseType == typeof(CListP<>) || baseType == typeof(CRefList<>)
						|| baseType == typeof(CArrayO<>) || baseType == typeof(CArrayP<>)) {
						sizeOf = 20;
					}
				}
			}
			
			if(!sizeOf.HasValue) {
				if (UseEditorSizes && EditorSizes.ContainsKey(type)) {
					sizeOf = EditorSizes[type];
				}
				if (!sizeOf.HasValue && RetailSizes.ContainsKey(type)) {
					sizeOf = RetailSizes[type];
				}
			}

			if (sizeOf.HasValue) {
				size = sizeOf.Value;
				return true;
			}
			return false;
		}


		public bool TryGetAlign(Type type, out uint align) {
			if (RetailAlign.ContainsKey(type)) {
				align = RetailAlign[type];
				return true;
			}
			align = 0;
			return false;
		}
	}
}