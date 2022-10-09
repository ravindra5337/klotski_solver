using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PuzzleSolver;

[System.Serializable]
public class BlockPrefab
{
    public BlockType m_Type;
    public GameObject m_Block;
}


[System.Serializable]
public class HintIndicatorPrefab
{
    public Direction m_Type;
    public GameObject m_Block;
}


public class GridGenerator : MonoBehaviour
{

    [SerializeField]
    RectTransform mBottomPanel;

    public TouchDetection mTouchDetection;
    public GameObject pUnit;
    public GameObject pGridBackground;
    public GameObject pScreenBackground;
    List<Block> mBlocks = new List<Block>();


    [SerializeField]
    HintIndicatorPrefab[] p_HintIndicatorPrefabs;

    [SerializeField]
    BlockPrefab[] p_BlockPrefabs;
    [SerializeField]
    BlockPrefab[] p_BlockHintPrefabs;

    Vector3[,] mGridPositions;

    LevelConfig mCurrentLevel;

    void AdjustBottomPanel(float inTotalHeight, float inGridHeight)
    {
        float bottomSpace = ((inTotalHeight - inGridHeight) / inTotalHeight) * 0.5f;
        mBottomPanel.anchorMax = new Vector2(1f, bottomSpace);
    }

    GameObject GetBlockPrefab(BlockType inType)
    {
        for (int i = 0; i < p_BlockPrefabs.Length; i++)
        {
            if (p_BlockPrefabs[i].m_Type == inType)
            {
                return p_BlockPrefabs[i].m_Block;
            }
        }

        return null;
    }


    GameObject GetBlockHintPrefab(BlockType inType)
    {
        for (int i = 0; i < p_BlockHintPrefabs.Length; i++)
        {
            if (p_BlockHintPrefabs[i].m_Type == inType)
            {
                return p_BlockHintPrefabs[i].m_Block;
            }
        }

        return null;
    }

    GameObject GetHintIndicatorPrefab(Direction inType)
    {
        for (int i = 0; i < p_HintIndicatorPrefabs.Length; i++)
        {
            if (p_HintIndicatorPrefabs[i].m_Type == inType)
            {
                return p_HintIndicatorPrefabs[i].m_Block;
            }
        }

        return null;
    }

   

    public void StartGame(int level)
    {
        mTouchDetection.OnDrag += OnDrag;
        mTouchDetection.OnTouch += OnTouch;
        mTouchDetection.OnRelease += OnRelease;

        mCurrentLevel = GameConfigLoader.Instance.GetLevel(level);
        LoadLevel(level);
    }

    Block GetBlockAtIndex(int x, int y)
    {
        Block retval = null;
        if (x < mTotalColumns && y < mTotalRows && x >= 0 && y >= 0)
            for (int i = 0; i < mBlocks.Count; i++)
            {
                List<Vector2> occIndex = mBlocks[i].GetBlockOccupiedIndex();

                for (int j = 0; j < occIndex.Count; j++)
                {
                    if (occIndex[j].x == x && occIndex[j].y == y)
                    {
                        retval = mBlocks[i];
                        i = mBlocks.Count;
                        break;
                    }
                }


            }



        return retval;
    }

    public void OnClickPreviousLevel()
    {
        int prevLevel = mCurrentLevel.Level - 1;

        LoadLevel(prevLevel);
    }
    public void OnClickNextLevel()
    {
        int nxtLevel = mCurrentLevel.Level + 1;

        LoadLevel(nxtLevel);
    }

    void LoadLevel(int inLevelNo)
    {
        mCurrentLevel = GameConfigLoader.Instance.GetLevel(inLevelNo);
        DeleteCurrentGrid();
        CreateGridBG();
        CreateGrid();
        CreateBlock();
    }
    List<GameObject> mBlockerHint = new List<GameObject>();
    List<GameObject> mHintIndicators = new List<GameObject>();

