using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PuzzleSolver
{

    Vector2Int mWinPosition;
    sbyte mWinID;
    int mRowCount;
    int mColCount;


    Hashtable mTileSizes = new Hashtable();
    List<List<sbyte>> mAllIdenticalTiles = new List<List<sbyte>>();
    Hashtable GetTileSizeHashTable(sbyte[,] inBoard)
    {
        Hashtable retval = new Hashtable();
        for (int i = 0; i < mRowCount; i++)
        {
            for (int j = 0; j < mColCount; j++)
            {
                if (inBoard[i, j] == 0 || retval.ContainsKey(inBoard[i, j]))
                {
                    continue;
                }
                else
                {
                    sbyte tileId = inBoard[i, j];
                    Vector2Int size = GetRowColLengthOfTile(inBoard, tileId);
                    retval.Add(tileId, size);
                }
            }
        }
        return retval;
    }

    Vector2Int GetRowColLengthOfTile(sbyte[,] inBoard, sbyte tileId)
    {
        Vector2Int retval = Vector2Int.zero;
        Vector2Int startIndices = Vector2Int.zero;
        Vector2Int endIndices = Vector2Int.zero;
        bool found = false;
        for (int i = 0; i < mRowCount; i++)
        {
            for (int j = 0; j < mColCount; j++)
            {

                if (inBoard[i, j] == tileId)
                {
                    if (found == false)
                    {
                        startIndices = new Vector2Int(i, j);
                        found = true;
                    }

                    for (int qj = j; qj < mColCount; qj++)
                    {
                        if (inBoard[i, qj] == tileId)
                            endIndices = new Vector2Int(i, qj);
                        else
                            break;
                    }

                    for (int qi = i; qi < mRowCount; qi++)
                    {
                        if (inBoard[qi, j] == tileId)
                            endIndices.x = qi;
                        else
                            break;
                    }

                    i = mRowCount;
                    break;
                }

            }
        }
        return endIndices - startIndices + new Vector2Int(1, 1);


    }

    List<List<sbyte>> GenerateAllIdenticalTiles(sbyte[,] inBoard)
    {
        List<List<sbyte>> retval = new List<List<sbyte>>();
        for (int i = 0; i < mRowCount; i++)
        {
            for (int j = 0; j < mColCount; j++)
            {
                sbyte tileId = inBoard[i, j];
                if (tileId == 0)
                {
                    continue;
                }
                else
                {
                    bool alreadyExist = false;
                    for (int h = 0; h < retval.Count; h++)
                    {
                        if (retval[h].Contains(tileId))
                        {
                            alreadyExist = true;
                            break;
                        }
                    }

                    if (!alreadyExist)
                        retval.Add(GetIdenticalTiles(inBoard, tileId));
                }
            }

        }

        return retval;
    }

    List<sbyte> GetIdenticalTiles(sbyte[,] inBoard, sbyte tileId)
    {
        List<sbyte> retval = new List<sbyte>();
        Vector2Int size = (Vector2Int)mTileSizes[tileId];
        retval.Add(tileId);
        for (int i = 0; i < mRowCount; i++)
        {
            for (int j = 0; j < mColCount; j++)
            {
                if (inBoard[i, j] == tileId || inBoard[i, j] == 0 || inBoard[i, j] == mWinID)
                {
                    continue;
                }

                Vector2Int cursize = (Vector2Int)mTileSizes[inBoard[i, j]];
                if (size == cursize)
                {
                    retval.Add(inBoard[i, j]);
                }


            }
        }

        return retval;
    }


    sbyte[,] GetSimpleBoard(sbyte[,] board)
    {
        sbyte[,] retval = new sbyte[mRowCount, mColCount];


        for (int i = 0; i < mRowCount; i++)
        {
            for (int j = 0; j < mColCount; j++)
            {
                int simpleId = 0;
                if (board[i, j] != 0)
                {
                    simpleId = mAllIdenticalTiles.FindIndex(x => x.Contains(board[i, j]));
                    simpleId += 1;
                }

                retval[i, j] = (sbyte)simpleId;
            }
        }

        return retval;
    }

    void PrintIdenticalTiles()
    {
        string s = "";
        for (int i = 0; i < mAllIdenticalTiles.Count; i++)
        {
            s += i + ": ";
            for (int j = 0; j < mAllIdenticalTiles[i].Count; j++)
            {
                s += mAllIdenticalTiles[i][j] + " , ";
            }
            s += "\n";
        }

        Debug.LogWarning(s);
    }
    void PrintHashTable()
    {
        string s = "hash table ";
        foreach (DictionaryEntry entry in mTileSizes)
        {
            s+= entry.Key+" " +entry.Value+"\n";
        }
        Debug.LogWarning(s);
    }

    sbyte[,] GetBoardStateInterchanging(sbyte[,] inBoard, sbyte id1, sbyte id2)
    {
        sbyte[,] retval = new sbyte[mRowCount, mColCount];
        for (int i = 0; i < mRowCount; i++)
        {
            for (int j = 0; j < mColCount; j++)
            {
                if (inBoard[i, j] != id1 && inBoard[i, j] != id2)
                {
                    retval[i, j] = inBoard[i, j];
                }
                else
                {
                    retval[i, j] = inBoard[i, j] == id1 ? id2 : id1;
                }
            }
        }

        return retval;
    }
    public PuzzleSolver(int rowCount, int colCount, sbyte winId, Vector2Int inWinPos, sbyte[,] inBoard)
    {
        mRowCount = rowCount;
        mColCount = colCount;
        mWinID = winId;
        mWinPosition = inWinPos;


        mTileSizes = GetTileSizeHashTable(inBoard);
        
        mAllIdenticalTiles = GenerateAllIdenticalTiles(inBoard);
        
        mAllPossibleBoardStates = new List<ByteBoardState>();
        mAllPossibleBoardStates.Add(new ByteBoardState(-1, rowCount, colCount, inBoard, GetSimpleBoard(inBoard), 0, Direction.eNone));

        for (int i = 0; i < mAllPossibleBoardStates.Count; i++)
        {
            

            bool val = CreateNextPossibleStates(mAllPossibleBoardStates[i].Board, i);

            if (val)
            {
                GetSolution();
            //    PrintSolutionSequence();
                Debug.LogWarning("winnning id " + mWinID);
                Debug.LogWarning("goal pos " + mWinPosition);
                break;
            }
        }
    }

    void GetSolution()
    {

        List<ByteBoardState> seq = new List<ByteBoardState>();
        int parentIndex = mAllPossibleBoardStates.Count - 1;
        do
        {
            seq.Add(mAllPossibleBoardStates[parentIndex]);
            parentIndex = mAllPossibleBoardStates[parentIndex].ParentIndex;
        } while (parentIndex != -1);

        seq.Reverse();



        for (int i = seq.Count - 1; i > 0; i--)
        {
            if (seq[i].Direction == seq[i - 1].Direction && seq[i].BlockIdMoved == seq[i - 1].BlockIdMoved)
            {
                seq[i].UnitStepMoved++;
                seq.RemoveAt(i - 1);


            }

        }
        mSolutionSequence = seq;
    }

    public List<ByteBoardState> GetSolutionSequence()
    {

        return mSolutionSequence;

    }

    List<ByteBoardState> mSolutionSequence = new List<ByteBoardState>();
    void PrintSolutionSequence()
    {
        List<ByteBoardState> seq = GetSolutionSequence();
        for (int i = 0; i < seq.Count; i++)
        {
            PrintState(seq[i].Board);
        }
    }

    void PrintAllStates()
    {


        var seq = (from x in mAllPossibleBoardStates select x.Board).ToList();
        var seqSimple = (from x in mAllPossibleBoardStates select x.SimpleBoard).ToList();
        for (int i = 0; i < seq.Count; i++)
        {
            PrintState(seq[i]);
            PrintState(seqSimple[i], true);
        }
    }


    void PrintState(sbyte[,] board, bool isSimple = false)
    {

        string s = "";
        for (int i = 0; i < mRowCount; i++)
        {
            for (int j = 0; j < mColCount; j++)
            {
                s += board[i, j].ToString() + ",";

            }
            s += "\n";
        }
        if (isSimple)
            Debug.LogWarning(s);
        else
            Debug.Log(s);
    }

    List<ByteBoardState> mAllPossibleBoardStates = new List<ByteBoardState>();

    public static bool IsBoardStateAreIdentical(sbyte[,] board1, sbyte[,] board2, int inTotalRow, int inTotalCol)
    {
        for (int i = 0; i < inTotalRow; i++)
        {
            for (int j = 0; j < inTotalCol; j++)
            {
                if (board1[i, j] != board2[i, j])
                {
                    return false;
                }
            }
        }




        return true;
    }
    bool IsBoardStateAreIdentical(sbyte[,] board1, sbyte[,] board2)
    {
        return IsBoardStateAreIdentical(board1, board2, mRowCount, mColCount);
    }

    bool IsBoardStateAlreadyExist(sbyte[,] board, sbyte[,] simpleBoard, sbyte inMovedId)
    {

        for (int i = mAllPossibleBoardStates.Count - 1; i >= 0; i--)
        {
            if (IsBoardStateAreIdentical(mAllPossibleBoardStates[i].Board, board) || IsBoardStateAreIdentical(mAllPossibleBoardStates[i].SimpleBoard, simpleBoard))
            {
                return true;
            }
        }



        bool retval = IsMirroredBoardStateAlreadyExist(board, simpleBoard, inMovedId);

        return retval;
    }

    sbyte[,] GetMirroredBoardState(sbyte[,] board)
    {
        sbyte[,] retval = new sbyte[mRowCount, mColCount];
        for (int i = 0; i < mRowCount; i++)
        {
            for (int j = 0; j < mColCount; j++)
            {
                retval[i, mColCount - (j + 1)] = board[i, j];
            }
        }

        return retval;
    }


    bool IsMirroredBoardStateAlreadyExist(sbyte[,] inboard, sbyte[,] insimpleboard, sbyte inMovedId)
    {
        sbyte[,] board = GetMirroredBoardState(inboard);
        sbyte[,] simpleboard = GetMirroredBoardState(insimpleboard);

        for (int i = mAllPossibleBoardStates.Count - 1; i >= 0; i--)
        {
            if (IsBoardStateAreIdentical(mAllPossibleBoardStates[i].Board, board) || IsBoardStateAreIdentical(mAllPossibleBoardStates[i].SimpleBoard, simpleboard))
            {
                return true;
            }
        }
        return false;
    }


    bool IsBoardComplete(sbyte[,] board)
    {
        bool retval = false;

        retval = board[mWinPosition.x, mWinPosition.y] == mWinID;

        retval = retval && board[mWinPosition.x, mWinPosition.y - 1] != mWinID;

        return retval;
    }


    bool CanBlockMoveLeftDirection(sbyte[,] board, int blckid)
    {
        bool retval = true;
        for (int i = 0; i < mRowCount; i++)
        {
            for (int j = 0; j < mColCount; j++)
            {
                if (board[i, j] == blckid)
                {
                    if (j - 1 < 0)
                    {
                        retval = false;
                        i = mRowCount;
                        break;
                    }
                    else
                    {
                        if (!(board[i, j - 1] == blckid || board[i, j - 1] == 0))
                        {
                            retval = false;
                            i = mRowCount;
                            break;
                        }
                    }
                }
            }
        }
        return retval;
    }


    bool CanBlockMoveDownwardDirection(sbyte[,] board, int blckid)
    {
        bool retval = true;
        for (int j = 0; j < mColCount; j++)
        {
            for (int i = mRowCount - 1; i >= 0; i--)
            {
                if (board[i, j] == blckid)
                {
                    if (i + 1 >= mRowCount)
                    {
                        retval = false;
                        j = mColCount;
                        break;
                    }
                    else
                    {
                        if (!(board[i + 1, j] == blckid || board[i + 1, j] == 0))
                        {
                            retval = false;
                            j = mColCount;
                            break;
                        }
                    }
                }
            }
        }
        return retval;
    }





    bool CanBlockMoveUpwardDirection(sbyte[,] board, int blckid)
    {
        bool retval = true;
        for (int j = 0; j < mColCount; j++)
        {
            for (int i = 0; i < mRowCount; i++)
            {
                if (board[i, j] == blckid)
                {
                    if (i - 1 < 0)
                    {
                        retval = false;
                        j = mColCount;
                        break;
                    }
                    else
                    {
                        if (!(board[i - 1, j] == blckid || board[i - 1, j] == 0))
                        {
                            retval = false;
                            j = mColCount;
                            break;
                        }
                    }
                }
            }
        }
        return retval;
    }



    bool CanBlockMoveRightDirection(sbyte[,] board, int blckid)
    {
        bool retval = true;
        for (int i = 0; i < mRowCount; i++)
        {
            for (int j = mColCount - 1; j >= 0; j--)
            {
                if (board[i, j] == blckid)
                {
                    if (j + 1 >= mColCount)
                    {
                        retval = false;
                        i = mRowCount;
                        break;
                    }
                    else
                    {
                        if (!(board[i, j + 1] == blckid || board[i, j + 1] == 0))
                        {
                            retval = false;
                            i = mRowCount;
                            break;
                        }
                    }
                }
            }
        }
        return retval;
    }

    sbyte[,] MoveBlockUpwardDirection(sbyte[,] board, sbyte blckid)
    {
        sbyte[,] retval = new sbyte[mRowCount, mColCount];
        for (int i = 0; i < mRowCount; i++)
        {
            for (int j = 0; j < mColCount; j++)
            {
                retval[i, j] = board[i, j];
            }
        }

        for (int j = mColCount - 1; j >= 0; j--)

        {
            for (int i = 0; i < mRowCount; i++)
            {
                if (retval[i, j] == blckid)
                {
                    retval[i - 1, j] = blckid;
                    retval[i, j] = 0;
                }
            }
        }
        return retval;
    }

    sbyte[,] MoveBlockDownwardDirection(sbyte[,] board, sbyte blckid)
    {
        sbyte[,] retval = new sbyte[mRowCount, mColCount];

        for (int i = 0; i < mRowCount; i++)
        {
            for (int j = 0; j < mColCount; j++)
            {
                retval[i, j] = board[i, j];
            }
        }
        for (int j = 0; j < mColCount; j++)
        {
            for (int i = mRowCount - 1; i >= 0; i--)
            {
                if (retval[i, j] == blckid)
                {
                    retval[i + 1, j] = blckid;
                    retval[i, j] = 0;
                }
            }
        }
        return retval;
    }

    sbyte[,] MoveBlockRightDirection(sbyte[,] board, sbyte blckid)
    {
        sbyte[,] retval = new sbyte[mRowCount, mColCount];

        for (int i = 0; i < mRowCount; i++)
        {
            for (int j = 0; j < mColCount; j++)
            {
                retval[i, j] = board[i, j];
            }
        }

        for (int i = 0; i < mRowCount; i++)
        {
            for (int j = mColCount - 1; j >= 0; j--)
            {
                if (retval[i, j] == blckid)
                {
                    retval[i, j + 1] = blckid;
                    retval[i, j] = 0;
                }
            }
        }

        return retval;

    }

    sbyte[,] MoveBlockLeftDirection(sbyte[,] board, sbyte blckid)
    {
        sbyte[,] retval = new sbyte[mRowCount, mColCount];

        for (int i = 0; i < mRowCount; i++)
        {
            for (int j = 0; j < mColCount; j++)
            {
                retval[i, j] = board[i, j];
            }
        }

        for (int i = 0; i < mRowCount; i++)
        {
            for (int j = 0; j < mColCount; j++)
            {
                if (retval[i, j] == blckid)
                {
                    retval[i, j - 1] = blckid;
                    retval[i, j] = 0;
                }
            }
        }

        return retval;

    }



    bool CreateNextPossibleStates(sbyte[,] board, int parentIndex)
    {
        bool retval = false;
        List<sbyte> idsMoved = new List<sbyte>();

        for (int i = 0; i < mRowCount; i++)
        {
            for (int j = 0; j < mColCount; j++)
            {
                if (board[i, j] == 0)
                {
                    continue;
                }

                if (!idsMoved.Contains(board[i, j]))
                {

                    idsMoved.Add(board[i, j]);


                    if (CanBlockMoveLeftDirection(board, board[i, j]))
                    {
                        sbyte[,] nxtState = MoveBlockLeftDirection(board, board[i, j]);
                        sbyte[,] nxtSimpleState = GetSimpleBoard(nxtState);
                        if (IsBoardComplete(nxtState))
                        {

                            ByteBoardState pState = new ByteBoardState(parentIndex, mRowCount, mColCount, nxtState, nxtSimpleState, board[i, j], Direction.eLeft);
                            mAllPossibleBoardStates.Add(pState);
                            return true;
                        }
                        else if (!IsBoardStateAlreadyExist(nxtState, nxtSimpleState, board[i, j]))
                        {

                            ByteBoardState pState = new ByteBoardState(parentIndex, mRowCount, mColCount, nxtState, nxtSimpleState, board[i, j], Direction.eLeft);
                            mAllPossibleBoardStates.Add(pState);
                        }
                    }

                    if (CanBlockMoveRightDirection(board, board[i, j]))
                    {


                        sbyte[,] nxtState = MoveBlockRightDirection(board, board[i, j]);
                        sbyte[,] nxtSimpleState = GetSimpleBoard(nxtState);
                        if (IsBoardComplete(nxtState))
                        {

                            ByteBoardState pState = new ByteBoardState(parentIndex, mRowCount, mColCount, nxtState, nxtSimpleState, board[i, j], Direction.eRight);
                            mAllPossibleBoardStates.Add(pState);
                            return true;
                        }
                        else if (!IsBoardStateAlreadyExist(nxtState, nxtSimpleState, board[i, j]))
                        {

                            ByteBoardState pState = new ByteBoardState(parentIndex, mRowCount, mColCount, nxtState, nxtSimpleState, board[i, j], Direction.eRight);
                            mAllPossibleBoardStates.Add(pState);
                        }
                    }

                    if (CanBlockMoveUpwardDirection(board, board[i, j]))
                    {
                        sbyte[,] nxtState = MoveBlockUpwardDirection(board, board[i, j]); sbyte[,] nxtSimpleState = GetSimpleBoard(nxtState);
                        if (IsBoardComplete(nxtState))
                        {
                            ByteBoardState pState = new ByteBoardState(parentIndex, mRowCount, mColCount, nxtState, nxtSimpleState, board[i, j], Direction.eUp);
                            mAllPossibleBoardStates.Add(pState);
                            return true;
                        }
                        else if (!IsBoardStateAlreadyExist(nxtState, nxtSimpleState, board[i, j]))
                        {

                            ByteBoardState pState = new ByteBoardState(parentIndex, mRowCount, mColCount, nxtState, nxtSimpleState, board[i, j], Direction.eUp);
                            mAllPossibleBoardStates.Add(pState);
                        }
                    }
                    if (CanBlockMoveDownwardDirection(board, board[i, j]))
                    {
                        sbyte[,] nxtState = MoveBlockDownwardDirection(board, board[i, j]); sbyte[,] nxtSimpleState = GetSimpleBoard(nxtState);
                        if (IsBoardComplete(nxtState))
                        {
                            ByteBoardState pState = new ByteBoardState(parentIndex, mRowCount, mColCount, nxtState, nxtSimpleState, board[i, j], Direction.eDown);
                            mAllPossibleBoardStates.Add(pState);
                            return true;
                        }
                        else if (!IsBoardStateAlreadyExist(nxtState, nxtSimpleState, board[i, j]))
                        {

                            ByteBoardState pState = new ByteBoardState(parentIndex, mRowCount, mColCount, nxtState, nxtSimpleState, board[i, j], Direction.eDown);
                            mAllPossibleBoardStates.Add(pState);
                        }
                    }

                }



            }
        }

        return retval;
    }

    public enum Direction
    {
        eNone = 0,
        eLeft,
        eRight,
        eUp,
        eDown
    }

    public class ByteBoardState
    {
        sbyte mBlockIdMoved = 0;
        Direction mDirection = Direction.eNone;
        int mParentIndex = -1;
        sbyte[,] mBoard;
        sbyte[,] mSimpleBoard;
        sbyte mUnitStepMoved = 0;
        public sbyte UnitStepMoved
        {
            get
            {
                return mUnitStepMoved;
            }
            set
            {
                mUnitStepMoved = value;
            }
        }
        public sbyte BlockIdMoved
        {
            get
            {
                return mBlockIdMoved;
            }
        }

        public Direction Direction
        {
            get
            {
                return mDirection;
            }
        }


        public int ParentIndex
        {
            get
            {
                return mParentIndex;
            }
        }
        public sbyte[,] Board
        {
            get
            {
                return mBoard;
            }
        }

        public sbyte[,] SimpleBoard
        {
            get
            {
                return mSimpleBoard;
            }
        }


        public ByteBoardState(int inParentIndex, int inRowCount, int inColCount, sbyte[,] inBoard, sbyte[,] inSimpleBoard, sbyte inBlockIdmoved, Direction inDirectionMoved, sbyte inStep = 1)
        {
            mUnitStepMoved = inStep;
            mDirection = inDirectionMoved;
            mBlockIdMoved = inBlockIdmoved;
            mBoard = new sbyte[inRowCount, inColCount];
            mSimpleBoard = new sbyte[inRowCount, inColCount];
            mParentIndex = inParentIndex;
            for (int i = 0; i < inRowCount; i++)
                for (int j = 0; j < inColCount; j++)
                {
                    mBoard[i, j] = inBoard[i, j];
                    mSimpleBoard[i, j] = inSimpleBoard[i, j];
                }

        }
    }

}
