using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Godot;
using Okra.Core.HexGame;
using Okra.Game.Scenes.Okra.Prefabs.hex_pawn;
using Okra.Game.Scenes.Okra.Prefabs.target;
using Okra.Game.Scenes.Okra.Scripts;
using Environment = System.Environment;

namespace Okra.Game.Scenes.Okra;

// scene manager
public partial class OkraPrototype : Node
{
    public enum StateType
    {
        Preparing,
        Ready,
    }

    private List<MapNode> _availableNodes;
    private ControllableObject _character;

    // UI
    [Export] public Button BtnChooseCharacter;
    [Export] public Button BtnClear;

    // GameObjects
    [Export] public ControllablePawn CharacterNode;
    [Export] public Node2D HexOrigin;

    // Prototypes
    [Export] public PackedScene HexPrefab;
    [Export] public ItemList ListLocations;

    // Variables
    [Export] public int NodeNumber = 4;

    public StateType State = StateType.Preparing;
    [Export] public Target Target;
    [Export] public float VectorMultiplier = 100;

    public WorldState WorldState;

    public override void _Ready()
    {
        Debug.Assert(BtnChooseCharacter != null, "BtnChooseCharacter != null");
        Debug.Assert(BtnClear != null, "BtnClear != null");
        Debug.Assert(ListLocations != null, "ListLocations != null");
        Debug.Assert(CharacterNode != null, "CharacterNode != null");
        Debug.Assert(HexPrefab != null, "HexPrototype != null");
        Debug.Assert(HexOrigin != null, "HexOrigin != null");
        Debug.Assert(NodeNumber >= 4, "NodeNumber >= 4");

        Debug.Assert(Target != null, "Target != null");
        Target.OkraPrototype = this;

        // todo: defer in the future?

        WorldState = new WorldState();
        WorldState.MapData.SetGameOrigin(new Vector3(HexOrigin.Position.X, HexOrigin.Position.Y, 0));
        WorldState.MapData.SetVectorMultiplier(VectorMultiplier);
        WorldState.MapData.Generate(NodeNumber);
        _character = new ControllableObject();
        _character.SetPosition(WorldState.MapData.Graph[Vector3I.Zero]);
        WorldState.GameObjectList.Add(_character);

        // refresh ui
        ListLocations.Clear();
        _availableNodes = WorldSimulator.CanGo(WorldState, _character).AvailableNodes;
        _availableNodes.ForEach(node => { ListLocations.AddItem($"Node: {node.Name}"); });
        ListLocations.ItemClicked += _listLocations_OnItem_Clicked;

        // generate hex
        foreach (var keyValuePair in WorldState.MapData.Graph)
        {
            var child = HexPrefab.Instantiate<HexPawn>();
            child.SetMapNode(keyValuePair.Value, this);
        }

        // todo: figure out how to enforce possessing?
        _character.ControllablePawn = CharacterNode;
        CharacterNode.SetPosition(WorldState.MapData.Graph[Vector3I.Zero]).ContinueWith(_ =>
        {
            State = StateType.Ready;
        });
    }

    // todo: when we click ListLocations we go to that location


    private void UpdateAvailableNodes()
    {
        _availableNodes = WorldSimulator.CanGo(WorldState, _character).AvailableNodes;
        _availableNodes.ForEach(node => { ListLocations.AddItem($"Node: {node.Name}"); });
    }

    private void _listLocations_OnItem_Clicked(long index, Vector2 atPosition, long mouseButtonIndex)
    {
        if (index >= _availableNodes.Count)
        {
            Debug.WriteLine("Warning: Index out of range: " + Environment.StackTrace);
            return;
        }

        ListLocations.Clear();
        var moveResponse = WorldSimulator.Move(WorldState, _character, _availableNodes[(int)index]);

        Task.WhenAll(moveResponse.PawnTaskList).ContinueWith(_ => { CallDeferred(MethodName.UpdateAvailableNodes); });
    }
}