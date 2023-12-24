using DungeonCrawler.Core;

namespace DungeonCrawler.Client;

public class AssetHandler<T> {
	private VFS _vfs;
	private Dictionary<string, T> _assets;
	private Func<byte[], T> _initializer;

	public AssetHandler(VFS vfs, Func<byte[], T> initializer) {
		this._vfs = vfs;
		this._assets = [];
		this._initializer = initializer;
	}

	public T GetAsset(string assetPath) {
		if (!this._assets.TryGetValue(assetPath, out var value)) {
			if (!this._vfs.TryGetValue(assetPath, out var bytes)) {
				throw new Exception($"Failed to find '{assetPath}'");
			}

			this._assets[assetPath] = value = this._initializer(bytes);
		}

		return value;
	}
}