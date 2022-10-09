using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    GridGenerator mGridGenerator;

    [SerializeField]
    GameObject mInGameUI;
   public void OnClickPlayButton()
    {
        this.gameObject.SetActive(false);
        mInGameUI.SetActive(true);
        mGridGenerator.StartGame(1);
    }
}
