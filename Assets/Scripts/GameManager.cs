using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    static GameManager instance;
    public static GameManager Instance
    {
        get { return instance; }
    }

    public event Action EventGameStart;

    public eGameState gameState = eGameState.None;

    SpawnManager spawnManager;

    GoalCtrl goalCtrl;
    public PlayerCtrl playerCtrl;

    private void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start () {
        goalCtrl = GameObject.Find("Goal").GetComponent<GoalCtrl>();
        playerCtrl = GameObject.Find("Player").GetComponent<PlayerCtrl>();
        spawnManager = SpawnManager.Instance;

        //gameState = eGameState.gameStartWaiting;
        OnGameStart();

    }

    public void OnGameStart()
    {
        UIManager.Instance.goBtnGameStart.SetActive(false);
        StartCoroutine(CoGameStart());
    }

    public void OnGameRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    IEnumerator CoGameStart()
    {
        yield return null;
        yield return StartCoroutine(goalCtrl.CoGameStart());
        yield return StartCoroutine(spawnManager.CoSpawnObstacles());
        
        if (EventGameStart != null)
            EventGameStart();

        goalCtrl.trigger.radius = 0.4f;
        playerCtrl.trigger.radius = 0.4f;

        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(playerCtrl.CoGameStart());

        gameState = eGameState.gamePlaying;
    }

    public IEnumerator CoGameWin()
    {
        gameState = eGameState.gameOver;
        UIManager.Instance.goImgGameWin.SetActive(true);
        UIManager.Instance.goBtnGameRestart.SetActive(true);
        yield return null;
    }

    public IEnumerator CoGameLose()
    {
        gameState = eGameState.gameOver;
        UIManager.Instance.goImgGameLose.SetActive(true);
        UIManager.Instance.goBtnGameRestart.SetActive(true);
        yield return null;
    }

    // Update is called once per frame
    void Update () {
		
	}
}
