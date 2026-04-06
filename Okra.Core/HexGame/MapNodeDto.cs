using System.Text.Json.Serialization;
using Godot;

namespace Okra.Core.HexGame;

public class MapNodeDto
{
    [JsonInclude] public int[] HexPosition = new int[3];

    public static MapNodeDto From(MapNode mapNode)
    {
        return new MapNodeDto
        {
            HexPosition =
            [
                mapNode.HexPosition.X,
                mapNode.HexPosition.Y,
                mapNode.HexPosition.Z,
            ],
        };
    }

    public MapNode ToMapNode(MapData mapData)
    {
        return new MapNode
        {
            HexPosition = new Vector3I(HexPosition[0], HexPosition[1], HexPosition[2]),
            Parent = mapData,
        };
    }
}