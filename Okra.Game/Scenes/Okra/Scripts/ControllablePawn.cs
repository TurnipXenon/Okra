using System.Diagnostics;
using System.Threading.Tasks;
using Godot;
using Okra.Core.HexGame;
using Environment = System.Environment;

namespace Okra.Game.Scenes.Okra.Scripts;

public partial class ControllablePawn : Node2D, IControllablePawn, ISelectable
{
    private MapNode _positionNode;
    private ISelector? _selector;
    [Export] public float MovementSpeed = SampleMapData.VectorMultiplier;

    public async Task<MutationState> SetPosition(MapNode positionNode)
    {
        _positionNode = positionNode;
        var newPosition = new Vector2(positionNode.GamePosition.X, positionNode.GamePosition.Y);
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

    public void InformSelection(ISelector selector)
    {
        GD.Print($"Selected item: {Name}");
        _selector = selector;
    }

    public void InformSelectionCancelled()
    {
        GD.Print($"Item deselected: {Name}");
        _selector = null;
    }

    public void InformSelectorStateChanged()
    {
        Debug.Assert(_selector != null, "_selector != null");
        if (_selector != null)
        {
            GD.Print($"This should never happen. Stack trace: {Environment.StackTrace}");
            // ReSharper disable once RedundantJumpStatement
            return;
        }

        // todo: possibly run async if map too big?
        var positionList = _positionNode.FindShortestPathTo(_selector!.GetPositionNode());

        // todo: find set of HexPawn from start to finish?
        // todo: delegate this to Core and not Godot
        // todo: draw lines?
        // todo: this specific logic should be for character pawn (more specific version of controllable pawn)
    }

    public override void _Ready()
    {
        Debug.Assert(MovementSpeed > 0);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (_selector == null)
        {
            // ReSharper disable once RedundantJumpStatement
            return;
        }

        // todo: pass this specific function for the character pawn and not controllable pawn?
    }
}