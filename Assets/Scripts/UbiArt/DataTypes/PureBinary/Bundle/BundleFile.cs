using System.Collections.Generic;
using System.Linq;
using System.IO;
using Ionic.Zlib;
using System.Threading.Tasks;
using UbiCanvas.Helpers;
using System;

namespace UbiArt.Bundle {
	public class BundleFile : ICSerializable {
		public BundleBootHeader bootHeader;
		public FilePackMaster packMaster;
		private Dictionary<Path, ICSerializable> files = new Dictionary<Path, ICSerializable>();
		private Dictionary<Path, byte[]> preProcessedFiles = new Dictionary<Path, byte[]>();
		private Dictionary<Path, DateTime> times = new Dictionary<Path, DateTime>();

		public bool IsEmpty => !readFileData.Any() && !files.Any() && !preProcessedFiles.Any();

		private Dictionary<Path, byte[]> readFileData = new Dictionary<Path, byte[]>();

		public void Serialize(CSerializerObject s, string name) {
			bootHeader = s.SerializeObject<BundleBootHeader>(bootHeader, name: nameof(bootHeader));
			packMaster = s.SerializeObject<FilePackMaster>(packMaster, name: nameof(packMaster));
		}

		public async Task SerializeAsync(CSerializerObject s, string name) {
			await s.FillCacheForRead(11 * 4);
			bootHeader = s.SerializeObject<BundleBootHeader>(bootHeader, name: nameof(bootHeader));
			await s.FillCacheForRead(bootHeader.BaseOffset);
			packMaster = s.SerializeObject<FilePackMaster>(packMaster, name: nameof(packMaster));
		}

		public BundleFile() {
			bootHeader = new BundleBootHeader();
			packMaster = new FilePackMaster();
		}
		public BundleFile(Context context) {
			bootHeader = new BundleBootHeader(context);
			packMaster = new FilePackMaster();
		}
		public Stream GetFileStream(Path path) {
			if (readFileData.ContainsKey(path)) return new MemoryStream(readFileData[path]);
			return null;
		}

		public async Task<byte[]> GetFile(Context context, CSerializerObject s, Path path) {
			if (readFileData.ContainsKey(path)) return readFileData[path];
			if (packMaster != null) {
				FileHeaderRuntime h = packMaster.files.FirstOrDefault(f => f.Item2 == path)?.Item1;
				if (h != null) {
					ulong off = h.offsets[0];
					uint size = h.zSize != 0 ? h.zSize : h.size;
					s.Goto((long)off + bootHeader.BaseOffset);
					await s.FillCacheForRead(size);
					byte[] fileBytes = s.SerializeBytes(default, size);
					if (fileBytes != null) {
						if (h.zSize != 0) {
							byte[] decompressedData = new byte[(int)h.size];
							using (var zlibStream = new ZlibStream(new MemoryStream(fileBytes), CompressionMode.Decompress))
								zlibStream.Read(decompressedData, 0, decompressedData.Length);
							fileBytes = decompressedData;
						}
						readFileData[path] = fileBytes;
					}
					return fileBytes;
				}
			}
			return null;
		}

		public bool HasReadFile(Path path) {
			return readFileData.ContainsKey(path);
		}
		public bool HasPreprocessedFile(Path path) => preProcessedFiles.ContainsKey(path);
		public bool ContainsFile(Path path) => packMaster.files.Any(f => f.Item2 == path);
		public Path GetPathByStringID(StringID id) => packMaster.files.FirstOrDefault(f => f.Item2.stringID == id)?.Item2 ?? null;

		public void AddFile(Path path, ICSerializable data, DateTime? lastWriteTime = null) {
			files[path] = data;
			SetTime(path, lastWriteTime);
		}
		public void AddFile(Path path, byte[] data, DateTime? lastWriteTime = null) {
			preProcessedFiles[path] = data;
			SetTime(path, lastWriteTime);
		}
		public bool HasAddedFile(Path path) => files.ContainsKey(path);
		public ICSerializable GetAddedFile(Path path) => files[path];

		public void SetTime(Path path, DateTime? lastWriteTime = null) {
			if (lastWriteTime.HasValue)
				times[path] = lastWriteTime.Value;
			else
				times[path] = DateTime.Now;
		}

