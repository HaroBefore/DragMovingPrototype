using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using EasyMobile;
using MadLevelManager;
using UnityEngine.UI;
using HeavyDutyInspector;

public class GameManager : MonoBehaviour {
    static GameManager instance;
    public static GameManager Instance
    {
        get { return instance; }
    }

    public event Action EventGameStart;

    public eGameState gameState = eGameState.None;

    public bool isShowAd = false;

    UIManager uiManager;

    GoalCtrl goalCtrl;
    [HideInInspector]
    public PlayerCtrl playerCtrl;

    [Comment("Zone에 있지 않을 때 기본 TimeScale 배율")]
    public float baseicClickedTimeScaleMultiply = 0.05f;

    private void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    IEnumerator Start () {

        //gameState = eGameState.gameStartWaiting;
        //OnGameStart();

        var async = SceneManager.LoadSceneAsync("PlayUI", LoadSceneMode.Additive);

        yield return new WaitUntil(() => { return async.isDone; });

        goalCtrl = GameObject.Find("Goal").GetComponent<GoalCtrl>();
        playerCtrl = GameObject.Find("Player").GetComponent<PlayerCtrl>();

        uiManager = UIManager.Instance; 

        OnGameStart();
    }

    public void OnGameStart()
    {
        StartCoroutine(CoGameStart());
    }

    public void OnGameRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    IEnumerator CoGameStart()
    {
        if(isShowAd)
            AdManager.ShowBannerAd(BannerAdPosition.Bottom, BannerAdSize.SmartBanner);
        yield return null;

        //레벨 번호 보이고 사라짐
        int levelNum = 0;
        Int32.TryParse(MadLevel.arguments, out levelNum);
        Debug.Log("arg " + MadLevel.arguments);
        Debug.Log("levelNum " + levelNum);
        uiManager.textLevelNum.GetComponent<Text>().text = " - " + levelNum + " - ";

        uiManager.textLevelNum.SetActive(true);
        //////////

        //ObstacleCtrl 들 시작
        if (EventGameStart != null)
            EventGameStart();
        //////////////

        goalCtrl.trigger.radius = 0.4f;

        yield return new WaitForSeconds(0.8f);
        yield return StartCoroutine(goalCtrl.CoGameStart());
        yield return StartCoroutine(playerCtrl.CoGameStart());
        if(isShowAd)
            AdManager.HideBannerAd(BannerAdNetwork.AdMob);

        uiManager.textLevelNum.SetActive(false);

        //UI 클리어시 이벤트 추가
        uiManager.goBtnGameNext.GetComponent<Button>().onClick.AddListener(() =>
        {
            MadLevel.LoadNext();
        });
        uiManager.goBtnGameBack.GetComponent<Button>().onClick.AddListener(() =>
        {
            MadLevel.LoadLevelByName("LevelSelectScreen");
        });
        uiManager.goBtnGameRestart.GetComponent<Button>().onClick.AddListener(() =>
        {
            MadLevel.ReloadCurrent();
        });
        //////////////////

        gameState = eGameState.gamePlaying;

        if(isShowAd)
            EasyMobileManager.ShowInterstitialAd();
    }

    public IEnumerator CoGameWin()
    {
        gameState = eGameState.gameOver;
        if (MadLevel.HasNext())
            uiManager.goBtnGameNext.SetActive(true);
        uiManager.goImgGameWin.SetActive(true);
        uiManager.goBtnGameRestart.SetActive(true);
        uiManager.goBtnGameBack.SetActive(true);
        yield return null;
    }

    public IEnumerator CoGameLose()
    {
        gameState = eGameState.gameOver;
        uiManager.goImgGameLose.SetActive(true);
        uiManager.goBtnGameRestart.SetActive(true);
        uiManager.goBtnGameBack.SetActive(true);
        yield return null;
    }

    // Update is called once per frame
    void Update () {
		
	}
}
