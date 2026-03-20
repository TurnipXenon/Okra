namespace Okra.Core.HexGame;

public class ControllableObject: OkraObject, IControllable
{
    public IControllablePawn? ControllablePawn { get; set; }

    private MapNode _position = new();
    public MapNode Position
    {
        get => _position;
        set
        {
            if (ControllablePawn != null)
            {
                ControllablePawn.SetPosition(Position.Position);
            }
            
            _position = value;
        }
    }
}