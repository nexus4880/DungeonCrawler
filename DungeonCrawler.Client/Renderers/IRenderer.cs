using DungeonCrawler.Core.Entities.EntityComponents;

namespace DungeonCrawler.Client.Renderers;

public interface IRenderer : IEntityComponent
{
    void Draw();
}