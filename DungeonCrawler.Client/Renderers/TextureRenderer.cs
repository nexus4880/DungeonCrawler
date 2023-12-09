using System.Collections;
using DungeonCrawler.Client;
using DungeonCrawler.Client.Renderers;
using DungeonCrawler.Core;
using DungeonCrawler.Core.Entities;
using DungeonCrawler.Core.Extensions;
using LiteNetLib.Utils;
using Raylib_CsLo;

public class TextureRenderer : IRenderer
{
    public Entity Owner { get; init; }
    public Texture texture;

    public void Deserialize(NetDataReader reader)
    {
        throw new NotImplementedException();
    }

    public void Draw()
    {
        DrawTexture(texture,(Int32)this.Owner.Position.X,(Int32)this.Owner.Position.Y,WHITE);
    }

    public unsafe void Initialize(Queue properties)
    {
        String texturePath = properties.PopValue<String>(defaultValue: "assets/texture/checkmark.png");
        Byte* pBytes = Networking.currentVFS.PinnedBytes(texturePath, out int length);
        Image img = LoadImageFromMemory(".png",pBytes,length);
        texture = LoadTextureFromImage(img);
    }

    public void Serialize(NetDataWriter writer)
    {
        throw new NotImplementedException();
    }
}