using UnityEngine;
using System.Collections.Generic;

public class PuzzleBuilder
{
    #region member

    // public

    // private
    private int _size;
    private int _numberOfTiles;
    private Dictionary<int, int> _numberOfAviablePairs;
    #endregion

    #region functions

    // public
    public PuzzleBuilder()
    {
    }

    public int [ , , ] GenerateLevel(int size, List<int> tileIdentifier)
    {
        if(size % 2 != 0)
            throw new System.ArgumentException("size has to be even");
        _size = size;
        _numberOfTiles = size * size * size;
        //generateAviablePairs(tileIdentifier, _numberOfTiles);
        return new int[size,size,size];
    }

    // private
    #endregion
}
