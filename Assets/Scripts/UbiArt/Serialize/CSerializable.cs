using PlasticGui.WorkspaceWindow.Items;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace UbiArt {
	public class CSerializable : ICSerializable {
		[IgnoreDataMember]
		protected bool IsFirstLoad { get; set; } = true;
		[IgnoreDataMember]
		public Context UbiArtContext { get; protected set; }

		public void InitContext(Context c) {
			UbiArtContext = c;
		}

		public void Serialize(CSerializerObject s, string name) {
			InitContext(s.Context);
			OnPreSerialize(s);
			SerializeImpl(s);
			OnPostSerialize(s);
			IsFirstLoad = false;
		}

		protected virtual void OnPreSerialize(CSerializerObject s) {}
		protected virtual void OnPostSerialize(CSerializerObject s) {}
		protected virtual void SerializeImpl(CSerializerObject s) {
			s.SerializeObjectSize(GetType());
		}

		// Dangerous to use without calling LoadLoop, especially if some references haven't been loaded
		public virtual CSerializable Clone(string extension, Context context = null, bool allowLoadingReferences = false) {
			byte[] serializedData = null;
			context ??= UbiArtContext;
			if (!allowLoadingReferences) context.Loader.ResolveReferences = false;
			CSerializable result = null;
			using (MemoryStream stream = new MemoryStream()) {
				using (Writer writer = new Writer(stream, context.Settings.IsLittleEndian)) {
					CSerializerObject w = context.Loader.CreateBinarySerializer(extension, writer);
					object toWrite = w.Serialize(this, this.GetType(), name: "clone");
					serializedData = stream.ToArray();
				}
			}
			using (MemoryStream stream = new MemoryStream(serializedData)) {
				using (Reader reader = new Reader(stream, context.Settings.IsLittleEndian)) {
					CSerializerObject r = context.Loader.CreateBinarySerializer(extension, reader);
					object toRead = r.Serialize(null, this.GetType(), name: "clone");
					result = toRead as CSerializable;
				}
			}
			if (!allowLoadingReferences) context.Loader.ResolveReferences = true;
			return result;
		}
		public async Task<CSerializable> CloneAsync(string extension, Context context = null) {
			var result = Clone(extension, context: context, allowLoadingReferences: true);
			context ??= UbiArtContext;
			await context?.Loader?.LoadLoop();
			return result;
		}
		public virtual byte[] CloneGetBinaryData(string extension, Context context = null, bool allowLoadingReferences = false) {
			byte[] serializedData = null;
			context ??= UbiArtContext;
			if (!allowLoadingReferences) context.Loader.ResolveReferences = false;
			using (MemoryStream stream = new MemoryStream()) {
				using (Writer writer = new Writer(stream, context.Settings.IsLittleEndian)) {
					CSerializerObject w = context.Loader.CreateBinarySerializer(extension, writer);
					object toWrite = w.Serialize(this, this.GetType(), name: "clone");
					serializedData = stream.ToArray();
				}
			}
			if (!allowLoadingReferences) context.Loader.ResolveReferences = true;
			return serializedData;
		}

		public static CSerializable CreateFromBinaryData(byte[] serializedData, Type type, string extension, Context context, bool allowLoadingReferences = false) {
			CSerializable result = null;
			if (!allowLoadingReferences) context.Loader.ResolveReferences = false;
			using (MemoryStream stream = new MemoryStream(serializedData)) {
				using (Reader reader = new Reader(stream, context.Settings.IsLittleEndian)) {
					CSerializerObject r = context.Loader.CreateBinarySerializer(extension, reader);
					object toRead = r.Serialize(null, type, name: "clone");
					result = toRead as CSerializable;
				}
			}
			if (!allowLoadingReferences) context.Loader.ResolveReferences = true;
			return result;
		}

		[IgnoreDataMember]
		public virtual string ClassName {
			get {
				Type t = GetType();
				if (t.IsGenericType) {
					string typeName = t.Name;
					if (typeName.Contains('`')) typeName = typeName.Substring(0, typeName.IndexOf("`"));
					return typeName;
				} else {
					return t.Name;
				}
			}
		}

		[IgnoreDataMember]
		public virtual uint? ClassCRC => null;
	}
}