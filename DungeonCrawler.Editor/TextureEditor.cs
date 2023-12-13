using System.Data.SqlTypes;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using DungeonCrawler.Core;
using DungeonCrawler.Core.Packets;
using ImGuiNET;
using Raylib_CsLo;
using rlImGui_cs;
namespace DungeonCrawler.Editor;
public class TextureEditor
{
    public static bool isTextureEditorOpen = false;
    public static WorldData.TileData? currentTile = new WorldData.TileData();
    public static Vector2 mousePosition = new Vector2(-1);
    private static bool _canMoveNorth = true;
    private static bool _canMoveEast = true;
    private static bool _canMoveSouth = true;
    private static bool _canMoveWest = true;

    public static void DrawUI()
    {
        if (ImGui.Button("Close"))
        {
            isTextureEditorOpen = false;
            currentTile = null;
            return;
        }

        if (MapEditor.DoesContainTile((int)mousePosition.X, (int)mousePosition.Y) || currentTile != null)
        {
            WorldData.TileData tileData = currentTile.Value;
            ImGui.Text($"Selected tile {mousePosition.X}, {mousePosition.Y}");
            ImGui.Text($"Texture ID of Tile: {currentTile?.textureId}");
            unsafe
            {
                if (ImGui.Checkbox("Can enter from North", ref _canMoveNorth))
                {
                    tileData.canMoveOnto[0] = _canMoveNorth;
                }

                if (ImGui.Checkbox("Can enter from East", ref _canMoveEast))
                {
                    tileData.canMoveOnto[1] = _canMoveEast;
                }

                if (ImGui.Checkbox("Can enter from South", ref _canMoveSouth))
                {
                    tileData.canMoveOnto[2] = _canMoveSouth;
                }
                
                if (ImGui.Checkbox("Can enter from West", ref _canMoveWest))
                {
                    tileData.canMoveOnto[3] = _canMoveWest;
                }
            }

            foreach (Texture asset in TextureManager.textures)
            {
                if (rlImGui.ImageButton("name", asset))
                {
                    tileData.textureId = asset.id;
                    MapEditor.SetTile((int)mousePosition.X, (int)mousePosition.Y, tileData);
                    break;
                }
            }
        }
        else
        {
            ImGui.Text("Select a tile");
        }
    }
}