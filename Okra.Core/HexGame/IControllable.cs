namespace Okra.Core.HexGame;

public interface IControllable
{
    string Name { get; set; }
    MapNode Position { get; set; }
}