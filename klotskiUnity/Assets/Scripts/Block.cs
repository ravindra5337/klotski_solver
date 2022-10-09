using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Block : MonoBehaviour
{
    [SerializeField]
    protected bool mIsSolutionBlock;
    protected Vector3 mPrevTouchPosition;
    protected float mLeftLimit = 0f;
    protected float mRightLimit = 0f;
    protected float mLowerLimit = 0f;
    protected float mUpperLimit = 0f;
    protected float mSensitivityThreshold = 0.1f;
    protected Vector2 mBlockIndex;
    protected Vector2 mBoardSize;
    protected Vector3[,] mGridPositions;
    protected Vector3 mCurrentValidPos;
    Direction mMovementDirection = Direction.eNone;

    public Vector2 BlockIndex
    {
        get
        {
            return mBlockIndex;
        }
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    enum Direction
    {
        eNone,
        eHorizontal,
        eVertical
    }

    public void Initialize(Vector2 inBlockIndex, Vector2 inBoardSize, Vector3[,] inGridPositions, List<Block> inAllBlocks, bool inIsSolutionBlock)
    {
        mBoardSize = inBoardSize;
        mBlockIndex = inBlockIndex;
        mGridPositions = inGridPositions;
        mAllBlocks = inAllBlocks;
        mCurrentValidPos = this.gameObject.transform.position;
        mUpperLimit = mCurrentValidPos.y + this.gameObject.transform.localScale.y;
        mLowerLimit = mCurrentValidPos.y - this.gameObject.transform.localScale.y;
        mIsSolutionBlock = inIsSolutionBlock;
    }

    // Update is called once per frame
    void Update()
    {

    }



    Vector2Int GetClosestIndex()
    {
        Vector2Int retval = new Vector2Int(-1, -1);
        float minDistance = float.MaxValue;
        for (int i = 0; i < mBoardSize.y; i++)
        {
            for (int j = 0; j < mBoardSize.x; j++)
            {
                if (Vector3.Distance(this.gameObject.transform.position, mGridPositions[i, j]) < minDistance)
                {
                    minDistance = Vector3.Distance(this.gameObject.transform.position, mGridPositions[i, j]);
                    retval = new Vector2Int(i, j);
                }
            }
        }


        return retval;
    }

    public abstract BlockType GetBlockType();
    public bool IsSolutionBlock() { return mIsSolutionBlock; }

    public BlockProperty GetBlockProperty()
    {
        return new BlockProperty(new Vector2Int((int)mBlockIndex.y, (int)mBlockIndex.x), GetBlockType(), IsSolutionBlock());
    }

    public abstract List<Vector2> GetBlockOccupiedIndex();

    protected List<Block> mAllBlocks = new List<Block>();

    protected List<Vector2> GetAllOccupiedCellsIndices()
    {
        List<Vector2> retval = new List<Vector2>();
        for (int i = 0; i < mAllBlocks.Count; i++)
        {
            if (mAllBlocks[i] != this)
            {
                retval.AddRange(mAllBlocks[i].GetBlockOccupiedIndex());

            }
        }



        return retval;
    }

    protected abstract int GetLowerLimit();



    protected abstract int GetLeftLimit();

    protected abstract int GetRightLimit();

    protected abstract int GetUpperLimit();



    public void OnTouch(Vector3 pos)
    {

        mPrevTouchPosition = pos;
    }
    public void OnRelease(Vector3 pos)
    {
        Vector2Int closestIndex = GetClosestIndex();
        PositionBlockByBlockIndex(closestIndex);
    }

    public void PositionBlockByBlockIndex(Vector2Int closestIndex)
    {
        this.gameObject.transform.position = mGridPositions[closestIndex.x, closestIndex.y];
        mCurrentValidPos = this.gameObject.transform.position;
        mBlockIndex = new Vector2(closestIndex.y, closestIndex.x);
        mMovementDirection = Direction.eNone;
    }

    public void OnDrag(Vector3 pos)
    {

        if (pos.x != mPrevTouchPosition.x || pos.y != mPrevTouchPosition.y)
        {
            float magX = (pos.x - mPrevTouchPosition.x);
            float magY = pos.y - mPrevTouchPosition.y;

            if (Mathf.Abs(magX) < mSensitivityThreshold && Mathf.Abs(magY) < mSensitivityThreshold)
            {
                return;
            }

            if (Mathf.Abs(magX) > Mathf.Abs(magY))
            {
                if (mMovementDirection == Direction.eHorizontal || mMovementDirection == Direction.eNone)
                {
                    mMovementDirection = Direction.eHorizontal;
                    if (pos.x > mPrevTouchPosition.x)
                    {
                        Vector3 currentPos = this.gameObject.transform.position;
                        float rightLimit = mCurrentValidPos.x + this.gameObject.transform.localScale.x * Mathf.Abs(mBlockIndex.x - GetRightLimit());

                        float newX = Mathf.Clamp(currentPos.x + Mathf.Abs(magX), currentPos.x, rightLimit);
                        this.gameObject.transform.position = new Vector3(newX, currentPos.y, 0f);
                    }
                    else
                    {
                        Vector3 currentPos = this.gameObject.transform.position;
                        float leftLimit = mCurrentValidPos.x - this.gameObject.transform.localScale.x * Mathf.Abs(mBlockIndex.x - GetLeftLimit());

                        float newX = Mathf.Clamp(currentPos.x - Mathf.Abs(magX), leftLimit, currentPos.x);
                        this.gameObject.transform.position = new Vector3(newX, currentPos.y, 0f);
                    }

                }
            }
            else
            {
                if (mMovementDirection == Direction.eVertical || mMovementDirection == Direction.eNone)
                {
                    mMovementDirection = Direction.eVertical;
                    if (pos.y < mPrevTouchPosition.y)
                    {   // drag downwards

                        Vector3 currentPos = this.gameObject.transform.position;
                        float lowerLimit = mCurrentValidPos.y - this.gameObject.transform.localScale.y * Mathf.Abs(mBlockIndex.y - GetLowerLimit());

                        float newY = Mathf.Clamp(currentPos.y - Mathf.Abs(magY), lowerLimit, currentPos.y);
                        this.gameObject.transform.position = new Vector3(currentPos.x, newY, 0f);
                    }
                    else
                    {

                        // drag upwards
                        Vector3 currentPos = this.gameObject.transform.position;
                        float upLimit = mCurrentValidPos.y + this.gameObject.transform.localScale.y * Mathf.Abs(mBlockIndex.y - GetUpperLimit());
                        float newY = Mathf.Clamp(currentPos.y + Mathf.Abs(magY), currentPos.y, upLimit);
                        this.gameObject.transform.position = new Vector3(currentPos.x, newY, 0f);

                    }


                }
            }

            mPrevTouchPosition = pos;
        }
    }



}
