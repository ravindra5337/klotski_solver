using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelConfig {

    [JsonProperty("lvl")]
    int mLevel;

    [JsonProperty("row")]
    int mTotalRows;

    [JsonProperty("col")]
    int mTotalColumns;

    [JsonProperty("blocks")]
    BlockProperty[] mBlocks;

    public LevelConfig (int totalRows,int totalCol,int level,BlockProperty[] inBlocks)
    {
        mTotalColumns = totalCol;
        mTotalRows = totalRows;
        mBlocks = inBlocks;
        mLevel = level;
    }
    [JsonIgnore]
    public int Level { get { return mLevel; } }

    [JsonIgnore]
    public int TotalRows { get { return mTotalRows; } }

    [JsonIgnore]
    public int TotalColumns { get { return mTotalColumns; } }
    [JsonIgnore]

    public BlockProperty[] Blocks { get { return mBlocks; } }


}

public enum BlockType
{
    eNone=0,
    eSingleSquare=1,
    eVeriticalBlock=2,
    eHoriziontalBlock=3,
    eBigSquare=4
}

public class CoOrdinate
{
    [JsonProperty("x")]
    int mX;
    [JsonProperty("y")]
    int mY;

    [JsonIgnore]
    public int X { get { return mX; } }
    [JsonIgnore]
    public int Y { get { return mY; } }


    public CoOrdinate(Vector2Int inD)
    {
        mX = inD.x;
        mY = inD.y;
    }
}

public class BlockProperty
{
    [JsonProperty("pos")]
    CoOrdinate mPosition;

    [JsonProperty("type")]
    BlockType mBlockType;

    [JsonProperty("is_goal")]
    bool mIsSolutionBlock;

    [JsonIgnore]
    public Vector2Int Position { get { return new Vector2Int(mPosition.X,mPosition.Y); } }

    [JsonIgnore]
    public BlockType CurrentBlockType { get { return mBlockType; } }

    [JsonIgnore]
    public bool IsSolutionBlock { get { return mIsSolutionBlock; } }

    public BlockProperty(Vector2Int inPos,BlockType inBlockType,bool isSolutionBlock)
    {
        mPosition =new CoOrdinate( inPos);
        mBlockType = inBlockType;
        mIsSolutionBlock = isSolutionBlock;
    }
}


public class GameLevels
{
    [JsonProperty("levels")]
    LevelConfig[] mLevels;
    [JsonIgnore]
    public LevelConfig[] Levels { get { return mLevels; } }


    public LevelConfig GetLevel(int levelNo)
    {
        for(int i = 0; i < mLevels.Length; i++)
        {
            if (mLevels[i].Level == levelNo)
                return mLevels[i];
        }

        return null;
    }
}