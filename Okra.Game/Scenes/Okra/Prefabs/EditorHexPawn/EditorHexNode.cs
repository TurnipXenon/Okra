using System.Linq;
using Godot;
using Okra.Core.HexGame;

namespace Okra.Game.Scenes.Okra.Prefabs.EditorHexPawn;

// todo: if we make this it should snap to the parent it's on
// todo: draw a grid?

[Tool]
public partial class EditorHexNode : Node2D, IMapNodePawn
{
    private Timer? _stopTimer;
    public Vector3I HexPosition { get; private set; }

    private void DetectHexPosition()
    {
        // position is based on parent so no transformations needed!
        SetNotifyLocalTransform(false);
        HexPosition = MapData.Pixel2DToPointyHex(Position / SampleMapData.VectorMultiplier);
        Position = MapData.PointyHexToPixel(HexPosition) * SampleMapData.VectorMultiplier;
        SetNotifyLocalTransform(true);
    }

    public override void _Ready()
    {
        if (Engine.IsEditorHint())
        {
            DetectHexPosition();

            SetNotifyLocalTransform(true);
            _stopTimer = new Timer();
            _stopTimer.WaitTime = 0.5;
            _stopTimer.OneShot = true;
            _stopTimer.Timeout += DetectHexPosition;
            AddChild(_stopTimer);
        }
    }

    // todo: lifecycle to grab parent all the. do not cache!
    // when to reorient:
    // 1. added
    // 2. parent moved?
    // 3. we moved?
    // todo: how to watch parent but also not cause NullExceptionError???

    private static Vector2[] MultiplyAllVectors()
    {
        return Enumerable.Range(0, 6)
            .Select(idx => PointyHexCorner(idx) * SampleMapData.VectorMultiplier * 0.9f)
            .ToArray();
    }


    // from: https://www.redblobgames.com/grids/hexagons/#angles
    private static Vector2 PointyHexCorner(int index)
    {
        var angleDeg = 60 * index - 30;
        return new Vector2(Mathf.Cos(Mathf.DegToRad(angleDeg)), Mathf.Sin(Mathf.DegToRad(angleDeg)));
    }


    public override void _Draw()
    {
        // We are going to paint with this color.
        var godotBlue = new Color("478cbf");
        // We pass the array of Vector2 to draw the shape.
        DrawPolygon(MultiplyAllVectors(), [godotBlue,]);
    }


    public override void _Notification(int what)
    {
        if (Engine.IsEditorHint())
        {
            if (what == NotificationLocalTransformChanged)
            {
                _stopTimer?.Start();
            }
        }
    }
}