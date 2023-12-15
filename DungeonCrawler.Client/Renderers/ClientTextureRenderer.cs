using System.Collections;
using Raylib_CsLo;
using DungeonCrawler.Core.Entities.EntityComponents.Renderers;
using DungeonCrawler.Core.Attributes;
using DungeonCrawler.Core.Entities;

namespace DungeonCrawler.Client.Renderers;

[HashAs("DungeonCrawler.Core.Entities.EntityComponents.Renderers.TextureRenderer")]
public class ClientTextureRenderer : TextureRenderer, IClientInitializable
{
    public Texture texture;

    public unsafe void ClientInitialize()
    {
        Byte* pBytes = Networking.currentVFS.PinnedBytes(this.TexturePath, out Int32 length);
        Image img = LoadImageFromMemory(".png", pBytes, length);
        this.texture = LoadTextureFromImage(img);
    }

    public override void Draw()
    {
        DrawTexture(this.texture, (Int32)this.Owner.Position.X, (Int32)this.Owner.Position.Y, WHITE);
    }

    public override void OnComponentRemoved()
    {
        base.OnComponentRemoved();
        UnloadTexture(this.texture);
    }
}