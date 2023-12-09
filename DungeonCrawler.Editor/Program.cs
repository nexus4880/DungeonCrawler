using static Raylib_CsLo.Raylib;
using rlImGui_cs;
using ImGuiNET;
public class Program
{
    public static void Main(string[] args)
    {
        InitWindow(1280,720,"Editor");
        rlImGui.Setup();
        while(!WindowShouldClose()){
            BeginDrawing();
            ClearBackground(BLACK);
            rlImGui.Begin();
            ImGui.ShowDemoWindow();
            rlImGui.End();
            EndDrawing();
        }

        rlImGui.Shutdown();
        CloseWindow();
    }
}