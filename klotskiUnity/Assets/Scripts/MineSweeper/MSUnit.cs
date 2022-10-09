using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MSUnit : MonoBehaviour
{
    public enum UnitRange
    {
        eNone,
        eInRange,
        eBomb
    }

    [SerializeField]
    SpriteRenderer mCoverSprite;
    [SerializeField]
    SpriteRenderer mMainSpriteRender;

    [SerializeField]
    UnitRange mUnitRange;

    int mRowIndex;

    int mColIndex;


    public int RowIndex { get {
            return mRowIndex;
        } }
    public int ColIndex
    {
        get
        {
            return mColIndex;
        } }


    public bool IsABomb { get { return mUnitRange==UnitRange.eBomb; } }

    public void SetBomb(UnitRange inRange)
    {
        mUnitRange = inRange;
        mMainSpriteRender.color = GetColorForRange(inRange);
    }

    public void Initialize(int inRow,int inCol)
    {
        mRowIndex = inRow;
        mColIndex = inCol;
    }

    public Color GetColorForRange(UnitRange inRange)
    {
        Color c = Color.white;
        switch (inRange)
        {
            case UnitRange.eBomb:
                c = Color.red;
                break;
            case UnitRange.eInRange:
                c = Color.yellow;
                break;
            case UnitRange.eNone:
                c = Color.green;
                break;
            default:
                break;
        }

        return c;
    }
    public void Reveal()
    {
        mCoverSprite.gameObject.SetActive(false);
        mIsRevealed = true;
        mMainSpriteRender.color = GetColorForRange(mUnitRange);
    }

    public void Mark()
    {
        mCoverSprite.gameObject.SetActive(true);
        mCoverSprite.color = Color.magenta;
    }

    bool mIsRevealed = false;
    public void Hide()
    {
        mCoverSprite.gameObject.SetActive(true);
        mIsRevealed = false;

    }
}
