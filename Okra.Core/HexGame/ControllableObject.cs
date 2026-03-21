using System.Threading.Tasks;

namespace Okra.Core.HexGame;

public class ControllableObject : OkraObject, IControllable
{
    public IControllablePawn? ControllablePawn { get; set; }

    private MapNode _position = new();

    public MapNode Position => _position;

    public Task<MutationState>? SetPosition(MapNode mapNode)
    {
        _position = mapNode;
        return ControllablePawn?.SetPosition(mapNode.GamePosition);
    }

    public void ForcePosition(MapNode mapNode)
    {
        _position = mapNode;
        ControllablePawn?.ForcePosition(mapNode.GamePosition);
    }
}