    void CreateBlock()
    {
        Vector3 scale = Vector3.one * mSizeOfEachBox;

        for (int i = 0; i < mCurrentLevel.Blocks.Length; i++)
        {
            GameObject box = Instantiate<GameObject>(GetBlockPrefab(mCurrentLevel.Blocks[i].CurrentBlockType), Vector3.zero, Quaternion.identity);
            box.transform.localScale = scale;
            box.transform.position = mGridPositions[mCurrentLevel.Blocks[i].Position.x, mCurrentLevel.Blocks[i].Position.y];

            box.GetComponent<Block>().Initialize(new Vector2(mCurrentLevel.Blocks[i].Position.y, mCurrentLevel.Blocks[i].Position.x), new Vector2(mTotalColumns, mTotalRows), mGridPositions, mBlocks, mCurrentLevel.Blocks[i].IsSolutionBlock);
            mBlocks.Add(box.GetComponent<Block>());

            box.transform.parent = mCurrentLevelElements.transform;



            GameObject boxHint = Instantiate<GameObject>(GetBlockHintPrefab(mCurrentLevel.Blocks[i].CurrentBlockType), Vector3.zero, Quaternion.identity);
            boxHint.transform.localScale = scale;
            boxHint.transform.position = mGridPositions[mCurrentLevel.Blocks[i].Position.x, mCurrentLevel.Blocks[i].Position.y];

            mBlockerHint.Add(boxHint);

            boxHint.transform.parent = mCurrentLevelElements.transform;

            boxHint.SetActive(false);
        }



        for (int i = 0; i < 5; i++)
        {

            GameObject hintIndicator = Instantiate<GameObject>(GetHintIndicatorPrefab((Direction)i), Vector3.zero, Quaternion.identity);
            hintIndicator.transform.localScale = scale;
            mHintIndicators.Add(hintIndicator);
            hintIndicator.transform.parent = mCurrentLevelElements.transform;

            hintIndicator.SetActive(false);
        }

        mAllPreviousStates.Clear();
        SaveStateForUndo(GetCurrentBoardState());
    }

    public int mTotalColumns;
    public int mTotalRows;

    public float mTopMostX = 0f;
    public float mTopMostY = 0f;


    public void OnClickHint()
    {
        sbyte winId = GetWinId();
        sbyte[,] currentBoard = GetBoardByteState();
        Vector2Int winPos = new Vector2Int(0, 1);
        PuzzleSolver ps = new PuzzleSolver(mTotalRows, mTotalColumns, winId, winPos, currentBoard);
        mSolutionSequence = ps.GetSolutionSequence();
        ShowHint();


    }
    List<ByteBoardState> mSolutionSequence = new List<ByteBoardState>();
    void HideAllHintBlocks()
    {
        for (int i = 0; i < mBlockerHint.Count; i++)
        {
            mBlockerHint[i].SetActive(false);
        }
    }

    void HideAllHintIndicators()
    {
        for (int i = 0; i < mHintIndicators.Count; i++)
        {
            mHintIndicators[i].SetActive(false);
        }
    }

    void ShowHint()
    {
        HideAllHintBlocks();
        HideAllHintIndicators();
        sbyte[,] currentBoard = GetBoardByteState();
        if (mSolutionSequence.Count > 0)
        {
            if (PuzzleSolver.IsBoardStateAreIdentical(mSolutionSequence[0].Board, currentBoard, mTotalRows, mTotalColumns))
            {
                mSolutionSequence.RemoveAt(0);
            }
            if (mSolutionSequence.Count > 0)
            {
                int x = 0;
                int y = 0;
                int steps = mSolutionSequence[0].UnitStepMoved;
                switch (mSolutionSequence[0].Direction)
                {
                    case PuzzleSolver.Direction.eUp:
                        y -= steps;
                        break;

                    case PuzzleSolver.Direction.eDown:
                        y += steps;
                        break;

                    case PuzzleSolver.Direction.eLeft:
                        x -= steps;
                        break;

                    case PuzzleSolver.Direction.eRight:
                        x += steps;
                        break;

                }

                int blockIndex = mSolutionSequence[0].BlockIdMoved - 1;
                Vector2 posIndex = mBlocks[blockIndex].BlockIndex;

                mHintIndicators[(int)mSolutionSequence[0].Direction].transform.position = mBlocks[blockIndex].transform.position;
                mHintIndicators[(int)mSolutionSequence[0].Direction].SetActive(true);
                Debug.LogWarning(posIndex);
                Debug.Log(x + " " + y);
                mBlockerHint[blockIndex].SetActive(true);
                mBlockerHint[blockIndex].transform.position = mGridPositions[(int)(posIndex.y + y), (int)(posIndex.x + x)];

            }
        }
    }

