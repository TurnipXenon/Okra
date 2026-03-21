using System.Diagnostics;
using System.Threading.Tasks;
using Godot;
using Okra.Core.HexGame;

namespace Okra.Game.Scenes.Okra.Scripts;

public partial class ControllablePawn: Node2D, IControllablePawn
{
    [Export] public float AnimationSpeed = SampleMapData.VectorMultiplier;

    public override void _Ready()
    {
        Debug.Assert(AnimationSpeed > 0);
    }

    public async Task<MutationState> SetPosition(Vector3 position)
    {
        var newPosition = new Vector2(position.X, position.Y);
        var duration = AnimationSpeed > 0 ? Position.DistanceTo(newPosition) / AnimationSpeed : 0f;

        var tween = CreateTween();
        tween.TweenProperty(this, "position", newPosition, duration);
        await ToSignal(tween, Tween.SignalName.Finished);

        return MutationState.Mutated;
    }

    public void ForcePosition(Vector3 position)
    {
        Position = new Vector2(position.X, position.Z);
    }
}