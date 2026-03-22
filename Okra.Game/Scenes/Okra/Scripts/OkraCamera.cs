using Godot;

namespace Okra.Game.Scenes.Okra.Scripts;

public partial class OkraCamera : Camera2D
{
    [Export] public float MovementSpeed = 0.5f;
    [Export] public Node2D Target;
    [Export] public float ZoomMax = 1.5f;
    [Export] public float ZoomMin = 0.2f;

    // todo: follow character
    [Export] public float ZoomStrength = 0.1f;

    public override void _Process(double delta)
    {
        if (Target == null)
        {
            return;
        }

        if (Target.Position != Position)
        {
            GD.Print($"Cam:  {Position} Target: {Target.Position}");
            Position = Position.MoveToward(Target.Position, (float)delta * MovementSpeed);
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton eventMouse)
        {
            if (eventMouse.IsPressed())
            {
                var nextZoom = Zoom;
                if (eventMouse.ButtonIndex == MouseButton.WheelUp)
                {
                    nextZoom = Zoom + Vector2.One * ZoomStrength;
                }
                else if (eventMouse.ButtonIndex == MouseButton.WheelDown)
                {
                    nextZoom = Zoom - Vector2.One * ZoomStrength;
                }

                if (nextZoom.X < ZoomMin)
                {
                    Zoom = Vector2.One * ZoomMin;
                }
                else if (nextZoom.X > ZoomMax)
                {
                    Zoom = Vector2.One * ZoomMax;
                }
                else
                {
                    Zoom = nextZoom;
                }
            }
        }
    }
}