#nullable enable
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;

namespace BinarySerializer
{
    public interface IFileManager
    {
        bool DirectoryExists([NotNullWhen(true)] string? path);
        bool FileExists([NotNullWhen(true)] string? path);

        Stream GetFileReadStream(string path);
        Stream GetFileWriteStream(string path, bool recreateOnWrite = true);

        PathSeparatorChar SeparatorCharacter { get; }

        Task FillCacheForReadAsync(long length, Reader reader);
    }
}