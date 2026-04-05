using System.Collections.Generic;
using System.Diagnostics;
using Godot;

namespace Okra.Core.HexGame;

/**
 * Thank you https://www.redblobgames.com/grids/hexagons/
 */
public class MapData
{
    public static readonly Vector3I HexUp = new(1, -1, 0);
    public static readonly Vector3I HexDown = new(-1, 1, 0);
    public static readonly Vector3I HexRight = new(1, 0, -1);
    public static readonly Vector3I HexLeft = new(-1, 0, 1);

    public static readonly List<Vector3I> HexOffset =
    [
        // q, r, s
        new(0, -1, 1),
        HexUp,
        HexRight,
        new(0, 1, -1),
        HexDown,
        HexLeft,
    ];

    public readonly HashSet<Vector3I> CalculatedNode = [];

    public readonly Dictionary<Vector3I, MapNode> Graph = [];

    public Vector3 GameOrigin { get; private set; }

    public float VectorMultiplier { get; private set; }

    public static Vector3I CubeRound(Vector3 fractionalCube)
    {
        var roundedCube = new Vector3I(
            Mathf.RoundToInt(fractionalCube.X),
            Mathf.RoundToInt(fractionalCube.Y),
            Mathf.RoundToInt(fractionalCube.Z)
        );
        var differenceCube = fractionalCube - roundedCube;
        if (differenceCube.X > differenceCube.Y && differenceCube.X > differenceCube.Z)
        {
            roundedCube.X = -roundedCube.Y - roundedCube.Z;
        }
        else if (differenceCube.Y > differenceCube.Z)
        {
            roundedCube.Y = -roundedCube.X - roundedCube.Z;
        }
        else
        {
            roundedCube.Z = -roundedCube.X - roundedCube.Y;
        }

        return roundedCube;
    }

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


    // todo Axial to Cube coord
    private static Vector3 FractionalAxialToFractionalCube(Vector2 axial)
    {
        return new Vector3(
            axial.X,
            axial.Y,
            -axial.X - axial.Y
        );
    }

    /**
     * It is your responsibility to send normalized game positions here
     */
    public static Vector3I Pixel2DToPointyHex(Vector2 gamePosition)
    {
        return PixelToPointyHex(new Vector3(
            gamePosition.X,
            gamePosition.Y,
            -gamePosition.X - gamePosition.Y
        ));
    }

    /**
 * It is your responsibility to send normalized game positions here
 */
    public static Vector3I PixelToPointyHex(Vector3 gamePosition)
    {
        var axial = new Vector2(Mathf.Sqrt(3) / 3 * gamePosition.X - gamePosition.Y / 3,
            gamePosition.Y * 2 / 3);
        return CubeRound(FractionalAxialToFractionalCube(axial));
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