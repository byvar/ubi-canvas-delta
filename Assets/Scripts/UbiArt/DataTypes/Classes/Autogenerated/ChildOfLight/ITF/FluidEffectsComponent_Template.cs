namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class FluidEffectsComponent_Template : ActorComponent_Template {
		public Nullable<FluidEffectsComponent_Template.CollisionRig> collisionRig;
		public CListO<FluidEffectsComponent_Template.HairMeshTemplate> hairMeshTemplateList;
		public CListO<FluidEffectsComponent_Template.TestHairTemplate> testHairTemplateList;
		public CListO<FluidEffectsComponent_Template.Impulse> impulseList;

		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			collisionRig = s.SerializeObject<Nullable<FluidEffectsComponent_Template.CollisionRig>>(collisionRig, name: "collisionRig");
			hairMeshTemplateList = s.SerializeObject<CListO<FluidEffectsComponent_Template.HairMeshTemplate>>(hairMeshTemplateList, name: "hairMeshTemplateList");
			testHairTemplateList = s.SerializeObject<CListO<FluidEffectsComponent_Template.TestHairTemplate>>(testHairTemplateList, name: "testHairTemplateList");
			impulseList = s.SerializeObject<CListO<FluidEffectsComponent_Template.Impulse>>(impulseList, name: "impulseList");
		}
		public override uint? ClassCRC => 0x9B311583;

		[Games(GameFlags.COL)]
		public class CollisionRig : CSerializable {
			public CListO<FluidEffectsComponent_Template.Capsule> capsuleList;
			protected override void SerializeImpl(CSerializerObject s) {
				base.SerializeImpl(s);
				capsuleList = s.SerializeObject<CListO<FluidEffectsComponent_Template.Capsule>>(capsuleList, name: "capsuleList");
			}
		}

		[Games(GameFlags.COL)]
		public class Capsule : CSerializable {
			public StringID boneName;
			public Vec3d pos;
			public float radius;
			public float fluidInfluencePower;
			protected override void SerializeImpl(CSerializerObject s) {
				base.SerializeImpl(s);
				boneName = s.SerializeObject<StringID>(boneName, name: "boneName");
				pos = s.SerializeObject<Vec3d>(pos, name: "pos");
				radius = s.Serialize<float>(radius, name: "radius");
				fluidInfluencePower = s.Serialize<float>(fluidInfluencePower, name: "fluidInfluencePower");
			}
		}

		[Games(GameFlags.COL)]
		public class HairMeshTemplate : CSerializable {
			public CListO<GFXMaterialSerializable> materialList;
			public Path mesh3D;
			public CArrayO<Path> mesh3DList;
			public Path skeleton3D;
			public uint iterations;
			public float fluidInfluence;
			public float elasticity;
			public float flexibility;
			public StringID parentBoneName;
			public int isRelativeToReference;
			public float localRotZ;
			public float rotationY;
			public float rotationZ;
			public Vec3d pos;
			protected override void SerializeImpl(CSerializerObject s) {
				base.SerializeImpl(s);
				materialList = s.SerializeObject<CListO<GFXMaterialSerializable>>(materialList, name: "materialList");
				mesh3D = s.SerializeObject<Path>(mesh3D, name: "mesh3D");
				mesh3DList = s.SerializeObject<CArrayO<Path>>(mesh3DList, name: "mesh3DList");
				skeleton3D = s.SerializeObject<Path>(skeleton3D, name: "skeleton3D");
				iterations = s.Serialize<uint>(iterations, name: "iterations");
				fluidInfluence = s.Serialize<float>(fluidInfluence, name: "fluidInfluence");
				elasticity = s.Serialize<float>(elasticity, name: "elasticity");
				flexibility = s.Serialize<float>(flexibility, name: "flexibility");
				parentBoneName = s.SerializeObject<StringID>(parentBoneName, name: "parentBoneName");
				isRelativeToReference = s.Serialize<int>(isRelativeToReference, name: "isRelativeToReference");
				localRotZ = s.Serialize<float>(localRotZ, name: "localRotZ");
				rotationY = s.Serialize<float>(rotationY, name: "rotationY");
				rotationZ = s.Serialize<float>(rotationZ, name: "rotationZ");
				pos = s.SerializeObject<Vec3d>(pos, name: "pos");
			}
		}

		[Games(GameFlags.COL)]
		public partial class TestHairTemplate : CSerializable {
			public StringID boneName;
			public Vec2d crownOffset;
			public float crownRadius;
			public float crownMinAngle;
			public float crownMaxAngle;
			public uint numStrands;
			public uint numSegments;
			public float segmentLength;
			protected override void SerializeImpl(CSerializerObject s) {
				base.SerializeImpl(s);
				boneName = s.SerializeObject<StringID>(boneName, name: "boneName");
				crownOffset = s.SerializeObject<Vec2d>(crownOffset, name: "crownOffset");
				crownRadius = s.Serialize<float>(crownRadius, name: "crownRadius");
				crownMinAngle = s.Serialize<float>(crownMinAngle, name: "crownMinAngle");
				crownMaxAngle = s.Serialize<float>(crownMaxAngle, name: "crownMaxAngle");
				numStrands = s.Serialize<uint>(numStrands, name: "numStrands");
				numSegments = s.Serialize<uint>(numSegments, name: "numSegments");
				segmentLength = s.Serialize<float>(segmentLength, name: "segmentLength");
			}
		}

		[Games(GameFlags.COL)]
		public partial class Impulse : CSerializable {
			public float offset;
			public float radius;
			public float power;
			public uint instanceMask;
			protected override void SerializeImpl(CSerializerObject s) {
				base.SerializeImpl(s);
				offset = s.Serialize<float>(offset, name: "offset");
				radius = s.Serialize<float>(radius, name: "radius");
				power = s.Serialize<float>(power, name: "power");
				instanceMask = s.Serialize<uint>(instanceMask, name: "instanceMask");
			}
		}
	}
}

