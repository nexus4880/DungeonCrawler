using DungeonCrawler.Core;

namespace DungeonCrawler.Client;

public class AssetHandler<T>
{
    private VFS _vfs;
    private Dictionary<string, T> _assets;
    private Func<Byte[], T> _initializer;

    public AssetHandler(VFS vfs, Func<Byte[], T> initializer)
    {
        this._vfs = vfs;
        this._assets = [];
        this._initializer = initializer;
    }

    public T GetAsset(String assetPath)
    {
        if (!_assets.TryGetValue(assetPath, out T value))
        {
            if (!this._vfs.TryGetValue(assetPath, out Byte[] bytes))
            {
                throw new Exception($"Failed to find '{assetPath}'");
            }

            this._assets[assetPath] = value = this._initializer(bytes);
        }

        return value;
    }
}