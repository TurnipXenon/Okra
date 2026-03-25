using System.Threading.Tasks;

namespace Okra.Core.HexGame;

public class ControllableObject : OkraObject, IControllable
{
    public IControllablePawn? ControllablePawn { get; set; }

    public MapNode? Position { get; private set; }

    public Task<MutationState>? SetPosition(MapNode mapNode)
    {
        Position = mapNode;
        return ControllablePawn?.SetPosition(mapNode);
    }

    public void ForcePosition(MapNode mapNode)
    {
        // todo: might need to delete since it's not consistent?
        Position = mapNode;
        ControllablePawn?.ForcePosition(mapNode.GamePosition);
    }
}