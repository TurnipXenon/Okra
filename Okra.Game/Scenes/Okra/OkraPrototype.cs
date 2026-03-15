using System.Collections.Generic;
using System.Diagnostics;
using Godot;
using Okra.Core.HexGame;

namespace Okra.Game.Scenes.Okra;

// scene manager
public partial class OkraPrototype : Node
{
    // UI
    [Export] public Button BtnChooseCharacter;
    [Export] public Button BtnClear;
    [Export] public ItemList ListLocations;

    // GameObjects
    [Export] public Node2D CharacterNode;

    // Prototypes
    [Export] public PackedScene HexPrefab;

    private WorldState _worldState;
    private ControllableObject _character;
    private List<MapNode> _availableNodes;

    public override void _Ready()
    {
        Debug.Assert(BtnChooseCharacter != null, "BtnChooseCharacter != null");
        Debug.Assert(BtnClear != null, "BtnClear != null");
        Debug.Assert(ListLocations != null, "ListLocations != null");
        Debug.Assert(CharacterNode != null, "CharacterNode != null");
        Debug.Assert(HexPrefab != null, "HexPrototype != null");

        _worldState = new WorldState();
        _worldState.MapData = SampleMapData.SimpleSample;
        _character = new ControllableObject { MapNode = _worldState.MapData.Graph[0] };
        _worldState.GameObjectList.Add(_character);

        // refresh ui
        ListLocations.Clear();
        _availableNodes = WorldSimulator.CanGo(_worldState, _character).AvailableNodes;
        _availableNodes.ForEach(node =>
        {
            ListLocations.AddItem($"Node: {node.Name}");
        });
        ListLocations.ItemClicked += _listLocations_OnItem_Clicked;

        // generate hex
        _worldState.MapData.Graph.ForEach(node =>
        {
            var child = HexPrefab.Instantiate<Node2D>();
            child.Position = new Vector2(node.Position.X, node.Position.Y);
            AddChild(child);
        });

        CharacterNode.Position = new Vector2(_character.MapNode.Position.X, _character.MapNode.Position.Y);
    }

    // todo: when we click ListLocations we go to that location

    private void _listLocations_OnItem_Clicked(long index, Vector2 atPosition, long mouseButtonIndex)
    {
        if (index >= _availableNodes.Count)
        {
            // todo: warning here
            return;
        }

        WorldSimulator.Move(_worldState, _character, _availableNodes[(int)index]);
        
        // todo: transfer responsibility to core
        CharacterNode.Position = new Vector2(_character.MapNode.Position.X, _character.MapNode.Position.Y);
        ListLocations.Clear();
        _availableNodes = WorldSimulator.CanGo(_worldState, _character).AvailableNodes;
        _availableNodes.ForEach(node =>
        {
            ListLocations.AddItem($"Node: {node.Name}");
        });
    }
}