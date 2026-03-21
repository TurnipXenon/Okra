using Godot;
using JetBrains.Annotations;
using Okra.Core.HexGame;

namespace Okra.Core.Test.HexGame;

[TestSubject(typeof(MapData))]
public class MapDataTest
{
    // todo: generate invalid integer

    [Fact]
    public void GenerateOneItem()
    {
        var mapData = MapData.Generate(Vector3.Zero, 1);
        Assert.True(mapData.Graph.ContainsKey(Vector3I.Zero));
        Assert.Single(mapData.Graph);
    }

    [Fact]
    public void GenerateSevenItems()
    {
        var mapData = MapData.Generate(Vector3.Zero, 7);
        Assert.True(mapData.Graph.ContainsKey(Vector3I.Zero));
        MapData.HexOffset.ForEach(hx => { Assert.True(mapData.Graph.ContainsKey(hx)); });
        Assert.Equal(7, mapData.Graph.Count);
    }

    [Fact]
    public void GenerateTwelveItems()
    {
        var mapData = MapData.Generate(Vector3.Zero, 19);
        Assert.True(mapData.Graph.ContainsKey(Vector3I.Zero));
        MapData.HexOffset.ForEach(hx => { Assert.True(mapData.Graph.ContainsKey(hx)); });
        Assert.Equal(19, mapData.Graph.Count);
    }
}