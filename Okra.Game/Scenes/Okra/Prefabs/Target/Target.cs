using System.Linq;
using System.Threading.Tasks;
using Godot;
using Okra.Core.HexGame;

namespace Okra.Game.Scenes.Okra.Prefabs.target;

public partial class Target : Node2D, ISelector
{
    private bool _ignoreInput;
    [Export] public float MovementSpeed = 200f;
    public OkraPrototype OkraPrototype;

    public ISelectable? SelectedItem;
    public HexPawn.HexPawn TargetPositionNode;

    public MapNode GetPositionNode()
    {
        return TargetPositionNode.MapNode;
    }

    public void RequestCancellation()
    {
        SelectedItem?.InformSelectionCancelled();
        SelectedItem = null;
    }

    private void TargetCharacterDefault()
    {
        TargetPositionNode = (HexPawn.HexPawn)OkraPrototype.WorldState.MapData.Graph[Vector3I.Zero].Pawn;
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
        if (TargetPositionNode == null)
        {
            return;
        }

        if (TargetPositionNode.Position != Position)
        {
            Position = Position.MoveToward(TargetPositionNode.Position, (float)delta * MovementSpeed);
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (_ignoreInput)
        {
            return;
        }

        _ignoreInput = true;

        // when i click enter, check state
        // unselected state: interact with current objects
        // selected state: do stuff when enter again
        if (@event.IsActionPressed("ui_accept"))
        {
            if (SelectedItem == null)
            {
                var l = WorldSimulator.GetObjectsAt(OkraPrototype.WorldState, TargetPositionNode.MapNode)
                    .Where(o => o.ControllablePawn != null);
                if (l.Any())
                {
                    // todo: handle multiple items
                    SelectedItem = l.First().ControllablePawn;
                    SelectedItem?.InformSelection(this);
                }
                else
                {
                    // todo: ui that there is no selected item
                    GD.Print($"No selectable item in current hex {TargetPositionNode.MapNode.HexPosition}");
                }

                _ignoreInput = false;
                return;
            }

            // if SelectedItem != null
            // move all input handling to SelectedItem
            // todo: lock the input here
            SelectedItem.UnhandledInputFromSelector(@event).ContinueWith(_ => { _ignoreInput = false; });
            return;
        }


        if (@event.IsActionPressed("ui_cancel"))
        {
            if (SelectedItem != null)
            {
                RequestCancellation();
            }

            _ignoreInput = false;
            return;
        }

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
            var currentVector = TargetPositionNode.MapNode.HexPosition;
            if (OkraPrototype.WorldState.MapData.Graph.TryGetValue(currentVector + offset,
                    out var mapNode))
            {
                TargetPositionNode = (HexPawn.HexPawn)mapNode.Pawn;
            }

            SelectedItem?.InformSelectorStateChanged();
        }

        _ignoreInput = false;
    }
}