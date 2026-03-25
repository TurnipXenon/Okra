using Godot;
using JetBrains.Annotations;
using Okra.Core.HexGame;

namespace Okra.Core.Test.HexGame;

[TestSubject(typeof(MapNode))]
public class MapNodeTest
{
    [Fact]
    public void FindShortestPathTo()
    {
        var mapData = new MapData();
        mapData.Generate(100);
        var source = mapData.Graph[new Vector3I(-2, 2, 0)];
        var destination = mapData.Graph[new Vector3I(2, -2, 0)];
        var path = source.FindShortestPathTo(destination);
        Assert.Equal(5, path.Count);
    }
}