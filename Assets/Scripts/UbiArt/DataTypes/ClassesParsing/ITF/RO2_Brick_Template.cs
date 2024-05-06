
using System.IO;

namespace UbiArt.ITF {
	public partial class RO2_Brick_Template {
		public ContainerFile<Scene> sceneFile;

		protected override void OnPostSerialize(CSerializerObject s) {
			base.OnPostSerialize(s);
			if (IsFirstLoad) {
				Loader l = s.Context.Loader;
				l.Load(archive, path.filename, (extS) => {
					sceneFile = extS.SerializeObject<ContainerFile<Scene>>(sceneFile);
				});
			}
		}

		protected override void OnPreSerialize(CSerializerObject s) {
			base.OnPreSerialize(s);
			if (s is CSerializerObjectBinary sb && sb.Mode == CSerializerObjectBinary.BinaryMode.Write && s.HasFlags(SerializeFlags.Flags10)) {
				if (sceneFile == null && path != null) {
					sceneFile = path.GetObject<ContainerFile<Scene>>();
					if (sceneFile?.obj != null) {
						sceneFile = new ContainerFile<Scene>((Scene)sceneFile.obj.Clone("isc", context: s.Context));
					}
				}


				if (sceneFile != null) {
					byte[] serializedData = null;
					using (MemoryStream stream = new MemoryStream()) {
						using (Writer writer = new Writer(stream, s.Settings.IsLittleEndian)) {
							CSerializerObject w = s.Context.Loader.CreateBinarySerializer(path.GetExtension(), writer);
							object toWrite = w.Serialize(sceneFile, sceneFile.GetType(), name: path.filename);
							serializedData = stream.ToArray();
						}
					}
					if (serializedData != null) {
						if(archive == null) archive = new ArchiveMemory();
						archive.AMData = serializedData;
					}
				}
			}
		}
	}
}
