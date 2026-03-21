using System.Collections.Generic;
using Godot;

namespace Okra.Core.HexGame;

// assume as pointy top hex
public class MapNode
{
    // todo: maybe not needed anymore since we can infer outgoing using Parent
    public readonly List<MapNode> OutgoingEdgeList = [];

    private Vector3 _gamePosition;


    // todo: calculated based on Parent Origin + Grid Position
    public Vector3I HexPosition = Vector3I.Zero;
    public string Name => $"Node {HexPosition.ToString()}";
    public MapData Parent { get; set; }

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