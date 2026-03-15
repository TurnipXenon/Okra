using System.Collections.Generic;
using Godot;

namespace Okra.Core.HexGame;

// assume as pointy top hex
public class MapNode
{
    public string Name = "";
    public Vector3 Position = Vector3.Zero;
    public readonly List<MapNode> OutgoingEdgeList = [];
}