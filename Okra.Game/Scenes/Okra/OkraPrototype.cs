using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Godot;
using Okra.Core.HexGame;
using Okra.Game.Scenes.Okra.Scripts;
using System;

namespace Okra.Game.Scenes.Okra;

// scene manager
public partial class OkraPrototype : Node
{
    // UI
    [Export] public Button BtnChooseCharacter;
    [Export] public Button BtnClear;
    [Export] public ItemList ListLocations;

    // GameObjects
    [Export] public ControllablePawn CharacterNode;

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
        _character = new ControllableObject();
        _character.SetPosition(_worldState.MapData.Graph[0]);
        _worldState.GameObjectList.Add(_character);

        // refresh ui
        ListLocations.Clear();
        _availableNodes = WorldSimulator.CanGo(_worldState, _character).AvailableNodes;
        _availableNodes.ForEach(node => { ListLocations.AddItem($"Node: {node.Name}"); });
        ListLocations.ItemClicked += _listLocations_OnItem_Clicked;

        // generate hex
        _worldState.MapData.Graph.ForEach(node =>
        {
            var child = HexPrefab.Instantiate<Node2D>();
            child.Position = new Vector2(node.Position.X, node.Position.Y);
            AddChild(child);
        });

        CharacterNode.Position = new Vector2(_character.Position.Position.X, _character.Position.Position.Y);
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
            Debug.WriteLine("Warning: Index out of range: " + System.Environment.StackTrace);
            return;
        }

        ListLocations.Clear();
        var moveResponse = WorldSimulator.Move(_worldState, _character, _availableNodes[(int)index]);

        Task.WhenAll(moveResponse.PawnTaskList).ContinueWith(_ =>
        {
            CallDeferred(MethodName.UpdateAvailableNodes);
        });
    }
}