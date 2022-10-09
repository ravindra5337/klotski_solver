using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigSquareBlock : Block
{
    public override List<Vector2> GetBlockOccupiedIndex()
    {
        List<Vector2> retval = new List<Vector2>();
        retval.Add(new Vector2(mBlockIndex.x,mBlockIndex.y));
        retval.Add(new Vector2(mBlockIndex.x+1, mBlockIndex.y));
        retval.Add(new Vector2(mBlockIndex.x, mBlockIndex.y+1));
        retval.Add(new Vector2(mBlockIndex.x+1, mBlockIndex.y+1));

        return retval;

    }

    public override BlockType GetBlockType()
    {
        return BlockType.eBigSquare;
    }

  

    protected override int GetLeftLimit()
    {
        List<Vector2> occupiedCells = GetAllOccupiedCellsIndices();

        Vector2 limit = new Vector2(mBlockIndex.x, mBlockIndex.y);
        while (!occupiedCells.Exists((b) => (b.x == limit.x && b.y == limit.y)) && limit.x < mBoardSize.x && limit.x >= 0)
        {

            limit.x--;
        }
        int firstLimit= (int)(limit.x + 1f);

        limit = new Vector2(mBlockIndex.x, mBlockIndex.y+1);
        while (!occupiedCells.Exists((b) => (b.x == limit.x && b.y == limit.y)) && limit.x < mBoardSize.x && limit.x >= 0)
        {

            limit.x--;
        }
        int secondLimit = (int)(limit.x + 1f);

        return secondLimit > firstLimit ? secondLimit : firstLimit;
    }

    protected override int GetLowerLimit()
    {
        List<Vector2> occupiedCells = GetAllOccupiedCellsIndices();
        
        Vector2 limit = new Vector2(mBlockIndex.x, mBlockIndex.y+1);
        while (!occupiedCells.Exists((b) => (b.x == limit.x && b.y == limit.y)) && limit.y < mBoardSize.y && limit.y >= 0)
        {

            limit.y++;
        }

       int firstLimit= (int)(limit.y - 2f);
        limit = new Vector2(mBlockIndex.x+1, mBlockIndex.y+1);
        while (!occupiedCells.Exists((b) => (b.x == limit.x && b.y == limit.y)) && limit.y < mBoardSize.y && limit.y >= 0)
        {

            limit.y++;
        }

        int secLimit = (int)(limit.y - 2f);


        return secLimit < firstLimit ? secLimit : firstLimit;
    }

    protected override int GetRightLimit()
    {
        List<Vector2> occupiedCells = GetAllOccupiedCellsIndices();

        Vector2 limit = new Vector2(mBlockIndex.x+1, mBlockIndex.y);
        while (!occupiedCells.Exists((b) => (b.x == limit.x && b.y == limit.y)) && limit.x < mBoardSize.x && limit.x >= 0)
        {
            limit.x++;
        }

        int firstLimt= (int)(limit.x - 2f);
        
        limit = new Vector2(mBlockIndex.x+1, mBlockIndex.y+1);
        while (!occupiedCells.Exists((b) => (b.x == limit.x && b.y == limit.y)) && limit.x < mBoardSize.x && limit.x >= 0)
        {
            limit.x++;
        }
        int secLimt = (int)(limit.x - 2f);

        return secLimt < firstLimt ? secLimt : firstLimt;
    }

    protected override int GetUpperLimit()
    {
        List<Vector2> occupiedCells = GetAllOccupiedCellsIndices();

        Vector2 limit = new Vector2(mBlockIndex.x, mBlockIndex.y);
        while (!occupiedCells.Exists((b) => (b.x == limit.x && b.y == limit.y)) && limit.y < mBoardSize.y && limit.y >= 0)
        {

            limit.y--;
        }

        int firstLimit= (int)(limit.y + 1f);

         limit = new Vector2(mBlockIndex.x+1, mBlockIndex.y);
        while (!occupiedCells.Exists((b) => (b.x == limit.x && b.y == limit.y)) && limit.y < mBoardSize.y && limit.y >= 0)
        {

            limit.y--;
        }

        int secLimit = (int)(limit.y + 1f);

        return secLimit > firstLimit ? secLimit : firstLimit;
    }
}
