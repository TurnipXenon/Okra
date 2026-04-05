using Godot;
using Okra.Core.HexGame;

namespace Okra.Game.Scenes.Okra.Prefabs.HexPawn;

public partial class HexPawn : Node2D, IMapNodePawn
{
    public MapNode? MapNode { get; private set; }

    public void SetMapNode(MapNode mapNode, OkraPrototype okraPrototype)
    {
        MapNode = mapNode;
        mapNode.Pawn = this;
        var v3 = mapNode.GamePosition;
        Position = new Vector2(v3.X, v3.Y);
        okraPrototype.AddChild(this);
    }
}