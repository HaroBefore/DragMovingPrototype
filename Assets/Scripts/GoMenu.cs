using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MadLevelManager;

public class GoMenu : MonoBehaviour
{

    public void Go()
    {
        GameManager.Instance.gameState = eGameState.gameOver;
        MadLevel.LoadLevelByName("LevelSelectScreen");
    }
    public void GoNext()

    {
        GameManager.Instance.gameState = eGameState.gameOver;
        MadLevel.LoadNext();
    }

}
