using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchDetection : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public delegate void MouseDrag(Vector3 pos);
    public delegate void MouseDragButton(Vector3 pos,int mouseBtn);

    public event MouseDrag OnDrag;
    public event MouseDrag OnTouch;
    public event MouseDragButton OnTouchMouseButton;
    public event MouseDrag OnRelease;
    bool mTouched = false;
    // Update is called once per frame
    void Update()
    {
        Vector3 pos = Input.mousePosition;
        pos = Camera.main.ScreenToWorldPoint(pos);

        
        if (Input.GetMouseButtonDown(0))
        {

            if (!mTouched)
            {
                if (OnTouch != null)
                {
                    OnTouch(pos);
                }
                
            }
            mTouched = true;

            if (OnTouchMouseButton != null)
                OnTouchMouseButton(pos, 0);
        }

        if (Input.GetMouseButtonDown(1))
        {

           
                if (OnTouchMouseButton != null)
                    OnTouchMouseButton(pos, 1);
           
        }


        if (Input.GetMouseButtonUp(0))
        {
            mTouched = false;
            if (OnRelease != null)
            {
                OnRelease(pos);
            }
        }

        if (mTouched)
        {
            if (Input.GetMouseButton(0))
            {
               
                if (OnDrag != null)
                {
                    OnDrag(pos);
                }
            }
        }
    }
}
