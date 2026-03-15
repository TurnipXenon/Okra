using System.Collections.Generic;

namespace Okra.Core.HexGame;

public class WorldState
{
    public List<OkraObject> GameObjectList = [];
    public MapData MapData = new();
}