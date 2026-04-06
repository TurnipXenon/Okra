using System.Linq;
using Godot;

namespace Okra.Core.HexGame;

public class MapDataDto
{
    public float[] GameOrigin = new float[3];
    public MapNodeDto[] Graph { get; set; }

    public float VectorMultiplier { get; set; }

    public static MapDataDto From(MapData mapData)
    {
        return new MapDataDto
        {
            GameOrigin = Enumerable.Range(0, 3).Select(idx => mapData.GameOrigin[idx]).ToArray(),
            VectorMultiplier = mapData.VectorMultiplier,
            Graph = mapData.Graph.Values.Select(MapNodeDto.From).ToArray(),
        };
    }

    public MapData ToMapData(Node2D hexOrigin)
    {
        var mapData = new MapData();
        mapData.SetGameOrigin(new Vector3(hexOrigin.Position.X, hexOrigin.Position.Y, 0));
        mapData.SetVectorMultiplier(VectorMultiplier);
        foreach (var mapNodeDto in Graph)
        {
            var mapNode = mapNodeDto.ToMapNode(mapData);
            mapData.Graph[mapNode.HexPosition] = mapNode;
        }

        return mapData;
    }
}