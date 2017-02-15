using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyMobile;
using System;

public class EasyMobileManager : MonoBehaviour {

    static EasyMobileManager instance;
    public static EasyMobileManager Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    public static void ShowInterstitialAd()
    {
        instance.StartCoroutine(instance.CoShowInterstitialAd());
    }

	// Use this for initialization
	void Start () {

	}

    private IEnumerator CoShowInterstitialAd()
    {
        yield return new WaitUntil(() => 
        {
            return AdManager.IsInterstitialAdReady();
        });
        AdManager.ShowInterstitialAd();
    }



    // Update is called once per frame
    void Update () {
		
	}
}
