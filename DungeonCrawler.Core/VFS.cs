using System.IO.Compression;

namespace DungeonCrawler.Core;

public class VFS : Dictionary<String, Byte[]>
{
    public static VFS FromArchive(ZipArchive archive)
    {
        VFS result = new VFS();
        List<Byte> buffer = new List<Byte>(UInt16.MaxValue);
        foreach (ZipArchiveEntry entry in archive.Entries)
        {
            Stream entryStream = entry.Open();
            while (entryStream.CanRead)
            {
                buffer.Add((Byte)entryStream.ReadByte());
            }

            result[entry.FullName] = buffer[0..buffer.Count].ToArray();
            buffer.Clear();
        }

        return result;
    }

    public unsafe Byte* PinnedBytes(String resource, out Int32 length)
    {
        if (!this.TryGetValue(resource, out Byte[] bytes))
        {
            length = 0;

            return null;
        }

        length = bytes.Length;
        fixed (Byte* pBytes = bytes)
        {
            return pBytes;
        }
    }
}