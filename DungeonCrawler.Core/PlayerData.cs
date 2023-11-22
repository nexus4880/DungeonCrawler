using System.Numerics;
using DungeonCrawler.Core.Extensions;
using LiteNetLib.Utils;

namespace DungeonCrawler.Core;

public struct PlayerData : INetSerializable {
	public Vector2 position;

	public void Serialize(NetDataWriter writer) {
		writer.Put(this.position);
	}

	public void Deserialize(NetDataReader reader) {
		this.position = reader.GetVector2();
	}
}