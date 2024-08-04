using System.Collections.Generic;

namespace Core.Abstractions
{
    public interface IMatchStrategy
    {
        bool TryFindMatchingTiles(
            IGameTile tile, 
            out IReadOnlyCollection<IGameTile> match);
    }
}

