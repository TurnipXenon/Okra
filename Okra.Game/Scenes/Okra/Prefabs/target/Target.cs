using System.Threading.Tasks;
using Godot;
using Okra.Core.HexGame;
using Okra.Game.Scenes.Okra.Prefabs.hex_pawn;

namespace Okra.Game.Scenes.Okra.Prefabs.target;

public partial class Target : Node2D
{
    [Export] public float MovementSpeed = 200f;
    public OkraPrototype OkraPrototype;

    public HexPawn TargetNode;

    private void TargetCharacterDefault()
    {
        TargetNode = (HexPawn)OkraPrototype.WorldState.MapData.Graph[Vector3I.Zero].Pawn;
    }

    private async Task WaitForReady()
    {
        while (OkraPrototype != null && OkraPrototype.State != OkraPrototype.StateType.Ready)
        {
            await Task.Yield();
        }

        CallDeferred("TargetCharacterDefault");
    }

    public override void _Ready()
    {
        _ = WaitForReady();
    }

    public override void _Process(double delta)
    {
        if (TargetNode == null)
        {
            return;
        }

        if (TargetNode.Position != Position)
        {
            Position = Position.MoveToward(TargetNode.Position, (float)delta * MovementSpeed);
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        var offset = Vector3I.Zero;
        if (@event.IsActionPressed("ui_left"))
        {
            offset = MapData.HexLeft;
        }
        else if (@event.IsActionPressed("ui_right"))
        {
            offset = MapData.HexRight;
        }
        else if (@event.IsActionPressed("ui_up"))
        {
            offset = MapData.HexUp;
        }
        else if (@event.IsActionPressed("ui_down"))
        {
            offset = MapData.HexDown;
        }

        if (offset != Vector3I.Zero)
        {
            var currentVector = TargetNode.MapNode.HexPosition;
            if (OkraPrototype.WorldState.MapData.Graph.TryGetValue(currentVector + offset,
                    out var mapNode))
            {
                TargetNode = (HexPawn)mapNode.Pawn;
            }
        }
    }
}