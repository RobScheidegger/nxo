using System;
using System.Collections.Generic;
using System.Linq;

namespace NXO.Server.Modules.TicTacToe;

public struct TicTacToePosition
{
    public TicTacToePosition(List<int> location, int hash)
    {
        Location = location;
        Hash = hash;
    }
    public List<int> Location { get; set; } 
    public int Hash { get; set; }
    public override string ToString()
    {
        return string.Join(',', Location.Select(i => i.ToString()));
    }
}
