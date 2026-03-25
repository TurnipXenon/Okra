using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Okra.Core.HexGame;
using Environment = System.Environment;

namespace Okra.Game.Scenes.Okra.Scripts;

public partial class ControllablePawn : Node2D, IControllablePawn, ISelectable
{
    private ControllableObject _controllableObject;
    private List<MapNode> _currentPath;
    private MapNode _positionNode;
    private ISelector? _selector;
    private WorldState _worldState;
    [Export] public Line2D CharacterLine;
    [Export] public int MaxWalkDistance = 5;
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

    public void SetWorldState(WorldState worldState)
    {
        _worldState = worldState;
    }

    public void SetCoreObject(ControllableObject controllableObject)
    {
        _controllableObject = controllableObject;
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
        CharacterLine.Points = [];
    }

    public void InformSelectorStateChanged()
    {
        Debug.Assert(_selector != null, "_selector != null");
        if (_selector == null)
        {
            GD.Print($"This should never happen. Stack trace:\n{Environment.StackTrace}");
            // ReSharper disable once RedundantJumpStatement
            return;
        }

        // todo: possibly run async if map too big?
        // todo: delegate this to Core and not Godot
        var path = _positionNode.FindShortestPathTo(_selector!.GetPositionNode());
        if (path.Count > MaxWalkDistance)
        {
            return;
        }

        _currentPath = path;
        CharacterLine.Points = path.Select(p =>
            new Vector2(p.GamePosition.X, p.GamePosition.Y)).ToArray();
    }

    public async Task UnhandledInputFromSelector(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_accept") && _selector != null && _currentPath.Count > 0)
        {
            if (_selector.GetPositionNode().HexPosition != _currentPath.Last().HexPosition)
            {
                // todo: signal ui indications
                GD.Print("Cannot go to place beyond limit");
                return;
            }

            // todo: freeze all input
            var path = CharacterLine.Points;
            CharacterLine.Points = [];
            foreach (var mn in _currentPath)
            {
                await Task.WhenAll(WorldSimulator.Move(
                    _worldState,
                    _controllableObject,
                    mn).PawnTaskList);
            }

            _selector.RequestCancellation();
        }
    }

    public override void _Ready()
    {
        Debug.Assert(CharacterLine != null);
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