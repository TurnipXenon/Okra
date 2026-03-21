using System.Collections.Generic;
using System.Diagnostics;
using Godot;

namespace Okra.Core.HexGame;

/**
 * Thank you https://www.redblobgames.com/grids/hexagons/
 */
public class MapData
{
    public static readonly List<Vector3I> HexOffset =
    [
        // q, r, s
        new(0, -1, 1),
        new(1, -1, 0),
        new(1, 0, -1),
        new(0, 1, -1),
        new(-1, 1, 0),
        new(-1, 0, 1),
    ];

    public readonly HashSet<Vector3I> CalculatedNode = [];

    public readonly Dictionary<Vector3I, MapNode> Graph = [];

    public Vector3 GameOrigin { get; private set; }

    public float VectorMultiplier { get; private set; }

    public void SetGameOrigin(Vector3 _gameOrigin)
    {
        GameOrigin = _gameOrigin;
        CalculatedNode.Clear();
    }

    public void SetVectorMultiplier(float _vectorMultiplier)
    {
        VectorMultiplier = _vectorMultiplier;
        CalculatedNode.Clear();
    }


    public static Vector2 PointyHexToPixel(Vector3I point)
    {
        return new Vector2(
            Mathf.Sqrt(3) * point.X + Mathf.Sqrt(3) / 2 * point.Y,
            3f * point.Y / 2f);
    }

    public void Generate(int nodeCount)
    {
        Graph.Clear();

        if (nodeCount < 1)
        {
            Debug.WriteLine($"MapData.Generate: Invalid nodeCount: {nodeCount}");
            return;
        }

        var nodeQueue = new Queue<MapNode>();
        var currentNode = new MapNode
        {
            Parent = this,
            HexPosition = new Vector3I(0, 0, 0),
        };
        Graph.Add(
            currentNode.HexPosition,
            currentNode
        );
        nodeQueue.Enqueue(currentNode);
        var manualOffset = 0;
        for (var i = 1; i < nodeCount; i++)
        {
            var offsetIdx = (i + manualOffset) % HexOffset.Count;
            if (offsetIdx == 0 && nodeQueue.Count > 0)
            {
                currentNode = nodeQueue.Dequeue();
            }

            if (Graph.ContainsKey(currentNode.HexPosition + HexOffset[offsetIdx]))
            {
                // retry loop
                i--;
                manualOffset++;
                continue;
            }

            var newNode = new MapNode
            {
                Parent = this,
                HexPosition = currentNode.HexPosition + HexOffset[offsetIdx],
            };
            nodeQueue.Enqueue(newNode);
            Graph.Add(newNode.HexPosition, newNode);
        }
    }
}