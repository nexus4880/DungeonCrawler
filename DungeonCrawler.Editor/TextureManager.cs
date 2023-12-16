using Raylib_CsLo;
namespace DungeonCrawler.Editor;
public class TextureManager
{
    public static List<Texture> textures = new List<Texture>();
    public static Texture emptyTexture;
    public static void LoadTextures()
    {
        if (!Directory.Exists("assets"))
        {
            System.Console.WriteLine("Creating assets folder, place your assets there");
            Directory.CreateDirectory("assets");
        }

        string[] files = Directory.GetFiles("assets");
        for (int i = 0; i < files.Length; i++)
        {
            if (files[i] == "assets/checkmark.png")
            {
                emptyTexture = LoadTexture(files[i]);
            }

            textures.Add(LoadTexture(files[i]));
        }
    }
}