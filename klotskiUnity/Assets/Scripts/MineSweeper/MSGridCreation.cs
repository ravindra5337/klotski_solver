using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MSGridCreation : MonoBehaviour
{



    GameObject mCurrentLevelElements;
    [SerializeField]
    TouchDetection mTouchDetection;
    [SerializeField]
    GameObject pUnit;
    [SerializeField]
     GameObject pGridBackground;
    [SerializeField]
    GameObject pScreenBackground;

    [SerializeField]
  int   mTotalColumns;
    [SerializeField]
    int mTotalRows;


  float  mSizeOfEachBox;

    List<MSUnit> mAllUnits;
    Vector3[,] mGridPositions;
    float mTopMostX;
    float mTopMostY;
    // Start is called before the first frame update
    void Start()
    {
        mTouchDetection.OnDrag += OnDrag;
        mTouchDetection.OnTouchMouseButton += OnTouch;
        mTouchDetection.OnRelease += OnRelease;

        CreateGridBG();
        CreateGrid();
        PlaceAllBombs();
    }
    void OnDrag(Vector3 pos)
    { }
    void OnRelease(Vector3 pos)
    { }


    void OnTouch(Vector3 pos,int inMouseButton)
    {
        float x = pos.x - mTopMostX;
        float y = pos.y - mTopMostY;

        if (pos.y > mTopMostY || pos.x < mTopMostX)
            return;

        float indexX = x / mSizeOfEachBox;
        float indexY = y / mSizeOfEachBox;

        int curX = (int)Mathf.Abs(indexX);
        int curY = (int)Mathf.Abs(indexY);
        

        if (curX < mTotalColumns && curY < mTotalRows && curX > -1 && curY > -1)
        {
          int id=  curY*mTotalColumns+curX;

            if (inMouseButton == 0)
            {
                mAllUnits[id].Reveal();
                if (!mAllUnits[id].IsABomb)
                {
                    mTotalClicks++;
                    Debug.LogWarning("Clicks Count " + mTotalClicks);
                }
            }
            else
            {
                mAllUnits[id].Mark();
            }
        }
    }

    int mTotalClicks = 0;

    void PlaceAllBombs()
    {
    List<int> bombsIds=    GetAllBombLocations(mTotalRows, mTotalColumns, mTotalColumns + mTotalRows);
        Debug.Log("Bombs count " + bombsIds.Count);
        foreach (MSUnit b in mAllUnits)
        {
            b.SetBomb(MSUnit.UnitRange.eNone);
        }
        foreach (int b in bombsIds)
        {
          
            mAllUnits[b].SetBomb(MSUnit.UnitRange.eBomb);
          
        }

        foreach (MSUnit b in mAllUnits)
        {
            if (!b.IsABomb)
            {
                List<MSUnit> leftSideBombs = mAllUnits.FindAll(x => x.IsABomb &&  x.RowIndex == b.RowIndex && x.ColIndex < b.ColIndex);
                List<MSUnit> rightSideBombs = mAllUnits.FindAll(x => x.IsABomb && x.RowIndex == b.RowIndex && x.ColIndex >b.ColIndex);

                List<MSUnit> upSideBombs = mAllUnits.FindAll(x => x.IsABomb && x.RowIndex < b.RowIndex && x.ColIndex == b.ColIndex);
                List<MSUnit> downSideBombs = mAllUnits.FindAll(x => x.IsABomb && x.RowIndex > b.RowIndex && x.ColIndex == b.ColIndex);

                if((leftSideBombs.Count>0 && rightSideBombs.Count>0) || (upSideBombs.Count > 0 && downSideBombs.Count> 0))
                {
                    b.SetBomb(MSUnit.UnitRange.eInRange);
                }
            }
        }
    }

    int[,] GridWithBombLocations(int rowCount, int colCount, int bombCount)
    {
        int[,] grid = new int[rowCount, colCount];
        for(int i = 0; i < rowCount; i++)
        {
            
        }

        return grid;
    }

    void PlaceBombInRow(int rowIndex,int rowCount, int colCount,ref int[,] grid)
    {
        
    }

    int IsBombExistInCol(int colIndex, int rowCount, int colCount, ref int[,] grid)
    {
        int retval = 0;
        for(int i = 0; i < rowCount; i++)
        {
            if (grid[i, colIndex] == 1)
                retval++;
        }


        return retval;
    }

    List<int> GetAllBombLocations(int rowCount ,int colCount, int bombCount)
    {
        List<int> retval = new List<int>();
        List<int> availableIds = new List<int>();
        for (int i = 0; i < rowCount * colCount; i++)
        {
            availableIds.Add(i);
        }
        for (int i = 0; i < bombCount && availableIds.Count>0; i++)
        {
            int randomLoc = availableIds[UnityEngine.Random.Range(0, availableIds.Count)];

            List<int> existingInRow = retval.FindAll(x => { return x / rowCount == randomLoc / rowCount; });
            List<int> existingInCol= retval.FindAll(x => { return x % rowCount == randomLoc % rowCount; });
            if ((existingInRow==null || existingInRow.Count < 2) && (existingInCol == null || existingInCol.Count < 2))
            {
                int rowIndex = randomLoc / rowCount;
                int colIndex = randomLoc % rowCount;
                retval.Add(randomLoc);
                if (existingInRow.Count == 1)
                {
                    availableIds.RemoveAll(x => { return x / rowCount == randomLoc / rowCount; });
                }
                else
                {
                    if(rowIndex-1>=0)
                    availableIds.Remove((rowIndex-1)*rowCount+ colIndex);
                    if (rowIndex +1<rowCount)
                        availableIds.Remove((rowIndex + 1) * rowCount + colIndex);
                    if (colIndex - 1 >= 0)
                        availableIds.Remove((rowIndex ) * rowCount + colIndex-1);
                    if (colIndex +1<rowCount)
                        availableIds.Remove((rowIndex) * rowCount + colIndex + 1);

                    availableIds.Remove(randomLoc);
                }
            
                if (existingInCol.Count == 1)
                {
                    availableIds.RemoveAll(x => { return x % rowCount == randomLoc % rowCount; });
                }
                else
                {
                    availableIds.Remove(randomLoc);
                    if (rowIndex - 1 >= 0)
                        availableIds.Remove((rowIndex - 1) * rowCount + colIndex);
                    if (rowIndex + 1 < rowCount)
                        availableIds.Remove((rowIndex + 1) * rowCount + colIndex);
                    if (colIndex - 1 >= 0)
                        availableIds.Remove((rowIndex) * rowCount + colIndex - 1);
                    if (colIndex + 1 < rowCount)
                        availableIds.Remove((rowIndex) * rowCount + colIndex + 1);
                }
            }


        }


        return retval;
    }


        // Update is called once per frame
        void Update()
    {
        
    }


    void CreateGrid()
    {
        if (mCurrentLevelElements == null)
            mCurrentLevelElements = new GameObject("current_grid");
        

        float height = Camera.main.orthographicSize * 2f;
        float totalHeight = height;
        float width = Camera.main.aspect * height;
        width = width * 0.90f;
        height = 0.72f * height;
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
                box.transform.parent = mCurrentLevelElements.transform;

                if (mAllUnits == null)
                    mAllUnits = new List<MSUnit>();

                box.name = "msunit_" + mAllUnits.Count;
                box.GetComponent<MSUnit>().Initialize(i, j);
                mAllUnits.Add(box.GetComponent<MSUnit>());
            }
            starty -= sizeOfEachBox;
        }
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
}
