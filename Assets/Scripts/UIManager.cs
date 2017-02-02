using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {
    static UIManager instance;
    public static UIManager Instance
    {
        get { return instance; }
    }

    public GameObject goBtnGameStart;
    public GameObject goBtnGameRestart;

    public GameObject goImgGameWin;
    public GameObject goImgGameLose;

    private void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

}
