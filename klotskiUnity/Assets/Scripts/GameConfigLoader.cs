using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConfigLoader 
{
  

    GameLevels mAllGameLevels;


    static GameConfigLoader mLoader;


    public static GameConfigLoader Instance
    {
        get
        {
            if (mLoader == null)
            {

                mLoader = new GameConfigLoader();
            }


            return mLoader;
        }
    }


    GameConfigLoader()
    {

        TextAsset lvlConf = Resources.Load<TextAsset>("ConfigFiles/levels");
        mAllGameLevels = JsonConvert.DeserializeObject<GameLevels>(lvlConf.text);
    }
  

    public LevelConfig GetLevel(int levelno)
    {
        return mAllGameLevels.GetLevel(levelno);
    }
   
}
