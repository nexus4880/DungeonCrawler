namespace DungeonCrawler.Core;

public struct WorldData {
	public struct TileData {
		public ulong textureId;
		public unsafe fixed bool canMoveOnto[4];

		public static unsafe TileData FromReader(BinaryReader reader) {
			var tile = new TileData {
				textureId = reader.ReadUInt64()
			};
			for (var i = 0; i < 4; i++) {
				tile.canMoveOnto[i] = reader.ReadBoolean();
			}

			return tile;
		}

		public unsafe void Serialize(BinaryWriter writer) {
			writer.Write(this.textureId);
			for (var i = 0; i < 4; i++) {
				writer.Write(this.canMoveOnto[i]);
			}
		}
	}

	public int width;
	public int height;
	public TileData[,] tiles;

	public static WorldData FromReader(BinaryReader reader) {
		var world = new WorldData {
			width = reader.ReadInt32(),
			height = reader.ReadInt32()
		};
		world.tiles = new TileData[world.width, world.height];
		for (var x = 0; x < world.width; x++) {
			for (var y = 0; y < world.height; y++) {
				world.tiles[x, y] = TileData.FromReader(reader);
			}
		}

		return world;
	}

	public void Serialize(BinaryWriter writer) {
		writer.Write(this.width);
		writer.Write(this.height);
		for (var x = 0; x < this.width; x++) {
			for (var y = 0; y < this.height; y++) {
				this.tiles[x, y].Serialize(writer);
			}
		}
	}
}