using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HorizontalBlock : Block
{
    
    public override List<Vector2> GetBlockOccupiedIndex()
    {
        List<Vector2> retval = new List<Vector2>();
        retval.Add(mBlockIndex);
        retval.Add(new Vector2(mBlockIndex.x+1, mBlockIndex.y ));
        return retval;

    }



    protected override int GetLowerLimit()
    {
        List<Vector2> occupiedCells = GetAllOccupiedCellsIndices();
        Vector2 limit = new Vector2(mBlockIndex.x, mBlockIndex.y );
        while (!occupiedCells.Exists((b) => (b.x == limit.x && b.y == limit.y)) && limit.y < mBoardSize.y && limit.y >= 0)
        {
            limit.y++;
        }

        int firstLimit= (int)(limit.y - 1f);


         limit = new Vector2(mBlockIndex.x+1, mBlockIndex.y);
        while (!occupiedCells.Exists((b) => (b.x == limit.x && b.y == limit.y)) && limit.y < mBoardSize.y && limit.y >= 0)
        {
            limit.y++;
        }
        int secLimit = (int)(limit.y - 1f);

        return secLimit < firstLimit ? secLimit : firstLimit;

    }

    protected override int GetLeftLimit()
    {
        List<Vector2> occupiedCells = GetAllOccupiedCellsIndices();

        Vector2 limit = new Vector2(mBlockIndex.x, mBlockIndex.y);
        while (!occupiedCells.Exists((b) => (b.x == limit.x && b.y == limit.y)) && limit.x < mBoardSize.x && limit.x >= 0)
        {

            limit.x--;
        }

        int firstLimit = (int)(limit.x + 1f);

      

        return  firstLimit;

    }

    protected override int GetRightLimit()
    {
        List<Vector2> occupiedCells = GetAllOccupiedCellsIndices();

        Vector2 limit = new Vector2(mBlockIndex.x+1f, mBlockIndex.y);
        while (!occupiedCells.Exists((b) => (b.x == limit.x && b.y == limit.y)) && limit.x < mBoardSize.x && limit.x >= 0)
        {

            limit.x++;
        }

        int firstLimit = (int)(limit.x - 2f);

       
        return  firstLimit;

    }

    protected override int GetUpperLimit()
    {
        List<Vector2> occupiedCells = GetAllOccupiedCellsIndices();
        float curIndex = mBlockIndex.y;
        Vector2 limit = new Vector2(mBlockIndex.x, mBlockIndex.y);
        while (!occupiedCells.Exists((b) => (b.x == limit.x && b.y == limit.y)) && limit.y < mBoardSize.y && limit.y >= 0)
        {

            limit.y--;
        }

        int firstLimit=(int)(limit.y + 1f);

         limit = new Vector2(mBlockIndex.x+1, mBlockIndex.y);
        while (!occupiedCells.Exists((b) => (b.x == limit.x && b.y == limit.y)) && limit.y < mBoardSize.y && limit.y >= 0)
        {

            limit.y--;
        }
        int secLimit = (int)(limit.y + 1f);



        return secLimit > firstLimit ? secLimit : firstLimit;
    }

    public override BlockType GetBlockType()
    {
        return BlockType.eHoriziontalBlock;
    }

}

