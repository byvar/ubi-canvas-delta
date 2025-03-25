using System;
using System.Linq;
using UbiArt.ITF;

namespace UbiArt.Animation {
	// See: ITF::AnimTrack::serialize
	// anm.ckd file
	public class AnimTrack : CSerializable {
		public const uint VersionLegends = 34;

		public uint version;
		public float length;
		public CListO<AnimTrackBML> bml;
		public CListO<AnimTrackBonePAS> bonePAS;
		public CListO<AnimTrackBoneZAL> boneZAL;
		public float multiplierA;
		public float multiplierP;
		public float multiplierS;
		public CListO<AnimTrackPolyline> polylines;
		public CListO<AnimTrackBonesList> bonesLists;
		public CListO<AnimTrackFrameEvents> frameEvents;
		public CListO<AnimTrackFrameSoundEvents> soundEvents;
		public CListO<Vec2d> vectors;
		public AABB LocalAABB;
		public AABB WorldAABB = new AABB() { MIN = Vec2d.Infinity, MAX = Vec2d.MinusInfinity };
		public pair<StringID, Path> skeleton;
		public pair<StringID, CString> skeletonOrigins;
		public KeyArray<int> texturePathKeysOrigins;
		public CListO<pair<StringID, Path>> texturePaths;
		public CListO<pair<StringID, CString>> texturePathsOrigins;
		public uint bankId0;
		public uint bankId;
		public uint unk2;
		public CListP<ulong> unk1Origins;

		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			version = s.Serialize<uint>(version, name: "version");
			length = s.Serialize<float>(length, name: "length");
			bml = s.SerializeObject<CListO<AnimTrackBML>>(bml, name: "BML");
			bonePAS = s.SerializeObject<CListO<AnimTrackBonePAS>>(bonePAS, name: "PAS");
			boneZAL = s.SerializeObject<CListO<AnimTrackBoneZAL>>(boneZAL, name: "ZAL");
			multiplierA = s.Serialize<float>(multiplierA, name: "multiplierA");
			multiplierP = s.Serialize<float>(multiplierP, name: "multiplierP");
			multiplierS = s.Serialize<float>(multiplierS, name: "multiplierS");
			polylines = s.SerializeObject<CListO<AnimTrackPolyline>>(polylines, name: "polylines");
			bonesLists = s.SerializeObject<CListO<AnimTrackBonesList>>(bonesLists, name: "bonesLists");
			frameEvents = s.SerializeObject<CListO<AnimTrackFrameEvents>>(frameEvents, name: "frameEvents");
			soundEvents = s.SerializeObject<CListO<AnimTrackFrameSoundEvents>>(soundEvents, name: "soundEvents");
			vectors = s.SerializeObject<CListO<Vec2d>>(vectors, name: "vectors");
			LocalAABB = s.SerializeObject<AABB>(LocalAABB, name: "LocalAABB");
			WorldAABB = s.SerializeObject<AABB>(WorldAABB, name: "WorldAABB");
			if (s.Settings.EngineVersion > EngineVersion.RO) {
				skeleton = s.SerializeObject<pair<StringID, Path>>(skeleton, name: "skeleton");
				texturePaths = s.SerializeObject<CListO<pair<StringID, Path>>>(texturePaths, name: "texturePaths");
				if (s.Settings.Game == Game.RL || s.Settings.Game == Game.COL) {
					bankId0 = s.Serialize<uint>(bankId0, name: "bankId0");
				}
				bankId = s.Serialize<uint>(bankId, name: "bankId");
				unk2 = s.Serialize<uint>(unk2, name: "unk2");
			} else {
				skeletonOrigins = s.SerializeObject<pair<StringID, CString>>(skeletonOrigins, name: "skeleton");
				texturePathKeysOrigins = s.SerializeObject<KeyArray<int>>(texturePathKeysOrigins, name: "texturePathKeys");
				texturePathsOrigins = s.SerializeObject<CListO<pair<StringID, CString>>>(texturePathsOrigins, name: "texturePaths");
				bankId0 = s.Serialize<uint>(bankId0, name: "bankId0");
				bankId = s.Serialize<uint>(bankId, name: "bankId");
				unk1Origins = s.SerializeObject<CListP<ulong>>(unk1Origins, name: "unk1");
				unk2 = s.Serialize<uint>(unk2, name: "unk2");
			}
		}

		public AnimSkeleton skel;
		public TextureCooked[] texs;
		protected override void OnPostSerialize(CSerializerObject s) {
			base.OnPostSerialize(s);
			Loader l = s.Context.Loader;
			if (s.Settings.EngineVersion > EngineVersion.RO) {
				if (skeleton != null && skeleton.Item2 != null && IsFirstLoad) {
					l.LoadFile<AnimSkeleton>(skeleton.Item2, result => skel = result);
				}
				if (texturePaths != null) {
					texs = new TextureCooked[texturePaths.Count];
					for (int i = 0; i < texturePaths.Count; i++) {
						LoadTexture(l, i, texturePaths[i].Item2);
					}
				}
			} else {
				if (skeletonOrigins != null && skeletonOrigins.Item2 != null && IsFirstLoad) {
					l.LoadFile<AnimSkeleton>(new Path(skeletonOrigins.Item2.str), result => skel = result);
				}
				if (texturePathsOrigins != null) {
					texs = new TextureCooked[texturePathsOrigins.Count];
					for (int i = 0; i < texturePathsOrigins.Count; i++) {
						LoadTexture(l, i, new Path(texturePathsOrigins[i].Item2.str));
					}
				}
			}
		}

		protected void LoadTexture(Loader l, int index, Path path) {
			l.LoadTexture(path, tex => {
				texs[index] = tex;
			});
		}
	}
}
