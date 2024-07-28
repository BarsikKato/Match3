using System.Collections.Generic;

namespace Core.Abstractions
{
    public interface IMatchStrategy
    {
        bool TryMatch(
            IGameTile tileA, IGameTile tileB, 
            out IReadOnlyCollection<IGameTile> matchA,
            out IReadOnlyCollection<IGameTile> matchB);
    }
}

