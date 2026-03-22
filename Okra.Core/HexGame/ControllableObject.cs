using System.Threading.Tasks;

namespace Okra.Core.HexGame;

public class ControllableObject : OkraObject, IControllable
{
    public IControllablePawn? ControllablePawn { get; set; }

    public MapNode? Position { get; private set; }

    public Task<MutationState>? SetPosition(MapNode mapNode)
    {
        Position = mapNode;
        return ControllablePawn?.SetPosition(mapNode.GamePosition);
    }

    public void ForcePosition(MapNode mapNode)
    {
        Position = mapNode;
        ControllablePawn?.ForcePosition(mapNode.GamePosition);
    }
}