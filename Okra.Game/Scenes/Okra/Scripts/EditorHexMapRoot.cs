using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Godot;
using Okra.Core.HexGame;
using Okra.Game.Scenes.Okra.Prefabs.EditorHexPawn;

namespace Okra.Game.Scenes.Okra.Scripts;

[Tool]
public partial class EditorHexMapRoot : Node2D
{
    [Export] public Resource? HexMapResource;

    [ExportToolButton("Save map")] private Callable SaveCurrentMapToJsonButton => Callable.From(SaveCurrentMapToJson);

    public override void _Ready()
    {
        if (!Engine.IsEditorHint())
        {
            QueueFree();
        }
    }

    private void SaveCurrentMapToJson()
    {
        if (HexMapResource == null)
        {
            GD.PushWarning("HexMapResource not set to a file to write to");
            return;
        }

        if (!HexMapResource.GetPath().EndsWith(".hexmap.json"))
        {
            GD.PrintErr("HexMapResource should end in *.hexmap.json");
            return;
        }

        var children = GetChildren().OfType<EditorHexNode>();
        var mapData = ConvertToMapData(this, children);

        using var file = FileAccess.Open(HexMapResource.GetPath(), FileAccess.ModeFlags.Write);
        file.StoreString(JsonSerializer.Serialize(MapDataDto.From(mapData),
            new JsonSerializerOptions { WriteIndented = true, }));
    }

    private static MapData ConvertToMapData(EditorHexMapRoot editorHexMapRoot, IEnumerable<EditorHexNode> children)
    {
        var mapData = new MapData();
        mapData.SetGameOrigin(new Vector3(editorHexMapRoot.Position.X, editorHexMapRoot.Position.Y, 0));
        mapData.SetVectorMultiplier(SampleMapData.VectorMultiplier);
        foreach (var editorHexNode in children)
        {
            var mapNode = new MapNode
            {
                HexPosition = editorHexNode.HexPosition,
                Parent = mapData,
            };
            mapData.Graph[editorHexNode.HexPosition] = mapNode;
        }

        return mapData;
    }
}