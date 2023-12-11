using System.Collections;
using DungeonCrawler.Core.Extensions;
using LiteNetLib.Utils;
using Raylib_CsLo;

namespace DungeonCrawler.Client.Renderers;

public class TextureRenderer : BaseRenderer
{
    public Texture texture;

    public override void Deserialize(NetDataReader reader)
    {
        base.Deserialize(reader);
    }

    public override void Draw()
    {
        DrawTexture(texture, (Int32)this.Owner.Position.X, (Int32)this.Owner.Position.Y, WHITE);
    }

    public unsafe override void Initialize(Queue properties)
    {
        String texturePath = properties.PopValueOrThrow<String>();
        Console.WriteLine(texturePath);
        Byte* pBytes = Networking.currentVFS.PinnedBytes(texturePath, out int length);
        Image img = LoadImageFromMemory(".png", pBytes, length);
        texture = LoadTextureFromImage(img);
    }

    public override void OnComponentRemoved()
    {
        base.OnComponentRemoved();
        UnloadTexture(this.texture);
    }

    public override void Serialize(NetDataWriter writer)
    {
        base.Serialize(writer);
    }
}