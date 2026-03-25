namespace Okra.Core.HexGame;

public interface ISelector
{
    MapNode GetPositionNode();
    void RequestCancellation();
}