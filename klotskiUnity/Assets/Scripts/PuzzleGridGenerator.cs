using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class PuzzleGridGenerator : MonoBehaviour
{
    public TouchDetection mTouchDetection;
    public GameObject pUnit;
    
    List<Block> mBlocks = new List<Block>();


    [SerializeField]
    BlockPrefab[] p_BlockPrefabs;

    Vector3[,] mGridPositions;
    

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

    // Start is called before the first frame update
    void Start()
    {
        mTouchDetection.OnDrag += OnDrag;
        mTouchDetection.OnTouch += OnTouch;
        mTouchDetection.OnRelease += OnRelease;
        
        CreateGrid();
       
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

    public void OnClickGenerateConfig()
    {
        List<BlockProperty> blc = new List<BlockProperty>();
        for(int i = 0; i < mBlocks.Count; i++)
        {
            blc.Add(mBlocks[i].GetBlockProperty());
        }


        LevelConfig nwConfig = new LevelConfig(mTotalRows, mTotalColumns, mCurrentLevel++, blc.ToArray());

        string data = JsonConvert.SerializeObject(nwConfig);
        string path = Application.persistentDataPath + "/genLevel.txt";
        Debug.Log(path);
        File.AppendAllText(path, data);


        sbyte[,] board = new sbyte[5, 4];
        for (int i = 0; i < 5; i++)
            for (int j = 0; j < 4; j++)
                board[i, j] = (sbyte)(i * 100 + j);
        string data1 = JsonConvert.SerializeObject(board);
        string path1 = Application.persistentDataPath + "/genLevel11111.txt";
        Debug.Log(path1);
        File.AppendAllText(path1, data1);

    }

    void CreateBlock(BlockType inType, int x,int y)
    {
        
        Vector3 scale = Vector3.one * mSizeOfEachBox;
         GameObject box = Instantiate<GameObject>(GetBlockPrefab(inType), Vector3.zero, Quaternion.identity);
            box.transform.localScale = scale;
            box.transform.position = mGridPositions[y, x];

            box.GetComponent<Block>().Initialize(new Vector2(x, y), new Vector2(mTotalColumns, mTotalRows), mGridPositions, mBlocks,false);
            mBlocks.Add(box.GetComponent<Block>());
        
      

    }

    public int mTotalColumns;
    public int mTotalRows;

    public float mTopMostX = 0f;
    public float mTopMostY = 0f;



    Block mCurrentBlock;

    void OnDrag(Vector3 pos)
    {
      
    }

    void OnTouch(Vector3 pos)
    {
        
    }

    public int mCurrentLevel;
    int mCurrentBlockTypeToCreate;
    void OnRelease(Vector3 pos)
    {
        float x = pos.x - mTopMostX;
        float y = pos.y - mTopMostY;

        if (pos.y > mTopMostY || pos.x < mTopMostX)
            return;

        float indexX = x / mSizeOfEachBox;
        float indexY = y / mSizeOfEachBox;

        int curX = (int)Mathf.Abs(indexX);
        int curY = (int)Mathf.Abs(indexY);
        
        Block block = GetBlockAtIndex(curX, curY);

        Debug.Log(curX + "  " + curY);

        if(curX<mTotalColumns && curY<mTotalRows && curX>=0 && curY>=0)
        if (block == null)
        {
            CreateBlock(UpdateCurrentBlockToCreate(), curX, curY);
        }
        else
        {
            mBlocks.Remove(block);
            Destroy(block.gameObject);

        }
    }

    BlockType UpdateCurrentBlockToCreate()
    {
        List<BlockType> allTypes = new List<BlockType>((BlockType[])Enum.GetValues(typeof(BlockType)));
        mCurrentBlockTypeToCreate++;
        mCurrentBlockTypeToCreate = mCurrentBlockTypeToCreate % allTypes.Count;
        if (allTypes[mCurrentBlockTypeToCreate] == BlockType.eNone)
        {
            mCurrentBlockTypeToCreate++;
            mCurrentBlockTypeToCreate = mCurrentBlockTypeToCreate % allTypes.Count;
        }

        return allTypes[mCurrentBlockTypeToCreate];
    }

    float mSizeOfEachBox = 0f;
    void CreateGrid()
    {

        float height = Camera.main.orthographicSize * 2f;
        float width = Camera.main.aspect * height;

        float sizeOfEachBox = width / mTotalColumns;
        sizeOfEachBox = (height / mTotalRows) < sizeOfEachBox ? (height / mTotalRows) : sizeOfEachBox;

        mSizeOfEachBox = sizeOfEachBox;
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

            }
            starty -= sizeOfEachBox;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
