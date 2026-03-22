using System.Collections.Generic;
using Godot;

namespace Okra.Core.HexGame;

// assume as pointy top hex
public class MapNode
{
    // todo: calculated based on Parent Origin + Grid Position
    private Vector3 _gamePosition;

    public Vector3I HexPosition = Vector3I.Zero;

    // todo: cache?
    public List<MapNode> OutgoingEdgeList
    {
        get
        {
            List<MapNode> neighborNodes = [];
            MapData.HexOffset.ForEach(offset =>
            {
                if (Parent.Graph.TryGetValue(HexPosition + offset, out var possibleNeighbor))
                {
                    neighborNodes.Add(possibleNeighbor);
                }
            });
            return neighborNodes;
        }
    }

    public string Name => $"Node {HexPosition.ToString()}";
    public required MapData Parent { get; init; }

    public Vector3 GamePosition
    {
        get
        {
            if (Parent.CalculatedNode.Contains(HexPosition))
            {
                return _gamePosition;
            }

            var offset = MapData.PointyHexToPixel(HexPosition);
            _gamePosition = Parent.GameOrigin + new Vector3(offset.X, offset.Y, 0) * Parent.VectorMultiplier;
            Parent.CalculatedNode.Add(HexPosition);
            return _gamePosition;
        }
    }
}