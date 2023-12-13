using rlImGui_cs;
using ImGuiNET;
using DungeonCrawler.Core;
using System.Numerics;
using Raylib_CsLo;
namespace DungeonCrawler.Editor;
public class Program
{
    public static void Main(string[] args)
    {
        InitWindow(Globals.WIDTH, Globals.HEIGHT, "Editor");
        rlImGui.Setup();
        TextureManager.LoadTextures();
        int x = 20;
        int y = 20;
        Camera2D camera = new Camera2D { zoom = 1f };

        while (!WindowShouldClose())
        {
            BeginDrawing();
            ClearBackground(BLACK);
            rlImGui.Begin();

            if (ImGui.Begin("Save/Load Map"))
            {
                String input = String.Empty;
                ImGui.InputText("Path (x/y/z/map.dcm)", ref input, 32);
                if (ImGui.Button("Save your map"))
                {
                    MapEditor.SaveMap(input);
                }

                if (ImGui.Button("Load map"))
                {
                    MapEditor.LoadMap(input);
                }


                ImGui.InputInt("Width", ref x);
                ImGui.InputInt("Height", ref y);
                if (ImGui.Button("Create Map"))
                {
                    MapEditor.GenerateEmptyWorldData(x, y);
                }
            }

            ImGui.End();
            if (IsMouseButtonDown(MouseButton.MOUSE_BUTTON_LEFT))
            {
                int relativeY = (int)(GetMousePosition().Y - camera.offset.Y) / Globals.SIZEOFTILE;
                int relativeX = (int)(GetMousePosition().X - camera.offset.X) / Globals.SIZEOFTILE;
                /*System.Console.WriteLine(GetMousePosition());
                System.Console.WriteLine(relativeY);
                System.Console.WriteLine(relativeX);*/
                System.Console.WriteLine($"{relativeX} + {relativeY}");

                if (MapEditor.DoesContainTile(relativeX, relativeY))
                {
                    TextureEditor.mousePosition = new Vector2 { X = relativeX, Y = relativeY };
                    TextureEditor.isTextureEditorOpen = true;
                    TextureEditor.currentTile = MapEditor.GetTile(relativeX, relativeY);
                }
            }

            if (IsMouseButtonDown(MouseButton.MOUSE_BUTTON_MIDDLE))
            {
                camera.offset += GetMouseDelta();
            }

            if (TextureEditor.isTextureEditorOpen)
            {
                if (ImGui.Begin("Texture Editor"))
                {
                    TextureEditor.DrawUI();
                }

                ImGui.End();
            }

            BeginMode2D(camera);
            MapEditor.DrawMap();
            EndMode2D();
            rlImGui.End();
            EndDrawing();
        }

        rlImGui.Shutdown();
        CloseWindow();
    }
}