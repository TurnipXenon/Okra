using System.Threading.Tasks;
using Godot;

namespace Okra.Core.HexGame;

public interface IControllablePawn
{
    Task<MutationState> SetPosition(Vector3 position);
    void ForcePosition(Vector3 position);
}