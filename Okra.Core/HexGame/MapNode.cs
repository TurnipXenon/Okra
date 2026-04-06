using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Godot;

namespace Okra.Core.HexGame;

// assume as pointy top hex
// custom serialization: https://stackoverflow.com/a/35088054/10024566
public class MapNode
{
    [JsonInclude] private readonly int[] _hexPosition = new int[3];

    // todo: calculated based on Parent Origin + Grid Position
    [JsonIgnore] private Vector3 _gamePosition;

    public Vector3I HexPosition = Vector3I.Zero;

    // todo: cache?
    [JsonIgnore]
    public List<MapNode> OutgoingEdgeList
    {
        get
        {
            List<MapNode> neighborNodes = [];
            MapData.HexOffset.ForEach(offset =>
            {
                if (Parent == null)
                {
                    return;
                }

                if (Parent.Graph.TryGetValue(HexPosition + offset, out var possibleNeighbor))
                {
                    neighborNodes.Add(possibleNeighbor);
                }
            });
            return neighborNodes;
        }
    }

    [JsonIgnore] public string Name => $"Node {HexPosition.ToString()}";

    [JsonIgnore] public MapData? Parent { get; init; }

    [JsonIgnore]
    public Vector3 GamePosition
    {
        get
        {
            if ((bool)Parent?.CalculatedNode.Contains(HexPosition))
            {
                return _gamePosition;
            }

            var offset = MapData.PointyHexToPixel(HexPosition);
            _gamePosition = Parent.GameOrigin + new Vector3(offset.X, offset.Y, 0) * Parent.VectorMultiplier;
            Parent.CalculatedNode.Add(HexPosition);
            return _gamePosition;
        }
    }

    [JsonIgnore] public IMapNodePawn? Pawn { get; set; }

    public List<MapNode> FindShortestPathTo(MapNode destination)
    {
        // todo: go through all neighbors starting with the one with the lowest heuristic
        // todo: have a list of visited nodes
        HashSet<Vector3I> visitedNodes = [];
        PriorityQueue<List<MapNode>, float> pathQueue = new();
        pathQueue.Enqueue([this,], HexPosition.DistanceTo(destination.HexPosition));

        while (pathQueue.Count > 0)
        {
            var currentPath = pathQueue.Dequeue();
            var currentNode = currentPath.Last();
            if (currentNode.HexPosition == destination.HexPosition)
            {
                return currentPath;
            }

            var currentNeighbors = currentNode.OutgoingEdgeList;
            currentNeighbors.ForEach(cn =>
            {
                if (visitedNodes.Add(cn.HexPosition))
                {
                    var newPath = currentPath.ToList();
                    newPath.Add(cn);
                    pathQueue.Enqueue(newPath, cn.HexPosition.DistanceTo(destination.HexPosition));
                }
            });
        }

        return [];
    }

    [OnSerializing]
    private void OnSerializing()
    {
        for (var i = 0; i < _hexPosition.Length; i++)
        {
            _hexPosition[i] = HexPosition[i];
        }
    }

    [OnDeserialized]
    private void OnDeserialized()
    {
        for (var i = 0; i < _hexPosition.Length; i++)
        {
            HexPosition[i] = _hexPosition[i];
        }
    }
}