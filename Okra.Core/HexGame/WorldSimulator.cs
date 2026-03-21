using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Okra.Core.HexGame;

public static partial class WorldSimulator
{
    // # Non-mutating functions
    public static IEnumerable<IControllable> GetControllables(WorldState state)
    {
        return state.GameObjectList.OfType<IControllable>();
    }

    public static OkraObject? FindGameObject(WorldState state, string name)
    {
        return state.GameObjectList.First(o => o.Name == name);
    }

    public class CanGoResponse
    {
        public string Reason = "";
        public List<MapNode> AvailableNodes = [];
    }

    public static CanGoResponse CanGo(WorldState state, IControllable who)
    {
        // todo: consider terrains
        // todo: consider movement attribute
        return new CanGoResponse
        {
            AvailableNodes = who.Position.OutgoingEdgeList
        };
    }

    // # Mutating functions
    // mutate world state and spit out previous world state in serialized form
    // also tell if mutation failed, no mutation, or there is a mutation

    public class MoveResponse
    {
        public string Reason = "";

        public MutationState MutationState = MutationState.Mutated;
        // todo: serialized version of the old world state

        public List<Task<MutationState>> PawnTaskList = new();
    }

    public static MoveResponse Move(WorldState state, IControllable who, MapNode where)
    {
        if (!CanGo(state, who).AvailableNodes.Exists(n => n == where))
        {
            return new MoveResponse
            {
                Reason = "where node not in CanGo nodes for who",
                MutationState = MutationState.Failed,
            };
        }

        var resp = new MoveResponse();
        var t = who.SetPosition(where);
        if (t != null)
        {
            resp.PawnTaskList.Add(t);
        }
        return resp;
    }
}