using System.Threading.Tasks;

namespace Okra.Core.HexGame;

public class ControllableObject : OkraObject, IControllable
{
    private IControllablePawn? _controllablePawn;
    private WorldState _worldState;

    public IControllablePawn? ControllablePawn
    {
        get => _controllablePawn;
        set
        {
            _controllablePawn = value;
            _controllablePawn?.SetWorldState(_worldState);
            _controllablePawn?.SetCoreObject(this);
        }
    }

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

    public void SetWorldState(WorldState worldState)
    {
        _worldState = worldState;
        ControllablePawn?.SetWorldState(worldState);
    }
}