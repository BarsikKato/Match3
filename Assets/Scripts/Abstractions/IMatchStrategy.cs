using System.Collections.Generic;

namespace Match3.Abstractions
{
    public interface IMatchStrategy
    {
        bool TryFindMatchingTiles(
            IGameTile tile, 
            out IReadOnlyCollection<IGameTile> match);
    }
}

