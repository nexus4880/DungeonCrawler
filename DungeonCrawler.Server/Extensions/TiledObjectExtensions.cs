using System.Drawing;
using TiledCS;

namespace DungeonCrawler.Server.Extensions;

public static class TiledObjectExtensions {
	public static Dictionary<string, object> Parse(this TiledProperty[] properties) {
		var result = new Dictionary<string, object>(properties.Length);
		foreach (var property in properties) {
			switch (property.type) {
				case TiledPropertyType.String: {
					result[property.name] = property.value;

					break;
				}
				case TiledPropertyType.Bool: {
					result[property.name] = bool.Parse(property.value);

					break;
				}
				case TiledPropertyType.Color: {
					result[property.name] = Color.FromArgb(int.Parse(property.value));

					break;
				}
				case TiledPropertyType.Float: {
					result[property.name] = float.Parse(property.value);

					break;
				}
				case TiledPropertyType.Int: {
					result[property.name] = int.Parse(property.value);

					break;
				}
				case TiledPropertyType.Object: {
					var id = int.Parse(property.value);
					result[property.name] = id == 0 ? null :
						GameServer.MapReferences.GetValueOrDefault(id);

					break;
				}
				case TiledPropertyType.File: {
					throw new NotSupportedException();
				}
			}
		}

		return result;
	}
}