		public async Task WriteBundle(Context context, string path) {
			bootHeader.FilesCount = (uint)files.Keys.Count + (uint)preProcessedFiles.Count;
			List<byte[]> data = new List<byte[]>();
			ulong curOffset = 0;
			byte[] serializedData = null;
			foreach (KeyValuePair<Path, ICSerializable> kv in files) {
				using (MemoryStream stream = new MemoryStream()) {
					using (Writer writer = new Writer(stream, context.Settings.IsLittleEndian)) {
						CSerializerObject w = context.Loader.CreateSerializerForPath(kv.Key.UncookedPath(context), writer);
						object toWrite = w.Serialize(kv.Value, kv.Value.GetType(), name: kv.Key.filename);
						serializedData = stream.ToArray();
					}
				}
				FileHeaderRuntime fhr = new FileHeaderRuntime {
					numOffsets = 1,
					offsets = new ulong[1] { curOffset },
					size = (uint)serializedData.Length,
					zSize = 0,
					timeStamp = 0
				};
				if(times.ContainsKey(kv.Key)) fhr.timeStamp = times[kv.Key].ToFileTime();
				packMaster.files.Add(new pair<FileHeaderRuntime, Path>(fhr, kv.Key));
				data.Add(serializedData);
				curOffset += (uint)serializedData.Length;
				await TimeController.WaitIfNecessary();
			}
			foreach (var kv in preProcessedFiles) {
				serializedData = kv.Value;
				FileHeaderRuntime fhr = new FileHeaderRuntime {
					numOffsets = 1,
					offsets = new ulong[1] { curOffset },
					size = (uint)serializedData.Length,
					zSize = 0,
					timeStamp = 0
				};
				if (times.ContainsKey(kv.Key)) fhr.timeStamp = times[kv.Key].ToFileTime();
				packMaster.files.Add(new pair<FileHeaderRuntime, Path>(fhr, kv.Key));
				data.Add(serializedData);
				curOffset += (uint)serializedData.Length;
				await TimeController.WaitIfNecessary();
			}
			using (MemoryStream stream = new MemoryStream()) {
				using (Writer writer = new Writer(stream, context.Settings.IsLittleEndian)) {
					CSerializerObjectBinary w = context.Loader.CreateBinarySerializer("ipk", writer);
					object toWrite = w.Serialize(this, this.GetType(), name: "Bundle");
					serializedData = stream.ToArray();
				}
			}
			await TimeController.WaitIfNecessary();
			bootHeader.BaseOffset = (uint)serializedData.Length;
			using (MemoryStream stream = new MemoryStream()) {
				using (Writer writer = new Writer(stream, context.Settings.IsLittleEndian)) {
					CSerializerObjectBinary w = context.Loader.CreateBinarySerializer("ipk", writer);
					object toWrite = w.Serialize(this, this.GetType(), name: "Bundle");
					serializedData = stream.ToArray();
				}
			}
			await TimeController.WaitIfNecessary();
			Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
			using (BinaryWriter bw = new BinaryWriter(new FileStream(path, FileMode.Create))) {
				bw.Write(serializedData);
				for (int i = 0; i < data.Count; i++) {
					bw.Write(data[i]);
				}
			}
		}


		public async Task WriteFilesRaw(Context context, string path) {
			bootHeader.FilesCount = (uint)files.Keys.Count + (uint)preProcessedFiles.Count;
			byte[] serializedData = null;
			foreach (KeyValuePair<Path, ICSerializable> kv in files) {
				using (MemoryStream stream = new MemoryStream()) {
					using (Writer writer = new Writer(stream, context.Settings.IsLittleEndian)) {
						CSerializerObject w = context.Loader.CreateSerializerForPath(kv.Key.UncookedPath(context), writer);
						object toWrite = w.Serialize(kv.Value, kv.Value.GetType(), name: kv.Key.filename);
						serializedData = stream.ToArray();
					}
				}
				var outPath = $"{path}/{kv.Key.FullPath}";
				Util.ByteArrayToFile(outPath, serializedData);
				await TimeController.WaitIfNecessary();
			}
			foreach (var kv in preProcessedFiles) {
				serializedData = kv.Value;
				var outPath = $"{path}/{kv.Key.FullPath}";
				Util.ByteArrayToFile(outPath, serializedData);
			}
		}
	}
}
