using DungeonCrawler.Core.Attributes;
using DungeonCrawler.Core.Entities.EntityComponents.Renderers;

namespace DungeonCrawler.Server.Entities.EntityComponents.Renderers;

[HashAs("DungeonCrawler.Core.Entities.EntityComponents.Renderers.CircleRenderer")]
public class ServerCircleRenderer : CircleRenderer
{
    public override void Draw()
    {
        throw new NotImplementedException();
    }
}