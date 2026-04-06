using System.Linq;

namespace Okra.Core.HexGame;

public class MapDataDto
{
    public float[] GameOrigin = new float[3];
    public MapNode[] Graph { get; set; }

    public float VectorMultiplier { get; set; }

    public static MapDataDto From(MapData mapData)
    {
        return new MapDataDto
        {
            GameOrigin = Enumerable.Range(0, 3).Select(idx => mapData.GameOrigin[idx]).ToArray(),
            VectorMultiplier = mapData.VectorMultiplier,
            Graph = mapData.Graph.Values.ToArray(),
        };
    }
}