    Block mCurrentBlock;

    void OnDrag(Vector3 pos)
    {
        if (mCurrentBlock != null)
        {

            mCurrentBlock.OnDrag(pos);


        }
    }

    void OnTouch(Vector3 pos)
    {
        float x = pos.x - mTopMostX;
        float y = pos.y - mTopMostY;

        if (pos.y > mTopMostY || pos.x < mTopMostX)
            return;

        float indexX = x / mSizeOfEachBox;
        float indexY = y / mSizeOfEachBox;

        int curX = (int)Mathf.Abs(indexX);
        int curY = (int)Mathf.Abs(indexY);
        mCurrentBlock = GetBlockAtIndex(curX, curY);
        if (mCurrentBlock != null)
            mCurrentBlock.OnTouch(pos);
    }
    void OnRelease(Vector3 pos)
    {
        if (mCurrentBlock != null)
        {
            mCurrentBlock.OnRelease(pos);

            if (mCurrentBlock.IsSolutionBlock())
            {
                if (mCurrentBlock.BlockIndex.x == 1 && mCurrentBlock.BlockIndex.y == 0)
                {
                    mSolutionSequence.Clear();
                    OnClickNextLevel();

                }
                else
                {
                    SaveStateForUndo(GetCurrentBoardState());
                }

            }
            else
            {
                SaveStateForUndo(GetCurrentBoardState());
            }

            ShowHint();
        }

    }

    public void OnClickReset()
    {

        LoadLevel(mCurrentLevel.Level);
    }

    public void OnClickUndo()
    {
        if (mAllPreviousStates.Count >= 2)
        {
            mAllPreviousStates.RemoveAt(mAllPreviousStates.Count - 1);
            Vector2Int[] prevState = mAllPreviousStates[mAllPreviousStates.Count - 1];
            for (int i = 0; i < mBlocks.Count; i++)
            {
                mBlocks[i].PositionBlockByBlockIndex(prevState[i]);
            }
        }
    }

    float mSizeOfEachBox = 0f;

    GameObject mCurrentLevelElements = null;

    void DeleteCurrentGrid()
    {
        Destroy(mCurrentLevelElements);
        mGridPositions = null;
        mBlocks.Clear();
        mBlockerHint.Clear();
        mHintIndicators.Clear();
        mCurrentLevelElements = null;
    }

    void CreateGridBG()
    {
        if (mCurrentLevelElements == null)
            mCurrentLevelElements = new GameObject("current_grid");

        float height = Camera.main.orthographicSize * 2f;
        float width = Camera.main.aspect * height;
        Vector3 scale = new Vector3(width, height);

        GameObject box1 = Instantiate<GameObject>(pScreenBackground, Vector3.zero, Quaternion.identity);
        box1.transform.localScale = scale;

        box1.transform.parent = mCurrentLevelElements.transform;

        width = width * 0.90f;
        height = 0.72f * height;
        scale = new Vector3(width, height);
        GameObject box = Instantiate<GameObject>(pGridBackground, Vector3.zero, Quaternion.identity);
        box.transform.localScale = scale;

        box.transform.parent = mCurrentLevelElements.transform;

    }


    List<Vector2Int[]> mAllPreviousStates = new List<Vector2Int[]>();

    void SaveStateForUndo(Vector2Int[] curState)
    {
        if (mAllPreviousStates.Count == 0 || !IsStatesIdentical(mAllPreviousStates[mAllPreviousStates.Count - 1], curState))
        {
            mAllPreviousStates.Add(curState);
        }
    }

