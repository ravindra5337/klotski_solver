using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareBlock : Block
{
    public override List<Vector2> GetBlockOccupiedIndex()
    {
        return new List<Vector2>() { mBlockIndex };
    }

    public override BlockType GetBlockType()
    {
        return BlockType.eSingleSquare;
    }

 

    protected override int GetLeftLimit()
    {
        List<Vector2> occupiedCells = GetAllOccupiedCellsIndices();

        Vector2 limit = new Vector2(mBlockIndex.x, mBlockIndex.y);
        while (!occupiedCells.Exists((b) => (b.x == limit.x && b.y == limit.y)) && limit.x < mBoardSize.x && limit.x >= 0)
        {

            limit.x--;
        }

        return (int)(limit.x + 1f);

    }

    protected override int GetLowerLimit()
    {
        List<Vector2> occupiedCells = GetAllOccupiedCellsIndices();
        float curIndex = mBlockIndex.y;
        Vector2 limit = new Vector2(mBlockIndex.x, mBlockIndex.y);
        while (!occupiedCells.Exists((b) => (b.x == limit.x && b.y == limit.y)) && limit.y < mBoardSize.y && limit.y >= 0)
        {

            limit.y++;
        }

        return (int)(limit.y - 1f);
    }

    protected override int GetRightLimit()
    {
        List<Vector2> occupiedCells = GetAllOccupiedCellsIndices();

        Vector2 limit = new Vector2(mBlockIndex.x, mBlockIndex.y);
        while (!occupiedCells.Exists((b) => (b.x == limit.x && b.y == limit.y)) && limit.x < mBoardSize.x && limit.x >= 0)
        {

            limit.x++;
        }

        return (int)(limit.x - 1f);
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

        return (int)(limit.y + 1f);
    }
}
