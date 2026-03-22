using System.Diagnostics;
using System.Threading.Tasks;
using Godot;
using Okra.Core.HexGame;

namespace Okra.Game.Scenes.Okra.Scripts;

public partial class ControllablePawn : Node2D, IControllablePawn
{
    [Export] public float MovementSpeed = SampleMapData.VectorMultiplier;

    public async Task<MutationState> SetPosition(Vector3 position)
    {
        var newPosition = new Vector2(position.X, position.Y);
        var duration = MovementSpeed > 0 ? Position.DistanceTo(newPosition) / MovementSpeed : 0f;

        var tween = CreateTween();
        tween.TweenProperty(this, "position", newPosition, duration);
        await ToSignal(tween, Tween.SignalName.Finished);

        return MutationState.Mutated;
    }

    public void ForcePosition(Vector3 position)
    {
        Position = new Vector2(position.X, position.Z);
    }

    public override void _Ready()
    {
        Debug.Assert(MovementSpeed > 0);
    }
}