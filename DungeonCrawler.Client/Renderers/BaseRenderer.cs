using DungeonCrawler.Core.Entities.EntityComponents;

namespace DungeonCrawler.Client.Renderers;

public abstract class BaseRenderer : BaseEntityComponent
{
    public abstract void Draw();
}