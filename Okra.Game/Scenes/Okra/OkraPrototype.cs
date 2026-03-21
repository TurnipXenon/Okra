using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Godot;
using Okra.Core.HexGame;
using Okra.Game.Scenes.Okra.Scripts;
using Environment = System.Environment;

namespace Okra.Game.Scenes.Okra;

// scene manager
public partial class OkraPrototype : Node
{
    private List<MapNode> _availableNodes;
    private ControllableObject _character;

    private WorldState _worldState;

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
    [Export] public float VectorMultiplier = 100;

    public override void _Ready()
    {
        Debug.Assert(BtnChooseCharacter != null, "BtnChooseCharacter != null");
        Debug.Assert(BtnClear != null, "BtnClear != null");
        Debug.Assert(ListLocations != null, "ListLocations != null");
        Debug.Assert(CharacterNode != null, "CharacterNode != null");
        Debug.Assert(HexPrefab != null, "HexPrototype != null");
        Debug.Assert(HexOrigin != null, "HexOrigin != null");
        Debug.Assert(NodeNumber >= 4, "NodeNumber >= 4");

        // todo: defer in the future?

        _worldState = new WorldState();
        _worldState.MapData.SetGameOrigin(new Vector3(HexOrigin.Position.X, HexOrigin.Position.Y, 0));
        _worldState.MapData.SetVectorMultiplier(VectorMultiplier);
        _worldState.MapData.Generate(NodeNumber);
        _character = new ControllableObject();
        _character.SetPosition(_worldState.MapData.Graph[Vector3I.Zero]);
        _worldState.GameObjectList.Add(_character);

        // refresh ui
        ListLocations.Clear();
        _availableNodes = WorldSimulator.CanGo(_worldState, _character).AvailableNodes;
        _availableNodes.ForEach(node => { ListLocations.AddItem($"Node: {node.Name}"); });
        ListLocations.ItemClicked += _listLocations_OnItem_Clicked;

        // generate hex
        foreach (var keyValuePair in _worldState.MapData.Graph)
        {
            var child = HexPrefab.Instantiate<Node2D>();
            var v3 = keyValuePair.Value.GamePosition;
            child.Position = new Vector2(v3.X, v3.Y);
            AddChild(child);
        }

        CharacterNode.Position = new Vector2(_character.Position.GamePosition.X, _character.Position.GamePosition.Y);
        _character.ControllablePawn = CharacterNode;
    }

    // todo: when we click ListLocations we go to that location


    private void UpdateAvailableNodes()
    {
        _availableNodes = WorldSimulator.CanGo(_worldState, _character).AvailableNodes;
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
        var moveResponse = WorldSimulator.Move(_worldState, _character, _availableNodes[(int)index]);

        Task.WhenAll(moveResponse.PawnTaskList).ContinueWith(_ => { CallDeferred(MethodName.UpdateAvailableNodes); });
    }
}