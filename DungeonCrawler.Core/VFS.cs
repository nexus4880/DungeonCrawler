using System.IO.Compression;

namespace DungeonCrawler.Core;

public class VFS : Dictionary<string, byte[]> {
	public static VFS FromArchive(ZipArchive archive) {
		VFS result = [];
		var buffer = new List<byte>(ushort.MaxValue);
		foreach (var entry in archive.Entries) {
			var entryStream = entry.Open();
			int readByte;
			do {
				readByte = entryStream.ReadByte();
				if (readByte == -1) {
					break;
				}

				buffer.Add((byte)readByte);
			} while (true);

			result[entry.FullName] = buffer[0..buffer.Count].ToArray();
			buffer.Clear();
		}

		return result;
	}

	public unsafe byte* PinnedBytes(string resource, out int length) {
		if (!this.TryGetValue(resource, out var bytes)) {
			length = 0;

			return null;
		}

		length = bytes.Length;
		fixed (byte* pBytes = bytes) {
			return pBytes;
		}
	}
}