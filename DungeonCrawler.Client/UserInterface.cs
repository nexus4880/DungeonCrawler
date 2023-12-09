using DungeonCrawler.Client;
using DungeonCrawler.Core.Entities.EntityComponents;

public class UserInterface{
    public static void Draw(){
        DrawText(GameManager.localPlayer.GetComponent<HealthComponent>().Value.ToString(),0,0,24,RED);
    }
}