using System.Threading.Tasks;
using Godot;

namespace Okra.Game.Scenes.Okra.Prefabs.target;

public partial class Target : Node2D
{
    [Export] public float MovementSpeed = 200f;
    public OkraPrototype OkraPrototype;

    public Node2D TargetNode;

    private void TargetCharacterDefault()
    {
        TargetNode = (Node2D)OkraPrototype.WorldState.MapData.Graph[Vector3I.Zero].Pawn;
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
}