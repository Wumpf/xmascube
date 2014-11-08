using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PuzzleBuilder
{
    #region member

    // public

    // private
    private int _size;
    private int _numberOfTiles;
    private Dictionary<int, int> _numberOfAvialablePairs;
    private Dictionary<Vector3, int> _freePositions;
    #endregion

    #region functions

    // public
    public PuzzleBuilder()
    {
    }

    public int [ , , ] GenerateLevel(int size, List<int> tileIdentifier)
    {
        if(size % 2 != 1)
            throw new System.ArgumentException("size has to be odd");
        _size = size;
        _numberOfTiles = size * size * size;
        generateAvialablePairs(tileIdentifier, _numberOfTiles);
        return generateLevel();
    }

    public List<Vector3> GetNeighbors(Vector3 forPosition)
    {
        var neighborPositions = new Vector3[] 
        { 
            new Vector3(forPosition.x + 1, forPosition.y, forPosition.z),
            new Vector3(forPosition.x - 1, forPosition.y, forPosition.z),
            new Vector3(forPosition.x, forPosition.y + 1, forPosition.z),
            new Vector3(forPosition.x, forPosition.y - 1, forPosition.z),
            new Vector3(forPosition.x, forPosition.y, forPosition.z + 1),
            new Vector3(forPosition.x, forPosition.y, forPosition.z - 1)
        };

        return neighborPositions.Where( nPos => (nPos.x < _size && nPos.y < _size && nPos.z < _size && nPos.x > -1 && nPos.y > -1 && nPos.z > -1) ).ToList();
    }
    // private

    private void generateAvialablePairs(List<int> tileIdentifier, int numberOfTiles)
    {
        numberOfTiles /= 2;

        var pairCount = numberOfTiles /tileIdentifier.Count;
        var rest = numberOfTiles - (pairCount * tileIdentifier.Count);
        _numberOfAvialablePairs = tileIdentifier.ToDictionary<int,int,int>( tileId => tileId, tileId => pairCount );

        while(rest-- != 0)
        {
            _numberOfAvialablePairs[_numberOfAvialablePairs.Keys.ToList()[Random.Range(0,_numberOfAvialablePairs.Keys.Count)]] += 1;
        }
        if(_numberOfTiles != _numberOfAvialablePairs.Values.Sum())
            Debug.Log(string.Format("Wrong number of tiles {0} {1}", numberOfTiles, _numberOfAvialablePairs.Values.Sum()));
    }


    private int[,,] generateLevel()
    {
        var result = new int[_size,_size,_size];
        result.FillAllWithValue(-1);
        result[_size/2,_size/2,_size/2] = 0;
        _freePositions = getDictionaryOfFreePositions(result);
        while(_numberOfAvialablePairs.Any())
        {
            var key = _numberOfAvialablePairs.Keys.ToList()[Random.Range(0,_numberOfAvialablePairs.Keys.Count)];
            var pairs = _numberOfAvialablePairs[key];

            var pos1 = getRandomFreePosition(result);
            _freePositions.Remove(pos1);
            var pos2 = getRandomFreePosition(result);
            _freePositions.Remove(pos2);

            result[(int)pos1.x,(int)pos1.y,(int)pos1.z] = key;
            result[(int)pos2.x,(int)pos2.y,(int)pos2.z] = key;
            foreach(var pos in GetNeighbors(pos1))
                if(_freePositions.ContainsKey(pos)) _freePositions[pos] ++;
            foreach(var pos in GetNeighbors(pos2))
                if(_freePositions.ContainsKey(pos)) _freePositions[pos] ++;
            if(--pairs == 0)
                _numberOfAvialablePairs.Remove(key);
            else
                _numberOfAvialablePairs[key] = pairs;
        }

        return result;
    }

    private Vector3 getRandomFreePosition(int[,,] field)
    {
        var sort = _freePositions.OrderByDescending(kv => kv.Value);
        var possiblePos = sort.Where(kv => kv.Value == sort.First().Value).Select<KeyValuePair<Vector3, int>, Vector3>(
            kv => kv.Key ).ToList();
                
        return possiblePos[Random.Range(0, possiblePos.Count)];
    }

    private List<Vector3> getListOfFreePositions(int[,,] field)
    {
        var result = new List<Vector3> ();
        for(int i = 0; i < field.GetLength(0); ++i)
            for(int j = 0; j < field.GetLength(1); ++j)
                for(int k = 0; k < field.GetLength(2); ++k)
            {
                if(field[i,j,k] == -1)
                    result.Add(new Vector3(i,j,k));
            }
        return result;
    }

    private Dictionary<Vector3, int> getDictionaryOfFreePositions(int[,,] field)
    {
        var result = new Dictionary<Vector3, int> ();
        var tempPos = Vector3.zero;
        for(int i = 0; i < field.GetLength(0); ++i)
            for(int j = 0; j < field.GetLength(1); ++j)
                for(int k = 0; k < field.GetLength(2); ++k)
            {
                if(field[i,j,k] == -1)
                {
                    tempPos.x = i;
                    tempPos.y = j;
                    tempPos.z = k;
                    result.Add(tempPos, GetNeighbors(tempPos).Where( n => field[(int)n.x,(int)n.y,(int)n.z] != -1).Count());
                }
            }
        return result;
    }

    #endregion
}
