using System.Drawing;
using TiledCS;

namespace DungeonCrawler.Server.Extensions;

public static class TiledObjectExtensions
{
    public static Dictionary<String, Object> Parse(this TiledProperty[] properties)
    {
        Dictionary<String, Object> result = new Dictionary<string, Object>(properties.Length);
        foreach (var property in properties)
        {
            switch (property.type)
            {
                case TiledPropertyType.String:
                    {
                        result[property.name] = property.value;

                        break;
                    }
                case TiledPropertyType.Bool:
                    {
                        result[property.name] = Boolean.Parse(property.value);

                        break;
                    }
                case TiledPropertyType.Color:
                    {
                        result[property.name] = Color.FromArgb(Int32.Parse(property.value));

                        break;
                    }
                case TiledPropertyType.Float:
                    {
                        result[property.name] = Single.Parse(property.value);

                        break;
                    }
                case TiledPropertyType.Int:
                    {
                        result[property.name] = Int32.Parse(property.value);

                        break;
                    }
                case TiledPropertyType.File:
                    {
                        throw new NotSupportedException();
                    }
                case TiledPropertyType.Object:
                    {
                        throw new NotSupportedException();
                    }
            }
        }

        return result;
    }
}