    bool IsStatesIdentical(Vector2Int[] state1, Vector2Int[] state2)
    {
        bool retval = true;
        for (int i = 0; i < state1.Length; i++)
        {
            if ((state1[i].y != state2[i].y) || (state1[i].x != state2[i].x))
            {
                retval = false;
                break;
            }
        }

        return retval;
    }
    Vector2Int[] GetCurrentBoardState()
    {
        Vector2Int[] retval = new Vector2Int[mBlocks.Count];
        for (int i = 0; i < mBlocks.Count; i++)
        {
            retval[i] = new Vector2Int((int)mBlocks[i].BlockIndex.y, (int)mBlocks[i].BlockIndex.x);
        }

        return retval;
    }

    void PrintState(sbyte[,] board)
    {
        string s = "";
        for (int i = 0; i < mTotalRows; i++)
        {
            for (int j = 0; j < mTotalColumns; j++)
            {
                s += board[i, j].ToString() + ",";
            }
            s += "\n";
        }

        Debug.Log(s);
    }

    sbyte[,] GetBoardByteState()
    {
        sbyte[,] board = new sbyte[mTotalRows, mTotalColumns];
        for (int i = 0; i < mBlocks.Count; i++)
        {
            List<Vector2> occ = mBlocks[i].GetBlockOccupiedIndex();
            for (int j = 0; j < occ.Count; j++)
            {
                board[(int)occ[j].y, (int)occ[j].x] = (sbyte)(i + 1);
            }
        }

        return board;
    }

    sbyte GetWinId()
    {

        sbyte winId = -1;
        for (int i = 0; i < mBlocks.Count; i++)
        {
            if (mBlocks[i].IsSolutionBlock())
            {
                winId = (sbyte)(i + 1);
                break;
            }

        }

        return winId;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            OnClickHint();
        }
    }



    void CreateGrid()
    {
        if (mCurrentLevelElements == null)
            mCurrentLevelElements = new GameObject("current_grid");

        mTotalColumns = mCurrentLevel.TotalColumns;
        mTotalRows = mCurrentLevel.TotalRows;

        float height = Camera.main.orthographicSize * 2f;
        float totalHeight = height;
        float width = Camera.main.aspect * height;
        width = width * 0.90f;
        height = 0.72f * height;
        float sizeOfEachBox = width / mTotalColumns;
        sizeOfEachBox = (height / mTotalRows) < sizeOfEachBox ? (height / mTotalRows) : sizeOfEachBox;

        mSizeOfEachBox = sizeOfEachBox;

        AdjustBottomPanel(totalHeight, mSizeOfEachBox * mTotalRows);
        float startx = sizeOfEachBox * mTotalColumns * 0.5f;
        startx -= sizeOfEachBox * 0.5f;
        startx = -1f * startx;

        mTopMostX = startx;
        mTopMostX -= sizeOfEachBox * 0.5f;

        float starty = sizeOfEachBox * mTotalRows * 0.5f;
        starty -= sizeOfEachBox * 0.5f;

        mTopMostY = starty;
        mTopMostY += sizeOfEachBox * 0.5f;

        mGridPositions = new Vector3[mTotalRows, mTotalColumns];
        Vector3 scale = Vector3.one * sizeOfEachBox;
        for (int i = 0; i < mTotalRows; i++)
        {
            startx = sizeOfEachBox * mTotalColumns * 0.5f;
            startx -= sizeOfEachBox * 0.5f;
            startx = -1f * startx;

            for (int j = 0; j < mTotalColumns; j++)
            {
                GameObject box = Instantiate<GameObject>(pUnit, Vector3.zero, Quaternion.identity);
                box.transform.localScale = scale;
                box.transform.position = new Vector3(startx, starty, 0f);
                mGridPositions[i, j] = new Vector3(startx, starty, 0f);
                startx += sizeOfEachBox;
                box.transform.parent = mCurrentLevelElements.transform;
            }
            starty -= sizeOfEachBox;
        }
    }


}
