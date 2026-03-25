using System.Threading.Tasks;
using Godot;

namespace Okra.Core.HexGame;

public interface IControllablePawn : ISelectable
{
    Task<MutationState> SetPosition(MapNode positionNode);
    void ForcePosition(Vector3 position);
    void SetWorldState(WorldState worldState);
    void SetCoreObject(ControllableObject controllableObject);
}