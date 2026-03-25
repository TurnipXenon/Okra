using System.Threading.Tasks;
using Godot;

namespace Okra.Core.HexGame;

public interface ISelectable
{
    void InformSelection(ISelector selector);
    void InformSelectionCancelled();
    void InformSelectorStateChanged();
    Task UnhandledInputFromSelector(InputEvent @event);
}