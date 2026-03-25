namespace Okra.Core.HexGame;

public interface ISelectable
{
    void InformSelection(ISelector selector);
    void InformSelectionCancelled();
    void InformSelectorStateChanged();
}