using System.Collections.Generic;
using System.Diagnostics;
using Godot;

namespace Okra.Core.HexGame;

public static class SampleMapData
{
    class SerializedMapNode
    {
        public string Name = "";
        public Vector3 Position = Vector3.Zero;
        public List<string> OutgoingEdgeList = [];
    }

    static MapData generateMapFromSerializedMapNode(List<SerializedMapNode> serializedMapNodes)
    {
        MapData mapData = new();

        serializedMapNodes.ForEach(mn =>
        {
            // generate nodes
            mapData.Graph.Add(new MapNode { Name = mn.Name, Position = mn.Position });
        });

        mapData.Graph.ForEach(node =>
        {
            var serializedNode = serializedMapNodes.Find(mn => mn.Name.Equals(node.Name));
            if (serializedNode != null)
            {
                serializedNode.OutgoingEdgeList.ForEach(edge =>
                {
                    var edgeNode = mapData.Graph.Find(gn => gn.Name.Equals(edge));
                    if (edgeNode != null)
                    {
                        node.OutgoingEdgeList.Add(edgeNode);
                    }
                    else
                    {
                        Debug.Print($"Warning: Missing serialized node in {node.Name}: Node: ${edge}");
                    }
                });
            }
            else
            {
                Debug.Print($"Warning: Missing serialized node for {node.Name}");
            }
        });

        return mapData;
    }

    // see positioning based on https://www.redblobgames.com/grids/hexagons/#spacing
    public static MapData SimpleSample = generateMapFromSerializedMapNode([
        new SerializedMapNode
        {
            Name = "0",
            Position = new Vector3(1, 0.5f, 0),
            OutgoingEdgeList = ["1", "2", "3",]
        },
        new SerializedMapNode
        {
            Name = "1",
            Position = new Vector3(2, 0.5f, 0),
            OutgoingEdgeList = ["0", "3",]
        },
        new SerializedMapNode
        {
            Name = "2",
            Position = new Vector3(0.5f, 1.25f, 0),
            OutgoingEdgeList = ["0", "3",]
        },
        new SerializedMapNode
        {
            Name = "3",
            Position = new Vector3(1.5f, 1.25f, 0),
            OutgoingEdgeList = ["0", "1", "2",]
        },
    ]);
}