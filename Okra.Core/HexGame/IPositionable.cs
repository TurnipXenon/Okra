using System.Threading.Tasks;

namespace Okra.Core.HexGame;

public interface IPositionable
{
    // todo: make generic later?
    public IControllablePawn? ControllablePawn { get; set; }

    MapNode Position { get; }

    /**
     * IControllable might be manipulating a game-space pawn
     * Task will return something once an animation or decision is done
     */
    Task<MutationState>? SetPosition(MapNode mapNode);
}