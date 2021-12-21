using System;
using System.Collections.Generic;
using System.Linq;

namespace NXO.Shared.Modules.TicTacToe
{
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
        public override bool Equals(object obj)
        {
            return obj is TicTacToePosition other && other.Hash == Hash;
        }

        public override int GetHashCode()
        {
            return Hash;
        }

        public static bool operator ==(TicTacToePosition a, TicTacToePosition b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(TicTacToePosition a, TicTacToePosition b)
        {
            return !a.Equals(b);
        }
    }
}